using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Variables
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionVariableDebug : IAction
    {
        private const string LOG_FMT = "{0}: {1}";

        // PROPERTIES: ----------------------------------------------------------------------------

        public VariableProperty variable = new VariableProperty();

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Debug.LogFormat(
                LOG_FMT,
                variable.ToString(),
                variable.ToStringValue(target)
            );

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Debug/Debug Variable";
        private const string NODE_TITLE = "Log variable {0}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spVariable;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, variable);
        }

        protected override void OnEnableEditorChild()
        {
            spVariable = serializedObject.FindProperty("variable");
        }

        protected override void OnDisableEditorChild()
        {
            spVariable = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(spVariable);
            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}