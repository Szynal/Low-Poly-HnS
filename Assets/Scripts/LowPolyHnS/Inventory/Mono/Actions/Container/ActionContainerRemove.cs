using LowPolyHnS.Core;
using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
    [AddComponentMenu("")]
    public class ActionContainerRemove : IActionContainer
    {
        [Space] public ItemHolder item = new ItemHolder();
        public NumberProperty amount = new NumberProperty(1f);

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Container cont = GetContainer(target);
            if (cont == null) return true;

            if (item.item == null) return true;
            cont.RemoveItem(item.item.uuid, amount.GetInt(target));

            return true;
        }

#if UNITY_EDITOR
        public const string CUSTOM_ICON_PATH = "Assets/Content/Icons/Inventory/Actions/";
        public static new string NAME = "Inventory/Container/Remove Item Container";

        private const string NODE_TITLE = "Remove {0} from {1}";

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, item, container);
        }

#endif
    }
}