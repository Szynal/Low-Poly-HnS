using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionOpenURL : IAction
    {
        public StringProperty link = new StringProperty("http://...");

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Application.OpenURL(link.GetValue(target));
            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Application/Open URL";
        private const string NODE_TITLE = "Open URL: {0}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spLink;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, link);
        }

        protected override void OnEnableEditorChild()
        {
            spLink = serializedObject.FindProperty("link");
        }

        protected override void OnDisableEditorChild()
        {
            spLink = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spLink);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}