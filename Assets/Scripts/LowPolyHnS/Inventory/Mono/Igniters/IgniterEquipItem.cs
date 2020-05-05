using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
    [AddComponentMenu("")]
    public class IgniterEquipItem : Igniter
    {
#if UNITY_EDITOR
        public new static string NAME = "Inventory/On Equip Item";
        public new static string ICON_PATH = "Assets/Content/Icons/Inventory/Igniters/";
        public const string CUSTOM_ICON_PATH = "Assets/Content/Icons/Inventory/Igniters/";
#endif

        public TargetGameObject character = new TargetGameObject(TargetGameObject.Target.Player);
        public ItemHolder item = new ItemHolder();

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
            if (target == character.GetGameObject(gameObject) &&
                this.item.item.uuid == item)
            {
                ExecuteTrigger(target);
            }
        }
    }
}