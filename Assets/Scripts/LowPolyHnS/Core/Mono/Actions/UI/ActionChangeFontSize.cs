using LowPolyHnS.Variables;
using UnityEngine;
using UnityEngine.UI;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionChangeFontSize : IAction
    {
        public Text text;
        public NumberProperty size = new NumberProperty(16f);

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (text != null)
            {
                text.fontSize = size.GetInt(target);
            }

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "UI/Change Font Size";
        private const string NODE_TITLE = "Change text font size";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spText;
        private SerializedProperty spSize;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return NODE_TITLE;
        }

        protected override void OnEnableEditorChild()
        {
            spText = serializedObject.FindProperty("text");
            spSize = serializedObject.FindProperty("size");
        }

        protected override void OnDisableEditorChild()
        {
            spText = null;
            spSize = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spText);
            EditorGUILayout.PropertyField(spSize);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}