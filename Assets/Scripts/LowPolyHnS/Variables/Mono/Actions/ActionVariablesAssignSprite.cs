using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR

#endif

    [AddComponentMenu("")]
    public class ActionVariablesAssignSprite : IActionVariablesAssign
    {
        [VariableFilter(Variable.DataType.Sprite)]
        public VariableProperty variable;

        public Sprite value = null;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override void ExecuteAssignement(GameObject target)
        {
            variable.Set(value, target);
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Variables/Variable Sprite";

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, "Sprite", variable);
        }

        public override bool PaintInspectorTarget()
        {
            return false;
        }

#endif
    }
}