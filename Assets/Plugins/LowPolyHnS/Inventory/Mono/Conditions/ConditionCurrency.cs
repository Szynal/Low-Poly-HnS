using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ConditionCurrency : ICondition
    {
        public int currencyAmount;

        // EXECUTABLE: -------------------------------------------------------------------------------------------------

        public override bool Check()
        {
            int currentCurrency = InventoryManager.Instance.GetCurrency();
            return currencyAmount <= currentCurrency;
        }

        // +-----------------------------------------------------------------------------------------------------------+
        // | EDITOR                                                                                                    |
        // +-----------------------------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public const string CUSTOM_ICON_PATH = "Assets/Plugins/LowPolyHnS/Inventory/Icons/Conditions/";

        public static new string NAME = "Inventory/Enough Currency";
        private const string NODE_TITLE = "Player has at least {0} of currency";

        // PROPERTIES: -------------------------------------------------------------------------------------------------

        private SerializedProperty spCurrencyAmount;

        // INSPECTOR METHODS: ------------------------------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, currencyAmount);
        }

        protected override void OnEnableEditorChild()
        {
            spCurrencyAmount = serializedObject.FindProperty("currencyAmount");
        }

        protected override void OnDisableEditorChild()
        {
            spCurrencyAmount = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spCurrencyAmount);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}