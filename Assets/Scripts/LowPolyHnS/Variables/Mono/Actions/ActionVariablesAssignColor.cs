using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR

#endif

    [AddComponentMenu("")]
    public class ActionVariablesAssignColor : IActionVariablesAssign
    {
        [VariableFilter(Variable.DataType.Color)]
        public VariableProperty variable;

        public Color value = Color.white;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override void ExecuteAssignement(GameObject target)
        {
            variable.Set(value, target);
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Variables/Variable Color";

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, "Color", variable);
        }

        public override bool PaintInspectorTarget()
        {
            return false;
        }

#endif
    }
}