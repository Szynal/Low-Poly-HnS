using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
    [AddComponentMenu("")]
    public class IgniterEquip : Igniter
    {
#if UNITY_EDITOR
        public new static string NAME = "Inventory/On Equip Any";
        public new static string ICON_PATH = "Assets/Scripts/LowPolyHnS/Inventory/Icons/Igniters/";
        public const string CUSTOM_ICON_PATH = "Assets/Scripts/LowPolyHnS/Inventory/Icons/Igniters/";
#endif

        public TargetGameObject character = new TargetGameObject(TargetGameObject.Target.Player);

        private new void OnEnable()
        {
            base.OnEnable();
            InventoryManager.Instance.eventOnEquip.AddListener(OnCallback);
        }

        private void OnDisable()
        {
            if (isExitingApplication) return;
            InventoryManager.Instance.eventOnEquip.RemoveListener(OnCallback);
        }

        private void OnCallback(GameObject target, int item)
        {
            if (target == character.GetGameObject(gameObject))
            {
                ExecuteTrigger(target);
            }
        }
    }
}