using System.Collections;
using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionConditions : IAction
    {
        public enum Source
        {
            Conditions,
            Variable
        }

        public Source source = Source.Conditions;
        public Conditions conditions;

        [VariableFilter(Variable.DataType.GameObject)]
        public VariableProperty variable = new VariableProperty(Variable.VarType.LocalVariable);

        public bool waitToFinish = true;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Conditions conditionsToExecute = null;
            switch (source)
            {
                case Source.Conditions:
                    conditionsToExecute = conditions;
                    break;

                case Source.Variable:
                    GameObject value = variable.Get(target) as GameObject;
                    if (value != null) conditionsToExecute = value.GetComponent<Conditions>();
                    break;
            }

            if (!waitToFinish)
            {
                if (conditionsToExecute != null) conditionsToExecute.Interact(target);
                return true;
            }

            return false;
        }

        public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
        {
            Conditions conditionsToExecute = null;
            switch (source)
            {
                case Source.Conditions:
                    conditionsToExecute = conditions;
                    break;

                case Source.Variable:
                    GameObject value = variable.Get(target) as GameObject;
                    if (value != null) conditionsToExecute = value.GetComponent<Conditions>();
                    break;
            }

            if (conditionsToExecute != null)
            {
                yield return conditionsToExecute.InteractCoroutine(target);
            }

            yield return 0;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "General/Call Conditions";
        private const string NODE_TITLE = "Call conditions {0}{1}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spSource;
        private SerializedProperty spConditions;
        private SerializedProperty spVariable;
        private SerializedProperty spWaitToFinish;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            string conditionsName = source == Source.Conditions
                ? conditions == null ? "none" : conditions.name
                : variable.ToString();

            return string.Format(
                NODE_TITLE,
                conditionsName,
                waitToFinish ? " and wait" : ""
            );
        }

        protected override void OnEnableEditorChild()
        {
            spSource = serializedObject.FindProperty("source");
            spVariable = serializedObject.FindProperty("variable");
            spConditions = serializedObject.FindProperty("conditions");
            spWaitToFinish = serializedObject.FindProperty("waitToFinish");
        }

        protected override void OnDisableEditorChild()
        {
            spSource = null;
            spVariable = null;
            spConditions = null;
            spWaitToFinish = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spSource);

            EditorGUI.indentLevel++;
            switch (spSource.enumValueIndex)
            {
                case (int) Source.Conditions:
                    EditorGUILayout.PropertyField(spConditions);
                    break;

                case (int) Source.Variable:
                    EditorGUILayout.PropertyField(spVariable);
                    break;
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(spWaitToFinish);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}