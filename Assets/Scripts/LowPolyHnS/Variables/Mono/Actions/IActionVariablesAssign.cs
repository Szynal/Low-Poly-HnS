using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public abstract class IActionVariablesAssign : IAction
    {
        public enum ValueFrom
        {
            Player,
            Invoker,
            Constant,
            GlobalVariable,
            LocalVariable,
            ListVariable
        }

        public ValueFrom valueFrom = ValueFrom.Constant;
        public HelperGlobalVariable global = new HelperGlobalVariable();
        public HelperLocalVariable local = new HelperLocalVariable();
        public HelperGetListVariable list = new HelperGetListVariable();

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            ExecuteAssignement(target);
            return true;
        }

        public abstract void ExecuteAssignement(GameObject target);

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Variables/Variable Assign";
        protected const string NODE_TITLE = "Assign {0} to {1}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spVariable;
        private SerializedProperty spValueFrom;
        private SerializedProperty spValue;
        private SerializedProperty spGlobal;
        private SerializedProperty spLocal;
        private SerializedProperty spList;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, "(none)", "(none)");
        }

        protected override void OnEnableEditorChild()
        {
            spVariable = serializedObject.FindProperty("variable");
            spValueFrom = serializedObject.FindProperty("valueFrom");
            spValue = serializedObject.FindProperty("value");
            spGlobal = serializedObject.FindProperty("global");
            spLocal = serializedObject.FindProperty("local");
            spList = serializedObject.FindProperty("list");
        }

        protected override void OnDisableEditorChild()
        {
            spVariable = null;
            spValueFrom = null;
            spValue = null;
            spGlobal = null;
            spLocal = null;
            spList = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spVariable);
            EditorGUILayout.Space();

            if (PaintInspectorTarget())
            {
                EditorGUILayout.PropertyField(spValueFrom);
                switch ((ValueFrom) spValueFrom.enumValueIndex)
                {
                    case ValueFrom.Constant:
                        EditorGUILayout.PropertyField(spValue);
                        break;

                    case ValueFrom.GlobalVariable:
                        EditorGUILayout.PropertyField(spGlobal);
                        break;

                    case ValueFrom.LocalVariable:
                        EditorGUILayout.PropertyField(spLocal);
                        break;

                    case ValueFrom.ListVariable:
                        EditorGUILayout.PropertyField(spList);
                        break;
                }
            }
            else
            {
                EditorGUILayout.PropertyField(spValue);
            }

            serializedObject.ApplyModifiedProperties();
        }

        public abstract bool PaintInspectorTarget();

#endif
    }
}