using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionQualitySettings : IAction
    {
        public NumberProperty level = new NumberProperty(0);
        public bool applyExpensiveSettings = true;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            QualitySettings.SetQualityLevel(
                (int) level.GetValue(target),
                applyExpensiveSettings
            );

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Application/Quality Settings";
        private const string NODE_TITLE = "Change quality settings";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spLevel;
        private SerializedProperty spApplyExpensive;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return NODE_TITLE;
        }

        protected override void OnEnableEditorChild()
        {
            spLevel = serializedObject.FindProperty("level");
            spApplyExpensive = serializedObject.FindProperty("applyExpensiveSettings");
        }

        protected override void OnDisableEditorChild()
        {
            spLevel = null;
            spApplyExpensive = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spLevel);
            EditorGUILayout.PropertyField(spApplyExpensive);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}