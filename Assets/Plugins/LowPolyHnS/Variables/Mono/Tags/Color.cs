using System;
using UnityEngine;

namespace LowPolyHnS.Variables
{
    [Serializable]
    public class HexColor
    {
        public string hex;

        private bool init;
        private Color color;

        // INITIALIZERS: --------------------------------------------------------------------------

        public HexColor()
        {
            hex = "#ffffff";
            init = false;
            color = Color.white;
        }

        public HexColor(string hex)
        {
            this.hex = hex;
            init = false;
            color = Color.white;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public Color GetColor()
        {
            if (!init)
            {
                init = true;
                ColorUtility.TryParseHtmlString(hex, out color);
            }

            return color;
        }
    }
}