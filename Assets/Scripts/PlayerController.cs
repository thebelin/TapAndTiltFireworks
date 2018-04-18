using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tap.Tilt
{
    // Use to control a player object
    public class PlayerController : MonoBehaviour
    {
        // The socket id for identifying the client
        public string socketId;

        // The objects to use as the representative reticle
        public GameObject[] reticles;

        // The hinge joint used to move things around in the scene
        public HingeJoint hinge;

        // The highlighted SelectionController, if it exists
        private SelectionController indicated;

        // The selected SampleController, if it exists
        private SelectionController selected;

        // The hit to report
        private RaycastHit hit;

        // update each time touch data is processed
        [HideInInspector]
        public TouchData[] lastTouches;

        // the last time Touch Started
        [HideInInspector]
        public float lastTouchStart;

        private void OnEnable()
        {
            if (hinge == null)
                hinge = GetComponent<HingeJoint>();
            
            // destroy any existing reticles
            foreach (GameObject reticle in reticles)
                reticle.SetActive(false);
        }

        private void LateUpdate()
        {
            Scan(transform.rotation * Vector3.forward);
            if (!indicated)
                Scan(transform.rotation * Vector3.up);
            if (!indicated)
                Scan(transform.rotation * Vector3.right);
            if (!indicated)
                Scan(transform.rotation * Vector3.left);
            if (!indicated)
                Scan(transform.rotation * Vector3.back);
        }

        // Get the corresponding previous touch location for this touch location
        private TouchData PreviousTouch(TouchData touch)
        {
            foreach (TouchData thisTouch in lastTouches)
                if (thisTouch.Equals(touch))
                    return thisTouch;

            return null;
        }

        // cast a ray in the specified direction and call any SampleController.TriggerEnter found
        private void Scan(Vector3 direction)
        {
            // Draw a debug ray for the scanner line
            Debug.DrawRay(transform.position, direction);

            Ray origin = new Ray(transform.position, direction);

            // Get a Raycast Hit (and save it for the hinge position)
            if (Physics.Raycast(origin, out hit, 100))
            {
                SelectionController sampleHit = hit.collider.gameObject.GetComponent<SelectionController>();
                // If something is already indicated, check if it's different from currently indicated
                if (indicated != null && !indicated.Equals(sampleHit))
                    indicated.TriggerLeave();
                    
                // Debug.Log("There is " + hit.collider.gameObject.name + " detected");
                indicated = sampleHit;
                if (indicated != null)
                    indicated.TriggerEnter(hit.collider.gameObject);
            }
            else if (indicated)
            {
                indicated.TriggerLeave();
                indicated = null;
            }
        }

        // Rotate this towards the rotation sent
        public void Tilt (Quaternion tilt)
        {
            transform.localRotation = tilt;
        }

        // React to touches
        public void Touch(ControlData controlData)
        {
            // Pass the touch data on to an indicated object
            if (indicated != null)
            {
                ObjectController[] controlled = indicated.gameObject.GetComponents<ObjectController>();
                foreach (ObjectController cont in controlled)
                {
                    Debug.Log("Controlled: " + cont.name);
                    cont.TouchInput(controlData);
                }
            }

            // touchType will be touchstart, touchend, touchcancel, or touchmove
            switch (controlData.type)
            {
                case "touchend":
                case "touchcancel":
                    TouchEnd(controlData);
                    break;
                case "touchstart":
                    // activate reticles at all the active touchpoints
                    TouchStart(controlData);
                    break;
                case "touchmove":
                    // update the touch reticle positions
                    TouchMove(controlData);
                    break;
            }
            // update the lastTouches
            lastTouches = controlData.touches;
        }

        // activate reticles at all the touchpoints
        private void TouchStart(ControlData controlData)
        {
            TouchData[] touches = controlData.touches;
            ScreenData screen = controlData.screen;

            // deactivate the reticles
            foreach (GameObject retic in reticles)
                retic.SetActive(false);

            // If there's an item indicated, move the indicated to selected status
            if (indicated != null)
            {
                selected = indicated;
                Debug.Log("selected indicated: " + indicated.gameObject.name);
                if (hinge)
                {
                    hinge.transform.position = hit.point;
                    hinge.connectedBody = selected.gameObject.GetComponent<Rigidbody>();
                }
            }

            foreach (TouchData touch in touches)
            {
                int touchIndex = Int32.Parse(touch.identifier);
                if (touchIndex < reticles.Length)
                {
                    // Activate the reticle
                    reticles[touchIndex].SetActive(true);

                    // Move the reticle to the indicated relative touch location
                    reticles[touchIndex].transform.localPosition = TouchToScreen(touch, screen);
                }
            }
        }

        // deactivate reticles which aren't being touched
        private void TouchEnd(ControlData controlData)
        {
            TouchData[] ended = controlData.ended;

            if (ended.Length == 0)
            {
                // Deactivate all
                foreach (GameObject reticle in reticles)
                    reticle.SetActive(false);

                // Drop hinge
                hinge.connectedBody = null;
            }

            foreach (TouchData touch in ended)
            {
                // Debug.Log("Touch Data: " + JsonUtility.ToJson(touch).ToString());
                int touchIndex = Int32.Parse(touch.identifier);
                if (touchIndex < reticles.Length)
                {
                    // DeActivate the specified reticle
                    reticles[touchIndex].SetActive(false);
                }
                // Drop hinge if reticle 0
                if (touchIndex == 0)
                {
                    hinge.connectedBody = null;
                    hinge.transform.localPosition = new Vector3(0, 0, 1);
                }
            }
        }

        private void TouchMove(ControlData controlData)
        {
            // for each of the touches, find the corresponding reticle and move it to the specified location
            foreach (TouchData touch in controlData.touches)
            {
                int touchIndex = Int32.Parse(touch.identifier);
                if (touchIndex < reticles.Length)
                {
                    // Move the reticle to the indicated relative touch location
                    reticles[touchIndex].transform.localPosition = TouchToScreen(touch, controlData.screen);
                }
                // Get the difference between the previous touch and the current touch
                TouchData previousTouch = PreviousTouch(touch);
                if (previousTouch == null)
                {
                    Debug.Log("no previous touch");
                    break;
                }
                    

                float diffX = (touch.pageX - previousTouch.pageX);
                float diffY = (touch.pageY - previousTouch.pageY);

                // 0 Touch moves the position of the sample
                if (touch.identifier == "0")
                {
                    // Apply the Vertical touch as depth to the hinge
                    hinge.transform.localPosition = new Vector3(0, 0, hinge.transform.localPosition.z + (diffY * -.001f));
                }
            }
        }

        private Vector3 TouchToScreen(TouchData touch, ScreenData screen)
        {
            return new Vector3(
                2 * (touch.pageX - (screen.width / 2)) / screen.width,
                0,
                -5 * (touch.pageY - screen.height) / screen.height);
        }

        // Some comparisons
        public bool Equals (PlayerController other)
        {
            if (other == null) return false;
            return (this.socketId.Equals(other.socketId));
        }
        public bool Equals (string other)
        {
            if (other == null) return false;
            return (this.socketId.Equals(other));
        }
    }
}
