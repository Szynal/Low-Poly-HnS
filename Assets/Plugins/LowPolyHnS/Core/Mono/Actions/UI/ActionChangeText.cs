using LowPolyHnS.Variables;
using UnityEngine;
using UnityEngine.UI;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionChangeText : IAction
    {
        public Text text;
        public string content = "{0}";
        public VariableProperty variable = new VariableProperty();

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (text != null)
            {
                text.text = string.Format(
                    content,
                    new[] {variable.ToStringValue(target)}
                );
            }

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        private static readonly GUIContent GUICONTENT_VARIABLE = new GUIContent("{0} Variable");

        public static new string NAME = "UI/Change Text";
        private const string NODE_TITLE = "Change text to {0}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spText;
        private SerializedProperty spContent;
        private SerializedProperty spVariable;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, content);
        }

        protected override void OnEnableEditorChild()
        {
            spText = serializedObject.FindProperty("text");
            spContent = serializedObject.FindProperty("content");
            spVariable = serializedObject.FindProperty("variable");
        }

        protected override void OnDisableEditorChild()
        {
            spText = null;
            spContent = null;
            spVariable = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spText);
            EditorGUILayout.PropertyField(spContent);
            EditorGUILayout.PropertyField(spVariable, GUICONTENT_VARIABLE);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}