using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Variables
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionVariableRandom : IAction
    {
        public enum Step
        {
            Integer,
            Decimal
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public float minValue = 0.0f;
        public float maxValue = 10.0f;
        public Step step = Step.Decimal;

        [VariableFilter(Variable.DataType.Number)]
        public VariableProperty variable = new VariableProperty();

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            float random = 0.0f;
            switch (step)
            {
                case Step.Decimal:
                    random = Random.Range(minValue, maxValue);
                    break;
                case Step.Integer:
                    random = Random.Range((int) minValue, (int) maxValue);
                    break;
            }

            variable.Set(random, target);
            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Variables/Variable Random";
        private const string NODE_TITLE = "Random value to {0}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spMinValue;
        private SerializedProperty spMaxValue;
        private SerializedProperty spStep;
        private SerializedProperty spVariable;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, variable);
        }

        protected override void OnEnableEditorChild()
        {
            spMinValue = serializedObject.FindProperty("minValue");
            spMaxValue = serializedObject.FindProperty("maxValue");
            spStep = serializedObject.FindProperty("step");
            spVariable = serializedObject.FindProperty("variable");
        }

        protected override void OnDisableEditorChild()
        {
            spMinValue = null;
            spMaxValue = null;
            spStep = null;
            spVariable = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spMinValue);
            EditorGUILayout.PropertyField(spMaxValue);
            EditorGUILayout.PropertyField(spStep);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spVariable);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}