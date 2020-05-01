using System.Collections;
using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionSaveGame : IAction
    {
        public bool useCurrentProfile = true;
        public int selectProfile = 1;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
        {
            int profile = useCurrentProfile
                ? SaveLoadManager.Instance.GetCurrentProfile()
                : selectProfile;

            SaveLoadManager.Instance.Save(profile);

            yield return null;
            yield return 0;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Save & Load/Save Game";
        private const string NODE_TITLE = "Save Game (profile {0})";

        private static readonly GUIContent GUICONTENT_USECURR_PROFILE = new GUIContent("Use Current Profile?");
        private static readonly GUIContent GUICONTENT_SELECT_PROFILE = new GUIContent("Select Profile");

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spUseCurrentProfile;
        private SerializedProperty spSelectProfile;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                useCurrentProfile ? "current" : selectProfile.ToString()
            );
        }

        protected override void OnEnableEditorChild()
        {
            spUseCurrentProfile = serializedObject.FindProperty("useCurrentProfile");
            spSelectProfile = serializedObject.FindProperty("selectProfile");
        }

        protected override void OnDisableEditorChild()
        {
            spUseCurrentProfile = null;
            spSelectProfile = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spUseCurrentProfile, GUICONTENT_USECURR_PROFILE);
            if (!spUseCurrentProfile.boolValue)
            {
                EditorGUILayout.PropertyField(spSelectProfile, GUICONTENT_SELECT_PROFILE);
            }

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}