using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionMerchantMenu : IAction
    {
        public enum ACTION_TYPE
        {
            Open,
            Close
        }

        public ACTION_TYPE actionType = ACTION_TYPE.Open;
        public Merchant merchant;

        // EXECUTABLE: -------------------------------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (actionType == ACTION_TYPE.Open) MerchantUIManager.OpenMerchant(merchant);
            if (actionType == ACTION_TYPE.Close) MerchantUIManager.CloseMerchant();

            return true;
        }

        // +-----------------------------------------------------------------------------------------------------------+
        // | EDITOR                                                                                                    |
        // +-----------------------------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public const string CUSTOM_ICON_PATH = "Assets/Scripts/LowPolyHnS/Inventory/Icons/Actions/";

        public static new string NAME = "Inventory/Merchant UI";
        private const string NODE_TITLE = "{0} merchant {1}";

        // PROPERTIES: -------------------------------------------------------------------------------------------------

        private SerializedProperty spActionType;
        private SerializedProperty spMerchant;

        // INSPECTOR METHODS: ------------------------------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                actionType.ToString(),
                merchant == null ? "(none)" : merchant.name
            );
        }

        protected override void OnEnableEditorChild()
        {
            spMerchant = serializedObject.FindProperty("merchant");
            spActionType = serializedObject.FindProperty("actionType");
        }

        protected override void OnDisableEditorChild()
        {
            spMerchant = null;
            spActionType = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spActionType);
            EditorGUILayout.PropertyField(spMerchant);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}