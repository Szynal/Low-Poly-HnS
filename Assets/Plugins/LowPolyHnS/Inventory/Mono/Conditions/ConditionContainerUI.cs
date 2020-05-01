using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
    [AddComponentMenu("")]
    public class ConditionContainerUI : ICondition
    {
        public enum State
        {
            IsOpen,
            IsClosed
        }

        public State container = State.IsOpen;

        public override bool Check(GameObject target)
        {
            switch (container)
            {
                case State.IsOpen:
                    return ContainerUIManager.IsContainerOpen();

                case State.IsClosed:
                    return !ContainerUIManager.IsContainerOpen();
            }

            return false;
        }

#if UNITY_EDITOR

        public static new string NAME = "Inventory/Container UI";
        public const string CUSTOM_ICON_PATH = "Assets/Plugins/LowPolyHnS/Inventory/Icons/Conditions/";

        private const string NODE_TITLE = "Container UI {0}";

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, container);
        }

#endif
    }
}