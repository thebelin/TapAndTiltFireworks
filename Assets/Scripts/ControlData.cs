using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Tap.Tilt
{
    [Serializable]
    /**
     * The Touch information from a touch event
     */
    public class TouchData
    {
        public string identifier;
        public float pageX;
        public float pageY;
        public float force;
        public float radiusX;
        public float radiusY;

        public bool Equals (TouchData other)
        {
            return (other.identifier == this.identifier);
        }
    }

    [Serializable]
    /**
     * Screen data to figure out relative location
     */
    public class ScreenData
    {
        public float width;
        public float height;
    }

    [Serializable]
    public class ControlData
    {
        // The interface identifier
        public string i;

        // What type of event this is
        public string type;

        // Touch data
        public TouchData[] touches;

        // Ended touches
        public TouchData[] ended;

        // Tilt data
        public Quaternion tilt;

        // Screen data
        public ScreenData screen;
    }

}