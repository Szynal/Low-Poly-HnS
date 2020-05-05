using UnityEngine;
using UnityEngine.EventSystems;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class IgniterMouseMiddleClick : Igniter, IPointerClickHandler
    {
        public enum ClickType
        {
            SingleClick,
            DoubleClick
        }

#if UNITY_EDITOR
        public new static string NAME = "Input/On Mouse Middle Click";
        public new static bool REQUIRES_COLLIDER = true;
#endif

        public ClickType clickType = ClickType.SingleClick;
        private float lastClickTime = -100f;

        private void Start()
        {
            EventSystemManager.Instance.Wakeup();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Middle)
            {
                switch (clickType)
                {
                    case ClickType.SingleClick:
                        ExecuteTrigger(gameObject);
                        break;
                    case ClickType.DoubleClick:
                        if (Time.time - lastClickTime < 0.5f) ExecuteTrigger(gameObject);
                        else lastClickTime = Time.time;
                        break;
                }
            }
        }
    }
}