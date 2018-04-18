using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tap.Tilt
{
    public class SelectionController : MonoBehaviour
    {
        // The game object to show selection
        public GameObject selectionObject;

        // When an object is selected, see if it has a controller of any of these types
        // to run the Selector script on

        public void TriggerEnter(GameObject other)
        {
            Indicate();
        }
        public void TriggerLeave()
        {
            Dimmer();
        }

        // Turn on the indicator for this object
        public void Indicate()
        {
            selectionObject.SetActive(true);
        }

        // Turn off indicator for this object
        public void Dimmer()
        {
            selectionObject.SetActive(false);
        }
    }
}
