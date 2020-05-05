using System.Collections.Generic;
using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
    [AddComponentMenu("")]
    public class ActionContainerRetrieve : IActionContainer
    {
        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Container cont = GetContainer(target);
            if (cont == null) return true;

            List<Container.ItemData> items = cont.GetItems();
            for (int i = 0; i < items.Count; ++i)
            {
                Container.ItemData item = items[i];

                InventoryManager.Instance.AddItemToInventory(item.uuid, item.amount);
                cont.RemoveItem(item.uuid, item.amount);
            }

            return true;
        }

#if UNITY_EDITOR
        public const string CUSTOM_ICON_PATH = "Assets/Scripts/LowPolyHnS/Inventory/Icons/Actions/";
        public static new string NAME = "Inventory/Container/Retrieve all in Container";

        private const string NODE_TITLE = "Retrieve everything from {0}";

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, container);
        }

#endif
    }
}