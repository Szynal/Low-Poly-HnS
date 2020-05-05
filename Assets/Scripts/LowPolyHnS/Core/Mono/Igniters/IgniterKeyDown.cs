using UnityEngine;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class IgniterKeyDown : Igniter
    {
        public KeyCode keyCode = KeyCode.Space;

#if UNITY_EDITOR
        public new static string NAME = "Input/On Key Down";
#endif

        private void Update()
        {
            if (Input.GetKeyDown(keyCode))
            {
                ExecuteTrigger(gameObject);
            }
        }
    }
}