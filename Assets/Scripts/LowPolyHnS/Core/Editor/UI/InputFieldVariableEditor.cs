using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

namespace LowPolyHnS.Core
{
    [CustomEditor(typeof(InputFieldVariable))]
    public class InputFieldVariableEditor : InputFieldEditor
    {
        private SerializedProperty spVariable;

        private new void OnEnable()
        {
            base.OnEnable();
            spVariable = serializedObject.FindProperty("variable");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            serializedObject.Update();
            EditorGUILayout.PropertyField(spVariable);
            serializedObject.ApplyModifiedProperties();
        }

        // CREATE: --------------------------------------------------------------------------------

        [MenuItem("GameObject/LowPolyHnS/UI/Input Field", false, 10)]
        public static void CreateInputFieldVariable()
        {
            GameObject canvas = CreateSceneObject.GetCanvasGameObject();
            GameObject inputGO = DefaultControls.CreateInputField(CreateSceneObject.GetStandardResources());
            inputGO.transform.SetParent(canvas.transform, false);

            InputField input = inputGO.GetComponent<InputField>();
            Graphic targetGraphic = input.targetGraphic;
            Graphic placeholderComponent = input.placeholder;
            Text textComponent = input.textComponent;

            DestroyImmediate(input);
            input = inputGO.AddComponent<InputFieldVariable>();
            input.targetGraphic = targetGraphic;
            input.placeholder = placeholderComponent;
            input.textComponent = textComponent;
            Selection.activeGameObject = inputGO;
        }
    }
}