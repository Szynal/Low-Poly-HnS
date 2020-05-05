using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionInventoryItem : IAction
    {
        public enum ITEM_ACTION
        {
            Add,
            Substract,
            Consume
        }

        public ITEM_ACTION operation = ITEM_ACTION.Add;
        public ItemHolder itemHolder;
        public int amount = 1;

        // EXECUTABLE: -------------------------------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            switch (operation)
            {
                case ITEM_ACTION.Add:
                    InventoryManager.Instance.AddItemToInventory(itemHolder.item.uuid, amount);
                    break;

                case ITEM_ACTION.Substract:
                    InventoryManager.Instance.SubstractItemFromInventory(itemHolder.item.uuid, amount);
                    break;

                case ITEM_ACTION.Consume:
                    InventoryManager.Instance.ConsumeItem(itemHolder.item.uuid);
                    break;
            }

            return true;
        }

        // +-----------------------------------------------------------------------------------------------------------+
        // | EDITOR                                                                                                    |
        // +-----------------------------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public const string CUSTOM_ICON_PATH = "Assets/Scripts/LowPolyHnS/Inventory/Icons/Actions/";

        public static new string NAME = "Inventory/Item";
        private const string NODE_TITLE = "{0} {1} {2} item{3}";

        // PROPERTIES: -------------------------------------------------------------------------------------------------

        private SerializedProperty spOperation;
        private SerializedProperty spItemHolder;
        private SerializedProperty spAmount;

        // INSPECTOR METHODS: ------------------------------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                operation.ToString(),
                amount.ToString(),
                itemHolder.item == null ? "nothing" : itemHolder.item.itemName.content,
                amount != 1 ? "s" : ""
            );
        }

        protected override void OnEnableEditorChild()
        {
            spOperation = serializedObject.FindProperty("operation");
            spItemHolder = serializedObject.FindProperty("itemHolder");
            spAmount = serializedObject.FindProperty("amount");
        }

        protected override void OnDisableEditorChild()
        {
            spOperation = null;
            spItemHolder = null;
            spAmount = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(spOperation);
            EditorGUILayout.PropertyField(spItemHolder);

            EditorGUILayout.PropertyField(spAmount);
            spAmount.intValue = Mathf.Max(0, spAmount.intValue);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}