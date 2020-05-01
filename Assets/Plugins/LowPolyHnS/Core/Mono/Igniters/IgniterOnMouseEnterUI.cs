using UnityEngine;
using UnityEngine.EventSystems;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class IgniterOnMouseEnterUI : Igniter, IPointerEnterHandler
    {
#if UNITY_EDITOR
        public new static string NAME = "UI/On Mouse Enter UI";
#endif

        private void Start()
        {
            EventSystemManager.Instance.Wakeup();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            ExecuteTrigger(gameObject);
        }
    }
}