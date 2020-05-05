using LowPolyHnS.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TextEditor = UnityEditor.UI.TextEditor;

namespace LowPolyHnS.Localization
{
    [CustomEditor(typeof(TextLocalized), true)]
    [CanEditMultipleObjects]
    public class TextLocalizedEditor : TextEditor
    {
        private SerializedProperty spText;
        private SerializedProperty spLocString;
        private SerializedProperty spLocStringContent;

        protected override void OnEnable()
        {
            base.OnEnable();
            spLocString = serializedObject.FindProperty("locString");
            spLocStringContent = spLocString.FindPropertyRelative("content");

            spText = serializedObject.FindProperty("m_Text");

            if (!Application.isPlaying)
            {
                spText.stringValue = spLocStringContent.stringValue;
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spLocString);
            base.OnInspectorGUI();

            spLocStringContent.stringValue = spText.stringValue;
            serializedObject.ApplyModifiedProperties();
        }

        // STATIC METHODS: ------------------------------------------------------------------------

        [MenuItem("GameObject/LowPolyHnS/UI/Text (Localized)", false, 20)]
        public static void CreateTextLocalized()
        {
            GameObject canvas = CreateSceneObject.GetCanvasGameObject();
            GameObject textGO = DefaultControls.CreateText(CreateSceneObject.GetStandardResources());
            textGO.transform.SetParent(canvas.transform, false);

            DestroyImmediate(textGO.GetComponent<Text>());
            textGO.AddComponent<TextLocalized>();
            Selection.activeGameObject = textGO;
        }
    }
}