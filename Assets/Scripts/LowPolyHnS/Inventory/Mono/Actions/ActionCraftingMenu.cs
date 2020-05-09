using System;
using LowPolyHnS.Core;
using LowPolyHnS.Crafting;
using UnityEditor;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
#if UNITY_EDITOR
#endif
    public class ActionCraftingMenu : IAction
    {
        public enum MENU_TYPE
        {
            Crafting
        }

        public enum ACTION_TYPE
        {
            Open,
            Close,
            OpenCloseSystem
        }

        public MENU_TYPE MenuType = MENU_TYPE.Crafting;
        public ACTION_TYPE ActionType = ACTION_TYPE.Open;

        #region EXECUTABLE

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            switch (MenuType)
            {
                case MENU_TYPE.Crafting:
                    switch (ActionType)
                    {
                        case ACTION_TYPE.Open:
                            CraftingUIManager.OpenCraftingTable();
                            break;
                        case ACTION_TYPE.Close:
                            CraftingUIManager.CloseCraftingTable();
                            break;
                        case ACTION_TYPE.OpenCloseSystem:
                            CraftingUIManager.OpenCloseCraftingTable();
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

        #endregion

#if UNITY_EDITOR

        public const string CUSTOM_ICON_PATH = "Assets/Content/Icons/Inventory/Actions/";

        public new static string NAME = "Inventory/Crafting UI";
        private const string NODE_TITLE = "{0} {1} menu";


        private SerializedProperty spMenuType;
        private SerializedProperty spActionType;

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                ActionType.ToString(),
                MenuType.ToString()
            );
        }

        protected override void OnEnableEditorChild()
        {
            spMenuType = serializedObject.FindProperty("MenuType");
            spActionType = serializedObject.FindProperty("ActionType");
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