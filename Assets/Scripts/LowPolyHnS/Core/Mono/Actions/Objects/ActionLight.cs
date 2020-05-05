using LowPolyHnS.Variables;
using UnityEngine;
using UnityEngine.Serialization;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionLight : IAction
    {
        [FormerlySerializedAs("light")] public Light lightTarget;

        public bool changeRange = false;
        public NumberProperty range = new NumberProperty(10f);

        public bool changeIntensity = false;
        public NumberProperty intensity = new NumberProperty(1f);

        public bool changeColor = false;
        public ColorProperty color = new ColorProperty(Color.white);

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (lightTarget != null)
            {
                if (changeRange) lightTarget.range = range.GetValue(target);
                if (changeIntensity) lightTarget.intensity = intensity.GetValue(target);
                if (changeColor) lightTarget.color = color.GetValue(target);
            }

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Object/Light";
        private const string NODE_TITLE = "Change light {0}";

        private static readonly GUIContent GUICONTENT_LIGHT = new GUIContent("Light");

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spLight;
        private SerializedProperty spChangeRange;
        private SerializedProperty spRange;
        private SerializedProperty spChangeIntensity;
        private SerializedProperty spIntensity;
        private SerializedProperty spChangeColor;
        private SerializedProperty spColor;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                lightTarget == null ? "(none)" : lightTarget.gameObject.name
            );
        }

        protected override void OnEnableEditorChild()
        {
            spLight = serializedObject.FindProperty("lightTarget");
            spChangeRange = serializedObject.FindProperty("changeRange");
            spRange = serializedObject.FindProperty("range");
            spChangeIntensity = serializedObject.FindProperty("changeIntensity");
            spIntensity = serializedObject.FindProperty("intensity");
            spChangeColor = serializedObject.FindProperty("changeColor");
            spColor = serializedObject.FindProperty("color");
        }

        protected override void OnDisableEditorChild()
        {
            spLight = null;
            spChangeRange = null;
            spRange = null;
            spChangeIntensity = null;
            spIntensity = null;
            spChangeColor = null;
            spColor = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spLight, GUICONTENT_LIGHT);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(spChangeRange);
            EditorGUI.BeginDisabledGroup(!spChangeRange.boolValue);
            EditorGUILayout.PropertyField(spRange);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spChangeIntensity);
            EditorGUI.BeginDisabledGroup(!spChangeIntensity.boolValue);
            EditorGUILayout.PropertyField(spIntensity);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spChangeColor);
            EditorGUI.BeginDisabledGroup(!spChangeColor.boolValue);
            EditorGUILayout.PropertyField(spColor);
            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}