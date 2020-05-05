using LowPolyHnS.Core;
using LowPolyHnS.Core.Hooks;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
    [AddComponentMenu("")]
    public class IgniterCurrency : Igniter
    {
#if UNITY_EDITOR
        public new static string NAME = "Inventory/On Currency Change";
        public new static string ICON_PATH = "Assets/Content/Icons/Inventory/Igniters/";
        public const string CUSTOM_ICON_PATH = "Assets/Content/Icons/Inventory/Igniters/";
#endif

        private void Start()
        {
            InventoryManager.Instance.eventChangePlayerCurrency.AddListener(OnCurrencyChange);
        }

        private void OnCurrencyChange()
        {
            GameObject invoker = HookPlayer.Instance != null
                ? HookPlayer.Instance.gameObject
                : gameObject;

            ExecuteTrigger(invoker);
        }
    }
}