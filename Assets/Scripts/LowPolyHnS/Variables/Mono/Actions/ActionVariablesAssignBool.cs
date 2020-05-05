using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR

#endif

    [AddComponentMenu("")]
    public class ActionVariablesAssignBool : IActionVariablesAssign
    {
        [VariableFilter(Variable.DataType.Bool)]
        public VariableProperty variable;

        public bool value = true;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override void ExecuteAssignement(GameObject target)
        {
            variable.Set(value, target);
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Variables/Variable Bool";

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, "Bool", variable);
        }

        public override bool PaintInspectorTarget()
        {
            return false;
        }

#endif
    }
}