using UnityEngine;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class ActionNearestComponent : IActionNearest
    {
        [Space] public string componentName = "Light";

        protected override bool FilterCondition(GameObject item)
        {
            return item.GetComponent(componentName) != null;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Object/Nearest with Component";
        private const string NODE_TITLE = "Get nearest object with component {0}";

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, componentName);
        }

#endif
    }
}