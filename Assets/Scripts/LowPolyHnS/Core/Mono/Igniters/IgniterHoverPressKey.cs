using UnityEngine;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class IgniterHoverPressKey : Igniter
    {
#if UNITY_EDITOR
        public new static string NAME = "Input/On Hover Press Key";
        public new static bool REQUIRES_COLLIDER = true;
#endif

        public KeyCode keyCode = KeyCode.E;
        private bool isMouseOver;

        private void Update()
        {
            if (isMouseOver && Input.GetKeyDown(keyCode))
            {
                ExecuteTrigger(gameObject);
            }
        }

        private void OnMouseExit()
        {
            isMouseOver = false;
        }

        private void OnMouseEnter()
        {
            isMouseOver = true;
        }
    }
}