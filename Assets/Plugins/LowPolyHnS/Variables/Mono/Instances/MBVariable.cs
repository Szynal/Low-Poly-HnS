using UnityEngine;

namespace LowPolyHnS.Variables
{
    [AddComponentMenu("")]
    public class MBVariable : MonoBehaviour
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