using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
    [CustomEditor(typeof(Merchant))]
    public class MerchantEditor : Editor
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spUuid;
        private SerializedProperty spTitle;
        private SerializedProperty spDescription;
        private SerializedProperty spMerchantUI;

        private SerializedProperty spWares;
        private ReorderableList waresList;

        private SerializedProperty spPurchasePercent;
        private SerializedProperty spSellPercent;

        // METHODS: -------------------------------------------------------------------------------

        private void OnEnable()
        {
            spUuid = serializedObject.FindProperty("uuid");
            if (string.IsNullOrEmpty(spUuid.stringValue))
            {
                spUuid.stringValue = Guid.NewGuid().ToString("N");
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
                serializedObject.Update();
            }

            spTitle = serializedObject.FindProperty("title");
            spDescription = serializedObject.FindProperty("description");
            spMerchantUI = serializedObject.FindProperty("merchantUI");

            SerializedProperty spWarehouse = serializedObject.FindProperty("warehouse");
            spWares = spWarehouse.FindPropertyRelative("wares");

            waresList = new ReorderableList(
                spWares.serializedObject,
                spWares,
                true, true, true, true
            );

            waresList.drawHeaderCallback = WaresList_Header;
            waresList.drawElementCallback = WaresList_Paint;
            waresList.elementHeightCallback = WaresList_Height;

            spPurchasePercent = serializedObject.FindProperty("purchasePercent");
            spSellPercent = serializedObject.FindProperty("sellPercent");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spTitle);
            EditorGUILayout.PropertyField(spDescription);

            EditorGUILayout.Space();
            waresList.DoLayoutList();
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(spMerchantUI);

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(spUuid);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spPurchasePercent);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spSellPercent);

            serializedObject.ApplyModifiedProperties();
        }

        // WARE LIST METHODS: ---------------------------------------------------------------------

        private void WaresList_Header(Rect rect)
        {
            EditorGUI.LabelField(rect, "Wares");
        }

        private void WaresList_Paint(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty spProperty = spWares.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, spProperty);
        }

        private float WaresList_Height(int index)
        {
            return EditorGUI.GetPropertyHeight(spWares.GetArrayElementAtIndex(index)) +
                   EditorGUIUtility.standardVerticalSpacing;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        public string GetHeaderName()
        {
            return spTitle.stringValue;
        }
    }
}