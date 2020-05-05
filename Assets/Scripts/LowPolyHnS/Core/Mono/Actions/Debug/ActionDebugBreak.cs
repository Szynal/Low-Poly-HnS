using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionDebugBreak : IAction
    {
        public bool isEnabled = true;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (Application.isEditor && isEnabled) Debug.Break();
            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Debug/Debug Break";
        private const string NODE_TITLE = "Debug.Break ({0})";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spIsEnabled;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, isEnabled ? "On" : "Off");
        }

        protected override void OnEnableEditorChild()
        {
            spIsEnabled = serializedObject.FindProperty("isEnabled");
        }

        protected override void OnDisableEditorChild()
        {
            spIsEnabled = null;
        }

        public override void OnInspectorGUI()
        {
            if (serializedObject == null) return;

            serializedObject.Update();

            EditorGUILayout.PropertyField(spIsEnabled);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}