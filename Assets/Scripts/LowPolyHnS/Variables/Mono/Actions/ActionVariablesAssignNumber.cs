using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR

#endif

    [AddComponentMenu("")]
    public class ActionVariablesAssignNumber : IActionVariablesAssign
    {
        [VariableFilter(Variable.DataType.Number)]
        public VariableProperty variable;

        public float value = 1.0f;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override void ExecuteAssignement(GameObject target)
        {
            if (variable == null) return;
            variable.Set(value, target);
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Variables/Variable Number";

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, "Number", variable);
        }

        public override bool PaintInspectorTarget()
        {
            return false;
        }

#endif
    }
}