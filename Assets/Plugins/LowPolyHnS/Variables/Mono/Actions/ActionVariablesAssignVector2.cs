using LowPolyHnS.Core.Hooks;
using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR

#endif

    [AddComponentMenu("")]
    public class ActionVariablesAssignVector2 : IActionVariablesAssign
    {
        [VariableFilter(Variable.DataType.Vector2)]
        public VariableProperty variable;

        public Vector2 value = Vector2.zero;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override void ExecuteAssignement(GameObject target)
        {
            switch (valueFrom)
            {
                case ValueFrom.Invoker:
                    variable.Set(GetVector2(target.transform.position), target);
                    break;
                case ValueFrom.Player:
                    variable.Set(GetVector2(HookPlayer.Instance.transform.position), target);
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

        private Vector2 GetVector2(Vector3 vector)
        {
            return new Vector2(vector.x, vector.z);
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Variables/Variable Vector2";

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, "Vector2", variable);
        }

        public override bool PaintInspectorTarget()
        {
            return true;
        }

#endif
    }
}