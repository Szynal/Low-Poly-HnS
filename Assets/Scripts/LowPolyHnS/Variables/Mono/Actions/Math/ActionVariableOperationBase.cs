using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Variables
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public abstract class ActionVariableOperationBase : IAction
    {
        [VariableFilter(Variable.DataType.Number)]
        public VariableProperty variable = new VariableProperty(Variable.VarType.GlobalVariable);

        public float value = 1f;

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spVariable;
        private SerializedProperty spValue;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        protected override void OnEnableEditorChild()
        {
            spVariable = serializedObject.FindProperty("variable");
            spValue = serializedObject.FindProperty("value");
        }

        protected override void OnDisableEditorChild()
        {
            spVariable = null;
            spValue = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spVariable);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spValue);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}