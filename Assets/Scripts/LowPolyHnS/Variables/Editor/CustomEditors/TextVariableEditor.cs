using LowPolyHnS.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TextEditor = UnityEditor.UI.TextEditor;

namespace LowPolyHnS.Variables
{
    [CustomEditor(typeof(TextVariable), true)]
    [CanEditMultipleObjects]
    public class TextVariableEditor : TextEditor
    {
        private SerializedProperty spText;
        private SerializedProperty spFormat;
        private SerializedProperty spVariable;

        protected override void OnEnable()
        {
            base.OnEnable();

            spFormat = serializedObject.FindProperty("format");
            spVariable = serializedObject.FindProperty("variable");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spFormat);
            EditorGUILayout.PropertyField(spVariable);

            serializedObject.ApplyModifiedProperties();

            serializedObject.Update();
            base.OnInspectorGUI();
            serializedObject.ApplyModifiedProperties();
        }

        // STATIC METHODS: ------------------------------------------------------------------------

        [MenuItem("GameObject/LowPolyHnS/UI/Text (Variable)", false, 20)]
        public static void CreateTextVariable()
        {
            GameObject canvas = CreateSceneObject.GetCanvasGameObject();
            GameObject textGO = DefaultControls.CreateText(CreateSceneObject.GetStandardResources());
            textGO.transform.SetParent(canvas.transform, false);

            DestroyImmediate(textGO.GetComponent<Text>());
            textGO.AddComponent<TextVariable>();
            Selection.activeGameObject = textGO;
        }
    }
}