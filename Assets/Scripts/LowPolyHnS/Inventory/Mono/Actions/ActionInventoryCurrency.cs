using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionInventoryCurrency : IAction
    {
        public enum CURRENCY_OPERATION
        {
            Add,
            Substract
        }

        public CURRENCY_OPERATION operation;
        public int amount = 0;

        // EXECUTABLE: -------------------------------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            switch (operation)
            {
                case CURRENCY_OPERATION.Add:
                    InventoryManager.Instance.AddCurrency(amount);
                    break;
                case CURRENCY_OPERATION.Substract:
                    InventoryManager.Instance.SubstractCurrency(amount);
                    break;
            }

            return true;
        }

        // +-----------------------------------------------------------------------------------------------------------+
        // | EDITOR                                                                                                    |
        // +-----------------------------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public const string CUSTOM_ICON_PATH = "Assets/Scripts/LowPolyHnS/Inventory/Icons/Actions/";

        public static new string NAME = "Inventory/Currency";
        private const string NODE_TITLE = "{0} {1} of currency";

        // PROPERTIES: -------------------------------------------------------------------------------------------------

        private SerializedProperty spOperation;
        private SerializedProperty spAmount;

        // INSPECTOR METHODS: ------------------------------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, operation.ToString(), amount.ToString());
        }

        protected override void OnEnableEditorChild()
        {
            spOperation = serializedObject.FindProperty("operation");
            spAmount = serializedObject.FindProperty("amount");
        }

        protected override void OnDisableEditorChild()
        {
            spOperation = null;
            spAmount = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spOperation);
            EditorGUILayout.PropertyField(spAmount);
            spAmount.intValue = Mathf.Max(0, spAmount.intValue);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}