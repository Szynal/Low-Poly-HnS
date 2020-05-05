using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

namespace LowPolyHnS.Core
{
    [CustomEditor(typeof(ButtonActions))]
    public class ButtonActionsEditor : SelectableEditor
    {
        private ActionsEditor editorActions;

        protected override void OnEnable()
        {
            base.OnEnable();

            SerializedProperty spActions = serializedObject.FindProperty("actions");
            if (spActions.objectReferenceValue != null)
            {
                editorActions = CreateEditor(
                    spActions.objectReferenceValue
                ) as ActionsEditor;
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            serializedObject.Update();
            if (editorActions != null)
            {
                editorActions.OnInspectorGUI();
            }

            serializedObject.ApplyModifiedProperties();
        }

        // CREATE: --------------------------------------------------------------------------------

        [MenuItem("GameObject/LowPolyHnS/UI/Button", false, 30)]
        public static void CreateButtonActions()
        {
            GameObject canvas = CreateSceneObject.GetCanvasGameObject();
            GameObject buttonGO = DefaultControls.CreateButton(CreateSceneObject.GetStandardResources());
            buttonGO.transform.SetParent(canvas.transform, false);

            Button button = buttonGO.GetComponent<Button>();
            Graphic targetGraphic = button.targetGraphic;

            DestroyImmediate(button);
            ButtonActions buttonActions = buttonGO.AddComponent<ButtonActions>();
            buttonActions.targetGraphic = targetGraphic;
            Selection.activeGameObject = buttonGO;
        }
    }
}