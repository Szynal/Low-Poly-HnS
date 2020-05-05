﻿using UnityEngine;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class IgniterMouseUp : Igniter
    {
        public enum MouseButton
        {
            Left = KeyCode.Mouse0,
            Right = KeyCode.Mouse1,
            Middle = KeyCode.Mouse2
        }

        public MouseButton mouseButton = MouseButton.Left;

#if UNITY_EDITOR
        public new static string NAME = "Input/On Mouse Up";
#endif

        private void Update()
        {
            if (Input.GetKeyUp((KeyCode) mouseButton))
            {
                ExecuteTrigger(gameObject);
            }
        }
    }
}