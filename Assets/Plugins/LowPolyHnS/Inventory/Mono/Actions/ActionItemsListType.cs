using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionItemsListType : IAction
    {
        public ItemsListUI itemsListUI;

        [InventoryMultiItemType] public int itemTypes = ~0;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (itemsListUI != null)
            {
                itemsListUI.SetItemTypes(itemTypes);
            }

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public const string CUSTOM_ICON_PATH = "Assets/Plugins/LowPolyHnS/Inventory/Icons/Actions/";

        public static new string NAME = "Inventory/Items List UI";
        private const string NODE_TITLE = "Change ItemsListUI types";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spItemsListUI;
        private SerializedProperty spItemTypes;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return NODE_TITLE;
        }

        protected override void OnEnableEditorChild()
        {
            spItemsListUI = serializedObject.FindProperty("itemsListUI");
            spItemTypes = serializedObject.FindProperty("itemTypes");
        }

        protected override void OnDisableEditorChild()
        {
            spItemsListUI = null;
            spItemTypes = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spItemsListUI);
            EditorGUILayout.PropertyField(spItemTypes);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}