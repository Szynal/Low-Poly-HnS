using LowPolyHnS.Core;
using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
    [AddComponentMenu("")]
    public class ActionContainerAdd : IActionContainer
    {
        [Space] public ItemHolder item = new ItemHolder();
        public NumberProperty amount = new NumberProperty(1f);

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Container cont = GetContainer(target);
            if (cont == null) return true;

            if (item.item == null) return true;
            cont.AddItem(item.item.uuid, amount.GetInt(target));

            return true;
        }

#if UNITY_EDITOR
        public const string CUSTOM_ICON_PATH = "Assets/Scripts/LowPolyHnS/Inventory/Icons/Actions/";
        public static new string NAME = "Inventory/Container/Add Item Container";

        private const string NODE_TITLE = "Add {0} to {1}";

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, item, container);
        }

#endif
    }
}