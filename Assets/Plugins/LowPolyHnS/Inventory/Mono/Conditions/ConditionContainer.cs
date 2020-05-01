using LowPolyHnS.Core;
using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
    [AddComponentMenu("")]
    public class ConditionContainer : ICondition
    {
        public TargetGameObject container = new TargetGameObject(TargetGameObject.Target.Invoker);

        [Space] public ItemHolder item = new ItemHolder();
        public NumberProperty amount = new NumberProperty(1);

        public override bool Check(GameObject target)
        {
            GameObject containerGo = container.GetGameObject(target);
            if (containerGo == null) return false;

            Container containerTarget = containerGo.GetComponent<Container>();
            if (containerTarget == null) return false;
            if (item.item == null) return false;

            int containerAmount = containerTarget.GetAmount(item.item.uuid);
            return containerAmount >= amount.GetInt(target);
        }

#if UNITY_EDITOR

        public const string CUSTOM_ICON_PATH = "Assets/Plugins/LowPolyHnS/Inventory/Icons/Conditions/";

        public static new string NAME = "Inventory/Container Has Item";
        private const string NODE_TITLE = "Container {0} has {1}";

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, container, item);
        }

#endif
    }
}