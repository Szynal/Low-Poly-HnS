using UnityEngine;
using UnityEngine.EventSystems;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class IgniterMouseHoldMiddle : Igniter, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
#if UNITY_EDITOR
        public new static string NAME = "Input/On Mouse Middle Hold";
        public new static bool REQUIRES_COLLIDER = true;
#endif

        private const PointerEventData.InputButton MOUSE_BUTTON = PointerEventData.InputButton.Middle;

        public enum ReleaseType
        {
            OnMouseUp,
            OnTimeout
        }

        public float holdTime = 0.5f;
        public ReleaseType execute = ReleaseType.OnMouseUp;

        private float downTime = -999.0f;
        private bool isPressing;

        private void Start()
        {
            EventSystemManager.Instance.Wakeup();
        }

        private void Update()
        {
            if (execute == ReleaseType.OnTimeout && isPressing)
            {
                if (Input.GetMouseButton((int) MOUSE_BUTTON) &&
                    Time.time - downTime > holdTime)
                {
                    isPressing = false;
                    ExecuteTrigger(gameObject);
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                isPressing = true;
                downTime = Time.time;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isPressing = false;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != MOUSE_BUTTON) return;
            if (!isPressing) return;

            isPressing = false;

            if (execute == ReleaseType.OnMouseUp && Time.time - downTime > holdTime)
            {
                ExecuteTrigger(gameObject);
            }
        }
    }
}