using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
    [AddComponentMenu("")]
    public class ActionHotbarUnbind : IAction
    {
        public TargetGameObject hotbar = new TargetGameObject(TargetGameObject.Target.GameObject);

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            GameObject targetHotbar = hotbar.GetGameObject(target);
            if (target == null) return true;

            HotbarUI hotbarUI = targetHotbar.GetComponent<HotbarUI>();
            if (hotbarUI == null) return true;

            hotbarUI.UnbindItem();
            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public const string CUSTOM_ICON_PATH = "Assets/Scripts/LowPolyHnS/Inventory/Icons/Actions/";

        public static new string NAME = "Inventory/Unbind Item from Hotbar";
        private const string NODE_TITLE = "Unbind item from {0}";

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                hotbar
            );
        }

#endif
    }
}