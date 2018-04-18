using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tap.Tilt
{
    public class ObjectController : MonoBehaviour
    {
        // update each time touch data is processed
        [HideInInspector]
        public TouchData[] lastTouches;

        // the last time Touch Started
        [HideInInspector]
        public float lastTouchStart;

        public virtual void TouchInput(ControlData controlData)
        {
            string touchType = controlData.type;
            TouchData[] touches = controlData.touches;
            ScreenData screen = controlData.screen;

            // touchType will be touchstart, touchend, touchcancel, or touchmove
            switch (touchType)
            {
                case "touchstart":
                    break;
                case "touchmove":
                    break;
                case "touchend":
                    break;
                case "touchcancel":
                    break;
            }
            // update the lastTouches
            lastTouches = touches;
        }
    }
}
