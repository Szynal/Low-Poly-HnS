using System;
using UnityEngine;

namespace LowPolyHnS.Variables
{
    [Serializable]
    public class Tag
    {
        public static readonly string[] COLOR_NAMES =
        {
            "Red",
            "Pink",
            "Grape",
            "Violet",
            "Indigo",
            "Blue",
            "Cyan",
            "Teal",
            "Green",
            "Lime",
            "Yellow",
            "Orange"
        };

        public static readonly HexColor[] COLOR_HEXS =
        {
            new HexColor("#f03e3e"), // Red
            new HexColor("#d6336c"), // Pink
            new HexColor("#ae3ec9"), // Grape
            new HexColor("#7048e8"), // Violet
            new HexColor("#4263eb"), // Indigo
            new HexColor("#1c7ed6"), // Blue
            new HexColor("#1098ad"), // Cyan
            new HexColor("#0ca678"), // Teal
            new HexColor("#37b24d"), // Green
            new HexColor("#74b816"), // Lime
            new HexColor("#f59f00"), // Yellow
            new HexColor("#f76707") // Orange
        };

        // PROPERTIES: ----------------------------------------------------------------------------

        public string name;
        public int color;

        // INITIALIZERS: --------------------------------------------------------------------------

        public Tag()
        {
            name = "";
            color = 0;
        }

        public Tag(string name, int color)
        {
            this.name = name;
            this.color = color;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public Color GetColor()
        {
            return COLOR_HEXS[color].GetColor();
        }
    }
}