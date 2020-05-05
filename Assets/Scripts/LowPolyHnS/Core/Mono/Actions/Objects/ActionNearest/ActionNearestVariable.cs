using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class ActionNearestVariable : IActionNearest
    {
        [Space] public string variableName = "my-variable";

        protected override bool FilterCondition(GameObject item)
        {
            LocalVariables localVariables = item.GetComponent<LocalVariables>();
            if (localVariables == null) return false;

            variableName.Trim().Replace(" ", "-");
            return localVariables.Get(variableName) != null;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Object/Nearest with Local Variable";
        private const string NODE_TITLE = "Get nearest object with local[{0}]";

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, variableName);
        }

#endif
    }
}