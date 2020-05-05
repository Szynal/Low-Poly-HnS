using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionInventoryShop : IAction
    {
        public enum ACTION
        {
            Buy,
            Sell
        }

        public ACTION action = ACTION.Buy;
        public ItemHolder item;
        public int amount = 1;

        // EXECUTABLE: -------------------------------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            switch (action)
            {
                case ACTION.Buy:
                    InventoryManager.Instance.BuyItem(item.item.uuid, amount);
                    break;
                case ACTION.Sell:
                    InventoryManager.Instance.SellItem(item.item.uuid, amount);
                    break;
            }

            return true;
        }

        // +-----------------------------------------------------------------------------------------------------------+
        // | EDITOR                                                                                                    |
        // +-----------------------------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public const string CUSTOM_ICON_PATH = "Assets/Scripts/LowPolyHnS/Inventory/Icons/Actions/";

        public static new string NAME = "Inventory/Shop";
        private const string NODE_TITLE = "{0} {1} {2} item{3}";

        // PROPERTIES: -----------------------------------------------------------------------------------------------------

        private SerializedProperty spAction;
        private SerializedProperty spItem;
        private SerializedProperty spAmount;

        // INSPECTOR METHODS: ----------------------------------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                action.ToString(),
                amount,
                item.item == null ? "(none)" : item.item.itemName.content,
                amount == 1 ? "" : "s"
            );
        }

        protected override void OnEnableEditorChild()
        {
            spAction = serializedObject.FindProperty("action");
            spItem = serializedObject.FindProperty("item");
            spAmount = serializedObject.FindProperty("amount");
        }

        protected override void OnDisableEditorChild()
        {
            spAction = null;
            spItem = null;
            spAmount = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spAction);
            EditorGUILayout.PropertyField(spItem);
            EditorGUILayout.PropertyField(spAmount);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}