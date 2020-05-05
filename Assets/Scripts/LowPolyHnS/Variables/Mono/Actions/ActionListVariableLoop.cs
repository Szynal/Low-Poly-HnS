using System.Collections;
using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Variables
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionListVariableLoop : IAction
    {
        public enum Source
        {
            Actions,
            Conditions,
            VariableWithActions,
            VariableWithConditions
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public HelperListVariable listVariables = new HelperListVariable();

        public Source source = Source.Actions;
        public Actions actions;
        public Conditions conditions;

        [VariableFilter(Variable.DataType.GameObject)]
        public VariableProperty variable = new VariableProperty(Variable.VarType.LocalVariable);

        private bool executionComplete;
        private bool forceStop;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
        {
            ListVariables list = listVariables.GetListVariables(target);
            if (list == null) yield break;

            Actions actionsToExecute = null;
            Conditions conditionsToExecute = null;

            switch (source)
            {
                case Source.Actions:
                    actionsToExecute = this.actions;
                    break;

                case Source.Conditions:
                    conditionsToExecute = conditions;
                    break;

                case Source.VariableWithActions:
                    GameObject valueActions = variable.Get(target) as GameObject;
                    if (valueActions != null) actionsToExecute = valueActions.GetComponent<Actions>();
                    break;

                case Source.VariableWithConditions:
                    GameObject valueConditions = variable.Get(target) as GameObject;
                    if (valueConditions != null) conditionsToExecute = valueConditions.GetComponent<Conditions>();
                    break;
            }

            for (int i = 0; i < list.variables.Count && !forceStop; ++i)
            {
                Variable itemVariable = list.Get(i);
                if (itemVariable == null) continue;

                GameObject itemGo = itemVariable.Get() as GameObject;
                if (itemGo == null) continue;

                executionComplete = false;
                if (actionsToExecute != null)
                {
                    actionsToExecute.actionsList.Execute(itemGo, OnCompleteActions);
                    WaitUntil wait = new WaitUntil(() =>
                    {
                        if (actionsToExecute == null) return true;
                        if (forceStop)
                        {
                            actionsToExecute.actionsList.Stop();
                            return true;
                        }

                        return executionComplete;
                    });

                    yield return wait;
                }
                else if (conditionsToExecute != null)
                {
                    conditionsToExecute.Interact(itemGo);
                }
            }

            yield return 0;
        }

        private void OnCompleteActions()
        {
            executionComplete = true;
        }

        public override void Stop()
        {
            base.Stop();
            forceStop = true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Variables/List Variables Loop";
        private const string NODE_TITLE = "Loop List Variables {0}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spListVariables;
        private SerializedProperty spSource;
        private SerializedProperty spActions;
        private SerializedProperty spConditions;
        private SerializedProperty spVariable;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                listVariables
            );
        }

        protected override void OnEnableEditorChild()
        {
            spListVariables = serializedObject.FindProperty("listVariables");
            spSource = serializedObject.FindProperty("source");
            spVariable = serializedObject.FindProperty("variable");
            spActions = serializedObject.FindProperty("actions");
            spConditions = serializedObject.FindProperty("conditions");
        }

        protected override void OnDisableEditorChild()
        {
            spListVariables = null;
            spSource = null;
            spVariable = null;
            spActions = null;
            spConditions = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spListVariables);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spSource);

            switch (spSource.enumValueIndex)
            {
                case (int) Source.Actions:
                    EditorGUILayout.PropertyField(spActions);
                    break;

                case (int) Source.Conditions:
                    EditorGUILayout.PropertyField(spConditions);
                    break;

                case (int) Source.VariableWithActions:
                    EditorGUILayout.PropertyField(spVariable);
                    break;

                case (int) Source.VariableWithConditions:
                    EditorGUILayout.PropertyField(spVariable);
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}