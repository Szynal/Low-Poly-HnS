using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
    [AddComponentMenu("")]
    public class IgniterContainerUI : Igniter
    {
#if UNITY_EDITOR
        public new static string NAME = "Inventory/On Container UI";
        public new static string ICON_PATH = "Assets/Scripts/LowPolyHnS/Inventory/Icons/Igniters/";
        public const string CUSTOM_ICON_PATH = "Assets/Scripts/LowPolyHnS/Inventory/Icons/Igniters/";
#endif

        public enum State
        {
            OnOpen,
            OnClose
        }

        public State detect = State.OnOpen;

        private void Start()
        {
            InventoryManager.Instance.eventContainerUI.AddListener(OnChangeState);
        }

        private void OnChangeState(bool state, GameObject container)
        {
            if (state && detect == State.OnOpen) ExecuteTrigger(container);
            if (state == false && detect == State.OnClose) ExecuteTrigger(container);
        }
    }
}