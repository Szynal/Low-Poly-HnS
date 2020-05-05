using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR

#endif

    [AddComponentMenu("")]
    public class ConditionVariableSprite : ConditionVariable
    {
        [VariableFilter(Variable.DataType.Sprite)]
        public VariableProperty variable = new VariableProperty();

        public SpriteProperty compareTo = new SpriteProperty();

        // OVERRIDERS: ----------------------------------------------------------------------------

        protected override bool Compare(GameObject target)
        {
            return (Sprite) variable.Get(target) == compareTo.GetValue(target);
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Variables/Variable Sprite";

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