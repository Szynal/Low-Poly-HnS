using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionCancelActions : IAction
    {
        public enum Source
        {
            Actions,
            Variable
        }

        public Source source = Source.Actions;
        public Actions actions;

        [VariableFilter(Variable.DataType.GameObject)]
        public VariableProperty variable = new VariableProperty(Variable.VarType.LocalVariable);

        public bool stopImmidiately = false;

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Actions targetActions = null;
            switch (source)
            {
                case Source.Actions:
                    targetActions = this.actions;
                    break;

                case Source.Variable:
                    GameObject value = variable.Get(target) as GameObject;
                    if (value != null) targetActions = value.GetComponentInChildren<Actions>();
                    break;
            }

            if (targetActions != null && targetActions.actionsList != null)
            {
                if (stopImmidiately) targetActions.Stop();
                else targetActions.actionsList.Cancel();
            }

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "General/Cancel Actions";
        private const string NODE_TITLE = "Cancel action {0} {1}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spStopImmidiately;

        private SerializedProperty spSource;
        private SerializedProperty spActions;
        private SerializedProperty spVariable;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            string actionsName = string.Empty;
            switch (source)
            {
                case Source.Actions:
                    actionsName = actions == null
                        ? "(none)"
                        : actions.gameObject.name;
                    break;

                case Source.Variable:
                    actionsName = variable.ToString();
                    break;
            }

            return string.Format(
                NODE_TITLE,
                actionsName,
                stopImmidiately ? "(immidiately)" : string.Empty
            );
        }

        protected override void OnEnableEditorChild()
        {
            spSource = serializedObject.FindProperty("source");
            spActions = serializedObject.FindProperty("actions");
            spVariable = serializedObject.FindProperty("variable");
            spStopImmidiately = serializedObject.FindProperty("stopImmidiately");
        }

        protected override void OnDisableEditorChild()
        {
            spSource = null;
            spActions = null;
            spVariable = null;
            spStopImmidiately = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spSource);
            switch (spSource.enumValueIndex)
            {
                case (int) Source.Actions:
                    EditorGUILayout.PropertyField(spActions);
                    break;

                case (int) Source.Variable:
                    EditorGUILayout.PropertyField(spVariable);
                    break;
            }

            EditorGUILayout.PropertyField(spStopImmidiately);
            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}