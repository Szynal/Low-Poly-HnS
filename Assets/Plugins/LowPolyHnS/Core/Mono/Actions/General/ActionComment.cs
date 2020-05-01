using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionComment : IAction
    {
        [Multiline(5)] public string comment = "";

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "General/Comment";
        private const string NODE_TITLE = "// {0}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spComment;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, comment.Replace("\n", string.Empty));
        }

        protected override void OnEnableEditorChild()
        {
            spComment = serializedObject.FindProperty("comment");
        }

        protected override void OnDisableEditorChild()
        {
            spComment = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spComment);

            serializedObject.ApplyModifiedProperties();
        }

        public override float GetOpacity()
        {
            return 0.5f;
        }

#endif
    }
}