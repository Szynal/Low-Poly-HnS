using System.Collections;
using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionActions : IAction
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

        public bool waitToFinish = false;

        private bool actionsComplete;
        private bool forceStop;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
        {
            Actions actionsToExecute = null;

            switch (source)
            {
                case Source.Actions:
                    actionsToExecute = this.actions;
                    break;

                case Source.Variable:
                    GameObject value = variable.Get(target) as GameObject;
                    if (value != null) actionsToExecute = value.GetComponent<Actions>();
                    break;
            }

            if (actionsToExecute != null)
            {
                actionsComplete = false;
                actionsToExecute.actionsList.Execute(target, OnCompleteActions);

                if (waitToFinish)
                {
                    WaitUntil wait = new WaitUntil(() =>
                    {
                        if (actionsToExecute == null) return true;
                        if (forceStop)
                        {
                            actionsToExecute.actionsList.Stop();
                            return true;
                        }

                        return actionsComplete;
                    });

                    yield return wait;
                }
            }

            yield return 0;
        }

        private void OnCompleteActions()
        {
            actionsComplete = true;
        }

        public override void Stop()
        {
            forceStop = true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "General/Execute Actions";
        private const string NODE_TITLE = "Execute actions {0} {1}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spSource;
        private SerializedProperty spActions;
        private SerializedProperty spVariable;

        private SerializedProperty spWaitToFinish;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            string actionsName = source == Source.Actions
                ? actions == null ? "none" : actions.name
                : variable.ToString();

            return string.Format(
                NODE_TITLE,
                actionsName,
                waitToFinish ? "and wait" : ""
            );
        }

        protected override void OnEnableEditorChild()
        {
            spSource = serializedObject.FindProperty("source");
            spVariable = serializedObject.FindProperty("variable");
            spActions = serializedObject.FindProperty("actions");
            spWaitToFinish = serializedObject.FindProperty("waitToFinish");
        }

        protected override void OnDisableEditorChild()
        {
            spSource = null;
            spVariable = null;
            spActions = null;
            spWaitToFinish = null;
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

            EditorGUILayout.PropertyField(spWaitToFinish);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}