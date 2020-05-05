using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
    [AddComponentMenu("")]
    public class ConditionInventoryUI : ICondition
    {
        public enum State
        {
            IsOpen,
            IsClosed
        }

        public State inventory = State.IsOpen;

        public override bool Check(GameObject target)
        {
            switch (inventory)
            {
                case State.IsOpen:
                    return InventoryUIManager.IsInventoryOpen();

                case State.IsClosed:
                    return !InventoryUIManager.IsInventoryOpen();
            }

            return false;
        }

#if UNITY_EDITOR

        public static new string NAME = "Inventory/Inventory UI";
        public const string CUSTOM_ICON_PATH = "Assets/Content/Icons/Inventory/Conditions/";

        private const string NODE_TITLE = "Inventory UI {0}";

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, inventory);
        }

#endif
    }
}