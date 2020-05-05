using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ConditionItem : ICondition
    {
        public ItemHolder item;
        public int minAmount;

        // EXECUTABLE: -------------------------------------------------------------------------------------------------

        public override bool Check()
        {
            int currentAmount = InventoryManager.Instance.GetInventoryAmountOfItem(item.item.uuid);
            return minAmount <= currentAmount;
        }

        // +-----------------------------------------------------------------------------------------------------------+
        // | EDITOR                                                                                                    |
        // +-----------------------------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public const string CUSTOM_ICON_PATH = "Assets/Scripts/LowPolyHnS/Inventory/Icons/Conditions/";

        public static new string NAME = "Inventory/Item in Inventory";
        private const string NODE_TITLE = "Player has {0} {1} item{2} or more";

        // PROPERTIES: -------------------------------------------------------------------------------------------------

        private SerializedProperty spItem;
        private SerializedProperty spMinAmount;

        // INSPECTOR METHODS: ------------------------------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                minAmount,
                item.item == null ? "(none)" : item.item.itemName.content,
                minAmount == 1 ? "" : "s"
            );
        }

        protected override void OnEnableEditorChild()
        {
            spItem = serializedObject.FindProperty("item");
            spMinAmount = serializedObject.FindProperty("minAmount");
        }

        protected override void OnDisableEditorChild()
        {
            spItem = null;
            spMinAmount = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spItem);
            EditorGUILayout.PropertyField(spMinAmount);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}