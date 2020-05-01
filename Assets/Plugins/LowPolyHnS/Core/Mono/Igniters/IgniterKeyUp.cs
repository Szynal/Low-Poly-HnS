using UnityEngine;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class IgniterKeyUp : Igniter
    {
        public KeyCode keyCode = KeyCode.Space;

#if UNITY_EDITOR
        public new static string NAME = "Input/On Key Up";
#endif

        private void Update()
        {
            if (Input.GetKeyUp(keyCode))
            {
                ExecuteTrigger(gameObject);
            }
        }
    }
}