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
        private const string PROP_CRAFT_TABLE_PREFAB = "CraftTablePrefab";
        private const string PROP_CANVAS_GROUP = "CanvasGroup";
        private const string PROP_CRAFT_TABLE = "CraftTable";


        private SerializedProperty spItemHolder;
        private SerializedProperty CraftTablePrefab;
        private SerializedProperty spCanvasGroup;
        private SerializedProperty spCraftTable;

        public void OnEnable()
        {
            if (target == null || serializedObject == null) return;

            craftingUIRecipe = (CraftingUIRecipe) target;

            spItemHolder = serializedObject.FindProperty(PROP_ITEM_HOLDER);
            CraftTablePrefab = serializedObject.FindProperty(PROP_CRAFT_TABLE_PREFAB);
            spCanvasGroup = serializedObject.FindProperty(PROP_CANVAS_GROUP);
            spCraftTable = serializedObject.FindProperty(PROP_CRAFT_TABLE);
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(spItemHolder);
            EditorGUILayout.PropertyField(CraftTablePrefab);
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