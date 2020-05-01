using LowPolyHnS.Core.Hooks;
using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR

#endif

    [AddComponentMenu("")]
    public class ActionVariablesAssignVector3 : IActionVariablesAssign
    {
        [VariableFilter(Variable.DataType.Vector3)]
        public VariableProperty variable;

        public Vector3 value = Vector3.zero;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override void ExecuteAssignement(GameObject target)
        {
            switch (valueFrom)
            {
                case ValueFrom.Invoker:
                    variable.Set(target.transform.position, target);
                    break;
                case ValueFrom.Player:
                    variable.Set(HookPlayer.Instance.transform.position, target);
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

        public static new string NAME = "Variables/Variable Vector3";

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, "Vector3", variable);
        }

        public override bool PaintInspectorTarget()
        {
            return true;
        }

#endif
    }
}