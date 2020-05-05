using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
    [AddComponentMenu("")]
    public class ActionHotbarBind : IAction
    {
        public TargetGameObject hotbar = new TargetGameObject(TargetGameObject.Target.GameObject);
        public ItemHolder item = new ItemHolder();

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (item.item == null) return true;

            GameObject targetHotbar = hotbar.GetGameObject(target);
            if (target == null) return true;

            HotbarUI hotbarUI = targetHotbar.GetComponent<HotbarUI>();
            if (hotbarUI == null) return true;

            hotbarUI.BindItem(item.item);
            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public const string CUSTOM_ICON_PATH = "Assets/Scripts/LowPolyHnS/Inventory/Icons/Actions/";

        public static new string NAME = "Inventory/Bind Item to Hotbar";
        private const string NODE_TITLE = "Bind {0} on {1}";

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                item.item == null ? "(none)" : item.item.itemName.content,
                hotbar
            );
        }

#endif
    }
}