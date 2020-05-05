using System;
using LowPolyHnS.Core;
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
            Close,
            OpenCloseSystem
        }

        public MENU_TYPE menuType = MENU_TYPE.Inventory;
        public ACTION_TYPE actionType = ACTION_TYPE.Open;

        // EXECUTABLE: -------------------------------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            switch (menuType)
            {
                case MENU_TYPE.Inventory:
                    switch (actionType)
                    {
                        case ACTION_TYPE.Open:
                            InventoryUIManager.OpenInventory();
                            break;
                        case ACTION_TYPE.Close:
                            InventoryUIManager.CloseInventory();
                            break;
                        case ACTION_TYPE.OpenCloseSystem:
                            InventoryUIManager.OpenCloseInventory();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return true;
        }

        // +-----------------------------------------------------------------------------------------------------------+
        // | EDITOR                                                                                                    |
        // +-----------------------------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public const string CUSTOM_ICON_PATH = "Assets/Content/Icons/Inventory/Actions/";

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