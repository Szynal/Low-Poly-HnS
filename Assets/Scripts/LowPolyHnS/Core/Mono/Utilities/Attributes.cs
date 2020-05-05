using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace LowPolyHnS.Core
{
    // --------------------------------------------------------------------------------------------

    public class VariableAttribute : PropertyAttribute
    {
        public string controlName;
        public List<string> suggestions;
        public string suggestionsSeed;
        public GUIContent suggestedVariables;

        public VariableAttribute()
        {
            controlName = GenerateRandomString();
            suggestions = new List<string>();
            suggestionsSeed = "";

            suggestedVariables = new GUIContent("Suggested Variables");
        }

        private string GenerateRandomString()
        {
            string alphabet = "abcdefghijklmnopqrstuvwxyz1234567890";
            char[] characters = new char[8];
            Random random = new Random();

            for (int i = 0; i < characters.Length; i++)
            {
                characters[i] = alphabet[random.Next(alphabet.Length)];
            }

            return characters.ToString();
        }
    }

    // --------------------------------------------------------------------------------------------

    public class LocStringNoTextAttribute : PropertyAttribute
    {
    }

    public class LocStringNoPostProcessAttribute : PropertyAttribute
    {
    }

    public class LocStringBigTextAttribute : PropertyAttribute
    {
    }

    public class LocStringBigTextNoPostProcessAttribute : PropertyAttribute
    {
    }

    // --------------------------------------------------------------------------------------------

    public class RotationConstraintAttribute : PropertyAttribute
    {
    }

    public class TagSelectorAttribute : PropertyAttribute
    {
    }

    public class LayerSelectorAttribute : PropertyAttribute
    {
    }

    public class IndentAttribute : PropertyAttribute
    {
    }

    // --------------------------------------------------------------------------------------------

    public class EventNameAttribute : PropertyAttribute
    {
    }
}