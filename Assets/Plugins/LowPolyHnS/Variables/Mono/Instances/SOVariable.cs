using UnityEngine;

namespace LowPolyHnS.Variables
{
    public class SOVariable : ScriptableObject
    {
        public Variable variable = new Variable();

        // EDITOR METHODS: ------------------------------------------------------------------------

#if UNITY_EDITOR

        public bool isExpanded = false;

        public bool CanSave()
        {
            return variable.CanSave();
        }

#endif
    }
}