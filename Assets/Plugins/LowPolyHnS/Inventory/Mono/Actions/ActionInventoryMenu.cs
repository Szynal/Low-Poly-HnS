﻿using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionInventoryMenu : IAction
    {
        public enum MENU_TYPE
        {
            Inventory
        }

        public enum ACTION_TYPE
        {
            Open,
            Close
        }

        public MENU_TYPE menuType = MENU_TYPE.Inventory;
        public ACTION_TYPE actionType = ACTION_TYPE.Open;

        // EXECUTABLE: -------------------------------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            switch (menuType)
            {
                case MENU_TYPE.Inventory:
                    if (actionType == ACTION_TYPE.Open) InventoryUIManager.OpenInventory();
                    if (actionType == ACTION_TYPE.Close) InventoryUIManager.CloseInventory();
                    break;
            }

            return true;
        }

        // +-----------------------------------------------------------------------------------------------------------+
        // | EDITOR                                                                                                    |
        // +-----------------------------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public const string CUSTOM_ICON_PATH = "Assets/Plugins/LowPolyHnS/Inventory/Icons/Actions/";

        public static new string NAME = "Inventory/Inventory UI";
        private const string NODE_TITLE = "{0} {1} menu";

        // PROPERTIES: -------------------------------------------------------------------------------------------------

        private SerializedProperty spMenuType;
        private SerializedProperty spActionType;

        // INSPECTOR METHODS: ------------------------------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                actionType.ToString(),
                menuType.ToString()
            );
        }

        protected override void OnEnableEditorChild()
        {
            spMenuType = serializedObject.FindProperty("menuType");
            spActionType = serializedObject.FindProperty("actionType");
        }

        protected override void OnDisableEditorChild()
        {
            spMenuType = null;
            spActionType = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spMenuType);
            EditorGUILayout.PropertyField(spActionType);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}