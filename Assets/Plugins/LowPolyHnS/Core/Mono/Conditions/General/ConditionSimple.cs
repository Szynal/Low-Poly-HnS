using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ConditionSimple : ICondition
    {
        public bool satisfied = true;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool Check()
        {
            return satisfied;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "General/Simple Condition";
        private const string NODE_TITLE = "Condition is always {0}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spSatisfied;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, satisfied);
        }

        protected override void OnEnableEditorChild()
        {
            spSatisfied = serializedObject.FindProperty("satisfied");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spSatisfied, new GUIContent("Is Condition Satisfied?"));

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}