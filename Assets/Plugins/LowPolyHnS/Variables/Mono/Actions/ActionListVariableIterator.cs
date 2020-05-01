using LowPolyHnS.Core;
using UnityEditor;
using UnityEngine;

namespace LowPolyHnS.Variables
{
    [AddComponentMenu("")]
    public class ActionListVariableIterator : IAction
    {
        public enum Operation
        {
            VariableToIterator,
            IteratorToVariable,
            IteratorToNext,
            IteratorToPrevious
        }

        public Operation operation = Operation.VariableToIterator;
        public HelperListVariable listVariables = new HelperListVariable();

        [VariableFilter(Variable.DataType.Number)]
        public VariableProperty variable = new VariableProperty();

        public NumberProperty pointer = new NumberProperty(0);

        // EXECUTE METHOD: ------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            ListVariables list = listVariables.GetListVariables(target);
            if (list == null) return true;

            switch (operation)
            {
                case Operation.VariableToIterator:
                    int value = pointer.GetInt(target);
                    list.SetInterator(value);
                    break;

                case Operation.IteratorToVariable:
                    variable.Set(list.iterator, target);
                    break;

                case Operation.IteratorToNext:
                    list.NextIterator();
                    break;

                case Operation.IteratorToPrevious:
                    list.PrevIterator();
                    break;
            }

            return true;
        }

#if UNITY_EDITOR

        private const string NODE_TITLE_1 = "Set List Variables {0} from iterator";
        private const string NODE_TITLE_2 = "Set iterator to List Variables {0}";
        private const string NODE_TITLE_3 = "Set iterator to {0}";

        public static new string NAME = "Variables/List Variables Iterator";

        public override string GetNodeTitle()
        {
            switch (operation)
            {
                case Operation.VariableToIterator:
                    return string.Format(
                        NODE_TITLE_1,
                        listVariables
                    );

                case Operation.IteratorToVariable:
                    return string.Format(
                        NODE_TITLE_2,
                        listVariables
                    );

                case Operation.IteratorToNext:
                    return string.Format(
                        NODE_TITLE_3,
                        "next position"
                    );
                case Operation.IteratorToPrevious:
                    return string.Format(
                        NODE_TITLE_3,
                        "previous position"
                    );
            }

            return string.Empty;
        }

        private SerializedProperty spOperation;
        private SerializedProperty spListVariables;

        private SerializedProperty spVariable;
        private SerializedProperty spPointer;

        protected override void OnEnableEditorChild()
        {
            spOperation = serializedObject.FindProperty("operation");
            spListVariables = serializedObject.FindProperty("listVariables");
            spVariable = serializedObject.FindProperty("variable");
            spPointer = serializedObject.FindProperty("pointer");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spOperation);
            EditorGUILayout.PropertyField(spListVariables);

            EditorGUILayout.Space();
            switch (spOperation.enumValueIndex)
            {
                case (int) Operation.VariableToIterator:
                    EditorGUILayout.PropertyField(spPointer);
                    break;

                case (int) Operation.IteratorToVariable:
                    EditorGUILayout.PropertyField(spVariable);
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}