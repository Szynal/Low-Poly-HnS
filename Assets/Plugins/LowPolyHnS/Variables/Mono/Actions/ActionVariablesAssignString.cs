using LowPolyHnS.Core.Hooks;
using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR

#endif

    [AddComponentMenu("")]
    public class ActionVariablesAssignString : IActionVariablesAssign
    {
        [VariableFilter(Variable.DataType.String)]
        public VariableProperty variable;

        public string value = "";

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override void ExecuteAssignement(GameObject target)
        {
            switch (valueFrom)
            {
                case ValueFrom.Invoker:
                    variable.Set(target.name, target);
                    break;
                case ValueFrom.Player:
                    variable.Set(HookPlayer.Instance.gameObject.name, target);
                    break;
                case ValueFrom.Constant:
                    variable.Set(value, target);
                    break;
                case ValueFrom.GlobalVariable:
                    variable.Set(global.Get(target), target);
                    break;
                case ValueFrom.LocalVariable:
                    variable.Set(local.Get(target), target);
                    break;
                case ValueFrom.ListVariable:
                    variable.Set(list.Get(target), target);
                    break;
            }
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Variables/Variable String";

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, "String", variable);
        }

        public override bool PaintInspectorTarget()
        {
            return true;
        }

#endif
    }
}