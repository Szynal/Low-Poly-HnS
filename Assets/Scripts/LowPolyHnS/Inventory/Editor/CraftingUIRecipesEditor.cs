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
        private const string PROP_CANVAS_GROUP = "CanvasGroup";

        private SerializedProperty spItemHolder;
        private SerializedProperty spCanvasGroup;

        public void OnEnable()
        {
            if (target == null || serializedObject == null) return;

            craftingUIRecipe = (CraftingUIRecipe) target;

            spItemHolder = serializedObject.FindProperty(PROP_ITEM_HOLDER);
            spCanvasGroup = serializedObject.FindProperty(PROP_CANVAS_GROUP);
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(spItemHolder);
            EditorGUILayout.PropertyField(spCanvasGroup);

            if (GUILayout.Button("Init crafting recipes"))
            {
                craftingUIRecipe.InitRecipe();
            }

            if (GUILayout.Button("Open Preferences"))
            {
                PreferencesWindow.OpenWindow();
            }
        }
    }
}