using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tap.Tilt
{
    /**
     * An audio sample controller which reacts to the user inputs
     */
    public class FireworksController : ObjectController
    {
        // The root particle engine for this fireworks item
        public ParticleSystem particle;

        // Override the TouchInput method
        public override void TouchInput(ControlData controlData)
        {
            string touchType = controlData.type;
            TouchData[] touches = controlData.touches;
            ScreenData screen = controlData.screen;

            // touchType will be touchstart, touchend, touchcancel, or touchmove
            switch (touchType)
            {
                case "touchstart":
                    TouchStart(touches);
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

        void TouchStart(TouchData[] touches)
        {
            Debug.Log("TouchStart Fireworks");

            // update the lastTouchStart
            lastTouchStart = Time.time;

            // Fire off a particle
            if (particle)
                particle.Emit(1);
            
        }
    }
}
