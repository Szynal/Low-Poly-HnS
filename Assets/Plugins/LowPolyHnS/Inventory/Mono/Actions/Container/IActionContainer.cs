using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
    [AddComponentMenu("")]
    public abstract class IActionContainer : IAction
    {
        public TargetGameObject container = new TargetGameObject(TargetGameObject.Target.Invoker);

        protected Container GetContainer(GameObject target)
        {
            GameObject containerGo = container.GetGameObject(target);
            if (containerGo == null) return null;

            return containerGo.GetComponent<Container>();
        }
    }
}