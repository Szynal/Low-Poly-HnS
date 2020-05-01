using LowPolyHnS.Core;
using LowPolyHnS.Core.Hooks;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
#if UNITY_EDITOR

#endif

    [AddComponentMenu("")]
    public class ActionInventoryDropItem : IAction
    {
        public ItemHolder itemHolder;
        public float distance = 1.0f;

        // EXECUTABLE: -------------------------------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Vector3 position = HookPlayer.Instance.transform.TransformPoint(Vector3.forward * distance);

            Instantiate(itemHolder.item.prefab, position, Quaternion.identity);
            InventoryManager.Instance.SubstractItemFromInventory(itemHolder.item.uuid);

            return true;
        }

        // +-----------------------------------------------------------------------------------------------------------+
        // | EDITOR                                                                                                    |
        // +-----------------------------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public const string CUSTOM_ICON_PATH = "Assets/Plugins/LowPolyHnS/Inventory/Icons/Actions/";

        public static new string NAME = "Inventory/Drop Item";
        private const string NODE_TITLE = "Drop {0} before player";

        // INSPECTOR METHODS: ------------------------------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                itemHolder.item == null ? "nothing" : itemHolder.item.itemName.content
            );
        }

#endif
    }
}