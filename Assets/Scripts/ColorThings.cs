using UnityEngine;
using UnityEngine.UI;

namespace Tap.Tilt
{
    public class ColorThings : MonoBehaviour
    {

        // The objects to color with the specified user color
        public GameObject[] objectsToColor;

        // The current color
        private Color _color;
 
         // Change the render color of attached objects with Renderer
        public void RenderColor(Color renderColor)
        {
            if (objectsToColor == null) return;
            foreach (GameObject obj in objectsToColor)
            {
                Renderer r = obj.gameObject.GetComponent<Renderer>();
                if (r != null)
                {
                    r.material.SetColor("_Color", _color);
                }
            }
        }

        // Color all the things
        public void DoColor(Color newColor)
        {
            _color = newColor;

            if (objectsToColor == null) return;
            foreach (GameObject obj in objectsToColor)
            {
                if (obj != null)
                {
                    Renderer r = obj.gameObject.GetComponent<Renderer>();
                    if (r != null)
                    {
                        // For particles
                        r.material.SetColor("_TintColor", _color);

                        // For standard shader
                        r.material.SetColor("_Color", _color);

                        // For emission shader
                        r.material.SetColor("_EmissionColor", _color);
                    }
                    Text t = obj.gameObject.GetComponent<Text>();
                    if (t != null)
                    {
                        t.color = _color;
                    }
                    SpriteRenderer sr = obj.gameObject.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        sr.color = _color;
                    }
                    Image i = obj.GetComponent<Image>();
                    if (i != null)
                    {
                        i.color = _color;
                    }
                }
            }
        }

        // converter utility
        private static Color hexToColor(string hex)
        {
            hex = hex.Replace("0x", "");//in case the string is formatted 0xFFFFFF
            hex = hex.Replace("#", "");//in case the string is formatted #FFFFFF
            byte a = 255;//assume fully visible unless specified in hex
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            //Only use alpha if the string has enough characters
            if (hex.Length == 8)
            {
                a = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return new Color32(r, g, b, a);
        }

        // converter utility
        public static string colorToHex(Color32 color)
        {
            string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
            return hex;
        }
    }
}