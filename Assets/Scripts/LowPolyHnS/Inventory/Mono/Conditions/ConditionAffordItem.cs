using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ConditionAffordItem : ICondition
    {
        public ItemHolder item;
        public int amount = 1;

        // EXECUTABLE: -------------------------------------------------------------------------------------------------

        public override bool Check()
        {
            if (InventoryManager.Instance.GetCurrency() >= item.item.price * amount)
            {
                int currentAmount = InventoryManager.Instance.GetInventoryAmountOfItem(item.item.uuid);
                if (currentAmount + amount <= item.item.maxStack) return true;
            }

            return false;
        }

        // +-----------------------------------------------------------------------------------------------------------+
        // | EDITOR                                                                                                    |
        // +-----------------------------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public const string CUSTOM_ICON_PATH = "Assets/Scripts/LowPolyHnS/Inventory/Icons/Conditions/";

        public static new string NAME = "Inventory/Can Buy Item";
        private const string NODE_TITLE = "Can buy {0} {1} item{2}";

        // PROPERTIES: -------------------------------------------------------------------------------------------------

        private SerializedProperty spItem;
        private SerializedProperty spAmount;

        // INSPECTOR METHODS: ------------------------------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                amount,
                item.item == null ? "(none)" : item.item.itemName.content,
                amount == 1 ? "" : "s"
            );
        }

        protected override void OnEnableEditorChild()
        {
            spItem = serializedObject.FindProperty("item");
            spAmount = serializedObject.FindProperty("amount");
        }

        protected override void OnDisableEditorChild()
        {
            spItem = null;
            spAmount = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spItem);
            EditorGUILayout.PropertyField(spAmount);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}