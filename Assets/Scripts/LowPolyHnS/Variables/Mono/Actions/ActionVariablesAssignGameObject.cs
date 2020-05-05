using LowPolyHnS.Core.Hooks;
using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR

#endif

    [AddComponentMenu("")]
    public class ActionVariablesAssignGameObject : IActionVariablesAssign
    {
        [VariableFilter(Variable.DataType.GameObject)]
        public VariableProperty variable;

        public GameObject value;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override void ExecuteAssignement(GameObject target)
        {
            switch (valueFrom)
            {
                case ValueFrom.Invoker:
                    variable.Set(target);
                    break;

                case ValueFrom.Player:
                    variable.Set(HookPlayer.Instance.gameObject);
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

        public static new string NAME = "Variables/Variable GameObject";

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, "GameObject", variable);
        }

        public override bool PaintInspectorTarget()
        {
            return true;
        }

#endif
    }
}