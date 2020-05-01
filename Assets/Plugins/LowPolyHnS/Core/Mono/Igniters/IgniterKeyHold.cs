using UnityEngine;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class IgniterKeyHold : Igniter
    {
        public enum ReleaseType
        {
            OnKeyUp,
            OnTimeout
        }

        public KeyCode keyCode = KeyCode.Space;
        public float holdTime = 0.5f;
        public ReleaseType execute = ReleaseType.OnKeyUp;

        private float downTime = -9999.0f;
        private bool isPressing;

#if UNITY_EDITOR
        public new static string NAME = "Input/On Key Hold";
#endif

        private void Update()
        {
            if (isPressing && Time.time - downTime > holdTime)
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
                downTime = Time.time;
                isPressing = true;
            }

            if (Input.GetKeyUp(keyCode))
            {
                isPressing = false;
            }
        }
    }
}