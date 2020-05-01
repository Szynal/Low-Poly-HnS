using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

namespace LowPolyHnS.Core
{
    [CustomEditor(typeof(SliderVariable))]
    public class SliderVariableEditor : SliderEditor
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

        [MenuItem("GameObject/LowPolyHnS/UI/Slider", false, 10)]
        public static void CreateSliderVariable()
        {
            GameObject canvas = CreateSceneObject.GetCanvasGameObject();
            GameObject sliderGO = DefaultControls.CreateSlider(CreateSceneObject.GetStandardResources());
            sliderGO.transform.SetParent(canvas.transform, false);

            Slider slider = sliderGO.GetComponent<Slider>();
            Graphic targetGraphic = slider.targetGraphic;
            RectTransform rectFill = slider.fillRect;
            RectTransform rectHandle = slider.handleRect;

            DestroyImmediate(slider);
            slider = sliderGO.AddComponent<SliderVariable>();
            slider.targetGraphic = targetGraphic;
            slider.fillRect = rectFill;
            slider.handleRect = rectHandle;
            Selection.activeGameObject = sliderGO;
        }
    }
}