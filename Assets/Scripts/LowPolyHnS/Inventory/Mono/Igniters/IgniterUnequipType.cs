using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
    [AddComponentMenu("")]
    public class IgniterUnequipType : Igniter
    {
#if UNITY_EDITOR
        public new static string NAME = "Inventory/On Unequip Type";
        public new static string ICON_PATH = "Assets/Content/Icons/Inventory/Igniters/";
        public const string CUSTOM_ICON_PATH = "Assets/Content/Icons/Inventory/Igniters/";
#endif

        public TargetGameObject character = new TargetGameObject(TargetGameObject.Target.Player);

        [InventoryMultiItemType] public int itemTypes = -1;

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
                (InventoryManager.Instance.itemsCatalogue[item].itemTypes & itemTypes) > 0)
            {
                ExecuteTrigger(target);
            }
        }
    }
}