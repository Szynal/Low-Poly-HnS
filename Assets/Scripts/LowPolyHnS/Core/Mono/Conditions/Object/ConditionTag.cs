using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR

#endif

    [AddComponentMenu("")]
    public class ConditionTag : ICondition
    {
        public TargetGameObject targetGameObject = new TargetGameObject();

        [TagSelector] public string conditionTag = "";

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool Check(GameObject target)
        {
            GameObject result = targetGameObject.GetGameObject(target);
            if (result == null) return false;

            return result.CompareTag(conditionTag);
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Object/Tag";
        private const string NODE_TITLE = "Has {0} tag {1}";

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                targetGameObject,
                string.IsNullOrEmpty(conditionTag) ? "none" : conditionTag
            );
        }

#endif
    }
}