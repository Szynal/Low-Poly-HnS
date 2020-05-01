using LowPolyHnS.Localization;
using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionChangeLanguage : IAction
    {
        public SystemLanguage language;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            LocalizationManager.Instance.ChangeLanguage(language);
            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "General/Change Language";
        private const string NODE_TITLE = "Set Language to {0}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spLanguage;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, language);
        }

        protected override void OnEnableEditorChild()
        {
            spLanguage = serializedObject.FindProperty("language");
        }

        protected override void OnDisableEditorChild()
        {
            spLanguage = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spLanguage);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}