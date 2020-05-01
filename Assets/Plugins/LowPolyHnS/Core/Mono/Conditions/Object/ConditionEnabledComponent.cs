using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ConditionEnabledComponent : ICondition
    {
        public enum CheckStatus
        {
            IsEnabled,
            IsDisabled
        }

        public Behaviour component;
        public CheckStatus state = CheckStatus.IsEnabled;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool Check(GameObject target)
        {
            if (!component) return false;

            if (component.isActiveAndEnabled && state == CheckStatus.IsEnabled) return true;
            if (!component.isActiveAndEnabled && state == CheckStatus.IsDisabled) return true;
            return false;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Object/Component Enabled";
        private const string NODE_TITLE = "Component {0} {1}";

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                component ? component.name : "(none)",
                ObjectNames.NicifyVariableName(state.ToString())
            );
        }

#endif
    }
}