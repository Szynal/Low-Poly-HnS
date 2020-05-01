using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Variables
{
#if UNITY_EDITOR

#endif

    [AddComponentMenu("")]
    public class ActionVariableSubtract : ActionVariableOperationBase
    {
        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            float current = (float) (variable.Get(target) ?? 0f);
            variable.Set(current - value, target);

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Variables/Variable Subtract";
        private const string NODE_TITLE = "{0} - {1}";

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, variable, value);
        }

#endif
    }
}