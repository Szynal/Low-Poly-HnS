using UnityEngine;
using UnityEngine.EventSystems;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class IgniterOnMouseExitUI : Igniter, IPointerExitHandler
    {
#if UNITY_EDITOR
        public new static string NAME = "UI/On Mouse Exit UI";
#endif

        private void Start()
        {
            EventSystemManager.Instance.Wakeup();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ExecuteTrigger(gameObject);
        }
    }
}