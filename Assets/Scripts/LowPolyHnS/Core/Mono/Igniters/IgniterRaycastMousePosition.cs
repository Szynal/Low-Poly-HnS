using LowPolyHnS.Variables;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class IgniterRaycastMousePosition : Igniter, IPointerClickHandler
    {
#if UNITY_EDITOR
        public new static string NAME = "Input/On Mouse Click (World Position)";
        public new static bool REQUIRES_COLLIDER = true;
#endif

        public PointerEventData.InputButton mouseButton = PointerEventData.InputButton.Left;

        [Space, VariableFilter(Variable.DataType.Vector3)]
        public VariableProperty storeWorldPosition = new VariableProperty();

        private void Start()
        {
            EventSystemManager.Instance.Wakeup();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == mouseButton)
            {
                Vector3 clickPosition = eventData.pointerCurrentRaycast.worldPosition;
                storeWorldPosition.Set(clickPosition, gameObject);

                ExecuteTrigger(gameObject);
            }
        }
    }
}