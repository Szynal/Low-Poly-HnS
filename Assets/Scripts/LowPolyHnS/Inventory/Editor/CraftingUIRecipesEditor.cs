using LowPolyHnS.Core;
using UnityEditor;
using UnityEngine;

namespace LowPolyHnS.Crafting
{
    [CustomEditor(typeof(CraftingUIRecipe))]
    public class CraftingUIRecipesEditor : Editor
    {
        private CraftingUIRecipe craftingUIRecipe;

        private const string PROP_ITEM_HOLDER = "ItemHolder";
        private const string PROP_CATALOGUE_KEY = "CatalogueKey";
        private const string PROP_CRAFT_TABLE_PREFAB = "CraftTablePrefab";
        private const string PROP_CANVAS_GROUP = "CanvasGroup";
        private const string PROP_CRAFT_TABLE = "CraftTable";

        private SerializedProperty spItemHolder;
        private SerializedProperty spCatalogueKey;
        private SerializedProperty spCraftTablePrefab;
        private SerializedProperty spCanvasGroup;
        private SerializedProperty spCraftTable;

        public void OnEnable()
        {
            if (target == null || serializedObject == null) return;

            craftingUIRecipe = (CraftingUIRecipe) target;

            spItemHolder = serializedObject.FindProperty(PROP_ITEM_HOLDER);
            spCatalogueKey = serializedObject.FindProperty(PROP_CATALOGUE_KEY);
            spCraftTablePrefab = serializedObject.FindProperty(PROP_CRAFT_TABLE_PREFAB);
            spCanvasGroup = serializedObject.FindProperty(PROP_CANVAS_GROUP);
            spCraftTable = serializedObject.FindProperty(PROP_CRAFT_TABLE);
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(spItemHolder);
            EditorGUILayout.PropertyField(spCatalogueKey);
            EditorGUILayout.PropertyField(spCraftTablePrefab);
            EditorGUILayout.PropertyField(spCanvasGroup);
            EditorGUILayout.PropertyField(spCraftTable);

            if (GUILayout.Button("Init crafting recipes"))
            {
                craftingUIRecipe.InitRecipe();
            }

            if (GUILayout.Button("Open Preferences"))
            {
                PreferencesWindow.OpenWindow();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}