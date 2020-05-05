using LowPolyHnS.Core;
using LowPolyHnS.Core.Hooks;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
    [AddComponentMenu("")]
    public class IgniterInventoryUI : Igniter
    {
#if UNITY_EDITOR
        public new static string NAME = "Inventory/On Inventory UI";
        public new static string ICON_PATH = "Assets/Content/Icons/Inventory/Igniters/";
        public const string CUSTOM_ICON_PATH = "Assets/Content/Icons/Inventory/Igniters/";
#endif

        public enum State
        {
            OnOpen,
            OnClose
        }

        public State detect = State.OnOpen;

        private void Start()
        {
            InventoryManager.Instance.eventInventoryUI.AddListener(OnChangeState);
        }

        private void OnChangeState(bool state)
        {
            GameObject target = HookPlayer.Instance != null
                ? HookPlayer.Instance.gameObject
                : gameObject;

            if (state && detect == State.OnOpen) ExecuteTrigger(target);
            if (state == false && detect == State.OnClose) ExecuteTrigger(target);
        }
    }
}