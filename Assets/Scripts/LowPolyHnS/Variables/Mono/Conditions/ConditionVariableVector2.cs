using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR

#endif

    [AddComponentMenu("")]
    public class ConditionVariableVector2 : ConditionVariable
    {
        [VariableFilter(Variable.DataType.Vector2)]
        public VariableProperty variable = new VariableProperty();

        public Vector2Property compareTo = new Vector2Property();

        // OVERRIDERS: ----------------------------------------------------------------------------

        protected override bool Compare(GameObject target)
        {
            return (Vector2) variable.Get(target) == compareTo.GetValue(target);
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Variables/Variable Vector2";

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                variable,
                compareTo
            );
        }

#endif
    }
}