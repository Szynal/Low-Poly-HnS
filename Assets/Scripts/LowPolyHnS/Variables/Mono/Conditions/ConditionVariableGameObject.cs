using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR

#endif

    [AddComponentMenu("")]
    public class ConditionVariableGameObject : ConditionVariable
    {
        [VariableFilter(Variable.DataType.GameObject)]
        public VariableProperty variable = new VariableProperty();

        public TargetGameObject compareTo = new TargetGameObject();

        // OVERRIDERS: ----------------------------------------------------------------------------

        protected override bool Compare(GameObject target)
        {
            return (GameObject) variable.Get(target) == compareTo.GetGameObject(target);
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Variables/Variable GameObject";

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