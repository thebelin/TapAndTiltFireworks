using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using BestHTTP.SocketIO;

namespace Tap.Tilt
{
	// Handles all socketio network transport
    [RequireComponent(typeof(GameController))]
	public class SocketIONetwork : MonoBehaviour {
		public string uri = "https://slab-fondler.herokuapp.com/socket.io/display";

        // A text mesh which shows the root of the selected url to the users
        public TextMesh showUrl;

        // The game controller where the data is sent to manage the game
        private GameController gc;

		private Socket Socket;
		private Socket RootSocket;
        
		// number of seconds between server pings
		private float pingDelay = 30.0f;

		// The last ping time
		private float lastPing = 0f;

		private SocketManager manager;

		// Set to true when connected
		private bool connectState = false;

		// Reconnect interval for reconnect attempts
		private float reconnectInterval = 1f;
		private float lastAttempt;

		// Create events for the server to respond to
		void OnEnable () {
			// SocketConnect ();
            gc = GetComponent<GameController>();
		}

		public void SocketConnect () {

            // Show the user URL
            if (showUrl != null)
                showUrl.text = uri.Replace("/socket.io/display", "");

			// Build an options object and disable reconnect to keep the system from timing itself out
			// @todo: investigate this line for the Windows implementation
			SocketOptions options = new SocketOptions();
			options.Reconnection = false;

			// Create the SocketManager instance
			manager = new SocketManager(new Uri(uri), options);

			// Start the socket
			Debug.Log("Starting Socket on server " + uri);
			Socket = manager["/display"];
			RootSocket = manager.Socket;

			// Add error handler, so we can display it
			Socket.On(SocketIOEventTypes.Error, OnError);

			// Set up our event handlers.
			Socket.On(SocketIOEventTypes.Connect, OnConnected);

			Socket.On(SocketIOEventTypes.Disconnect, OnDisconnected);

			// Generic control changes
			Socket.On ("control", (socket, packet, args) => {
                // Debug.Log("control event on socket" + packet.ToString());

                ControlData cd = JsonUtility.FromJson<ControlData>(MessageData(packet.ToString()));
                
                // Send the control data to the Game Controller
                gc.UserControl(cd);
		    });

            Socket.On("controlsDisconnect", (socket, packet, args) => {
                Debug.Log("disconnect event on socket" + packet.ToString());
                gc.UserExit(MessageData(packet.ToString().Replace("\"", "")));
            });
		}

		/// <summary>
		/// Socket connected event.
		/// </summary>
		private void OnConnected(Socket socket, Packet packet, params object[] args)
		{
			Debug.Log("OnConnected");
			connectState = true;
			Identify ();
		}
		private void OnDisconnected(Socket socket, Packet packet, params object[] args)
		{
			Debug.Log("DISCONNECTED");
			connectState = false;
		}

		private void LateUpdate()
		{
			// If not connected the try to reconnect at the interval rate
			if (!connectState && lastAttempt + reconnectInterval < Time.time) {
				lastAttempt = Time.time;
				SocketConnect ();
			}
		}
		/// <summary>
		/// Identify this instance.
		/// </summary>
		private void Identify()
		{
            // Check if there's a saved identity for this instance
            // Attach the unity id to the identity
            string identity = PlayerPrefs.GetString("deviceUniqueIdentifier", SystemInfo.deviceUniqueIdentifier);

            // Save the id
            PlayerPrefs.SetString("deviceUniqueIdentifier", identity);

			// Send the identity of this server
			Socket.Emit ("identify", JsonUtility.ToJson (identity));
		}

		/// <summary>
		/// Called on local or remote error.
		/// </summary>
		private void OnError(Socket socket, Packet packet, params object[] args)
		{
			Error error = args[0] as Error;
			switch (error.Code)
			{
				case SocketIOErrors.User:
				Debug.Log("Exception in an event handler!");
				break;
				case SocketIOErrors.Internal:
				Debug.Log("Internal error!");
				break;
				default:
				Debug.Log("Server error! " + error.Code.ToString() + " " + error.Message.ToString());
				break;
			}
			Debug.Log(error.ToString());
		}

		public void Send(string eventName, params object [] data)
		{
			// Debug.Log("Socket Send " + eventName + data.ToString());
			Socket.Emit(eventName, data);
		}

		// Helper to extract data from JSON encoded strings
		private string MessageData(string message)
		{
			int startIndex = message.IndexOf(",");
			return (startIndex >= 1) ? message.Substring(startIndex + 1, message.Length - (startIndex + 2)) : "";
		}

		// Update is called once per frame
		void Update () {
			if (RootSocket != null && lastPing + pingDelay > Time.time)
			{
				lastPing = Time.time;
				RootSocket.Emit("ping");
			}
		}
	}
}