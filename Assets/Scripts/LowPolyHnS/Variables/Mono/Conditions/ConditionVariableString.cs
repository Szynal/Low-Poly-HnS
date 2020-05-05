using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR

#endif

    [AddComponentMenu("")]
    public class ConditionVariableString : ConditionVariable
    {
        [VariableFilter(Variable.DataType.String)]
        public VariableProperty variable = new VariableProperty();

        public StringProperty compareTo = new StringProperty();

        // OVERRIDERS: ----------------------------------------------------------------------------

        protected override bool Compare(GameObject target)
        {
            return (string) variable.Get(target) == compareTo.GetValue(target);
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Variables/Variable String";

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