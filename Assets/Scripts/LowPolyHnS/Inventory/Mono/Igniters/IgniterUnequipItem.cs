using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
    [AddComponentMenu("")]
    public class IgniterUnequipItem : Igniter
    {
#if UNITY_EDITOR
        public new static string NAME = "Inventory/On Unequip Item";
        public new static string ICON_PATH = "Assets/Scripts/LowPolyHnS/Inventory/Icons/Igniters/";
        public const string CUSTOM_ICON_PATH = "Assets/Scripts/LowPolyHnS/Inventory/Icons/Igniters/";
#endif

        public TargetGameObject character = new TargetGameObject(TargetGameObject.Target.Player);
        public ItemHolder item = new ItemHolder();

        private new void OnEnable()
        {
            base.OnEnable();
            InventoryManager.Instance.eventOnUnequip.AddListener(OnCallback);
        }

        private void OnDisable()
        {
            if (isExitingApplication) return;
            InventoryManager.Instance.eventOnUnequip.RemoveListener(OnCallback);
        }

        private void OnCallback(GameObject target, int item)
        {
            if (target == character.GetGameObject(gameObject) &&
                this.item.item.uuid == item)
            {
                ExecuteTrigger(target);
            }
        }
    }
}