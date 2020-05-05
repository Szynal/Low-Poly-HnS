using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public abstract class ConditionVariable : ICondition
    {
        public enum Comparison
        {
            Equal,
            EqualInteger,
            Less,
            LessOrEqual,
            Greater,
            GreaterOrEqual
        }

        public Comparison comparison = Comparison.Equal;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool Check(GameObject target)
        {
            return Compare(target);
        }

        protected abstract bool Compare(GameObject target);

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Variables/Variable";
        protected const string NODE_TITLE = "Compare {0} with {1}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spVariable;
        private SerializedProperty spCompareTo;
        private SerializedProperty spComparison;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return "unknown";
        }

        protected override void OnEnableEditorChild()
        {
            spVariable = serializedObject.FindProperty("variable");
            spCompareTo = serializedObject.FindProperty("compareTo");
            spComparison = serializedObject.FindProperty("comparison");
        }

        protected virtual bool ShowComparison()
        {
            return false;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(spVariable);

            if (ShowComparison())
            {
                EditorGUILayout.PropertyField(spComparison);
            }

            EditorGUILayout.PropertyField(spCompareTo);
            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}