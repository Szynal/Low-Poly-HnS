using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionCurrentProfile : IAction
    {
        public int profile = 0;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            SaveLoadManager.Instance.SetCurrentProfile(profile);
            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Save & Load/Current Profile";
        private const string NODE_TITLE = "Set Current profile to {0}";

        private static readonly GUIContent GUICONTENT_PROFILE = new GUIContent("Profile");

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spProfile;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, profile.ToString());
        }

        protected override void OnEnableEditorChild()
        {
            spProfile = serializedObject.FindProperty("profile");
        }

        protected override void OnDisableEditorChild()
        {
            spProfile = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spProfile, GUICONTENT_PROFILE);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}