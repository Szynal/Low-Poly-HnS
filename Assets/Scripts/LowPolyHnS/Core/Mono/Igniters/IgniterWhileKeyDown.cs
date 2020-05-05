using UnityEngine;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class IgniterWhileKeyDown : Igniter
    {
        public KeyCode key = KeyCode.Space;

#if UNITY_EDITOR
        public new static string NAME = "Input/While Key Down";
#endif

        private void Update()
        {
            if (Input.GetKey(key))
            {
                ExecuteTrigger(gameObject);
            }
        }
    }
}