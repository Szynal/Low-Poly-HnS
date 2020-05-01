using UnityEngine;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class IgniterHoverHoldKey : Igniter
    {
#if UNITY_EDITOR
        public new static string NAME = "Input/On Hover Hold Key";
        public new static bool REQUIRES_COLLIDER = true;
#endif

        public enum ReleaseType
        {
            OnKeyUp,
            OnTimeout
        }

        public KeyCode keyCode = KeyCode.E;
        public float holdTime = 0.5f;
        public ReleaseType execute = ReleaseType.OnKeyUp;

        private bool isPressing;
        private bool isMouseOver;
        private float downTime = -9999.0f;

        private void Update()
        {
            if (isMouseOver)
            {
                if (isPressing && Time.time > downTime + holdTime)
                {
                    switch (execute)
                    {
                        case ReleaseType.OnKeyUp:
                            if (Input.GetKeyUp(keyCode))
                            {
                                isPressing = false;
                                ExecuteTrigger(gameObject);
                            }

                            break;

                        case ReleaseType.OnTimeout:
                            if (Input.GetKey(keyCode))
                            {
                                isPressing = false;
                                ExecuteTrigger(gameObject);
                            }

                            break;
                    }
                }

                if (Input.GetKeyDown(keyCode))
                {
                    isPressing = true;
                    downTime = Time.time;
                }

                if (Input.GetKeyUp(keyCode))
                {
                    isPressing = false;
                }
            }
        }

        private void OnMouseExit()
        {
            isMouseOver = false;
            isPressing = false;
        }

        private void OnMouseEnter()
        {
            isMouseOver = true;
        }
    }
}