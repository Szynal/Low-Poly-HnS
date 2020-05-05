using System;
using System.Collections.Generic;
using LowPolyHnS.Core;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LowPolyHnS.Inventory
{
    [CustomEditor(typeof(DatabaseInventory))]
    public class DatabaseInventoryEditor : IDatabaseEditor
    {
        private const string PROP_INVENTORY_CATALOGUE = "inventoryCatalogue";
        private const string PROP_INVENTORY_CATALOGUE_ITEMS = "items";
        private const string PROP_INVENTORY_CATALOGUE_RECIPE = "recipes";
        private const string PROP_INVENTORY_CATALOGUE_TYPES = "itemTypes";

        private const string PROP_INVENTORY_SETTINGS = "inventorySettings";
        private const string PROP_CONTAINER_UI_PREFAB = "containerUIPrefab";
        private const string PROP_MERCHANT_UI_PREFAB = "merchantUIPrefab";
        private const string PROP_INVENTORY_UI_PREFAB = "inventoryUIPrefab";
        private const string PROP_INVENTORY_ONDRAG_GRABITEM = "onDragGrabItem";
        private const string PROP_ITEM_CURSOR_DRAG = "cursorDrag";
        private const string PROP_ITEM_CURSOR_DRAG_HS = "cursorDragHotspot";

        private const string PROP_ITEM_DRAG_TO_COMBINE = "dragItemsToCombine";
        private const string PROP_STOPTIME_ONOPEN = "pauseTimeOnUI";
        private const string PROP_DROP_ITEM_OUTSIDE = "canDropItems";
        private const string PROP_SAVE_INVENTORY = "saveInventory";
        private const string PROP_DROP_MAX_DISTANCE = "dropItemMaxDistance";

        private const string PROP_LIMIT_WEIGHT = "limitInventoryWeight";
        private const string PROP_MAX_WEIGHT = "maxInventoryWeight";

        private const string MSG_EMPTY_CATALOGUE = "There are no items. Add one clicking the 'Create Item' button";
        private const string MSG_EMPTY_RECIPES = "There are no recipes. Add one clicking the 'Create Recipe' button";

        private const string SEARCHBOX_NAME = "searchbox";

        private static readonly GUIContent GC_MERCHANT = new GUIContent("Merchant UI Prefab (opt)");
        private static readonly GUIContent GC_CONTAINER = new GUIContent("Container UI Prefab (opt)");
        private static readonly GUIContent GC_INVENTORY = new GUIContent("Inventory UI Prefab (opt)");

        private class ItemsData
        {
            public ItemEditor cachedEditor;
            public SerializedProperty spItem;

            public ItemsData(SerializedProperty item)
            {
                spItem = item;

                Editor cache = cachedEditor;
                CreateCachedEditor(item.objectReferenceValue, typeof(ItemEditor), ref cache);
                cachedEditor = (ItemEditor) cache;
            }
        }

        private class RecipeData
        {
            public RecipeEditor cachedEditor;
            public SerializedProperty spRecipe;

            public RecipeData(SerializedProperty recipe)
            {
                spRecipe = recipe;

                Editor cache = cachedEditor;
                CreateCachedEditor(recipe.objectReferenceValue, typeof(RecipeEditor), ref cache);
                cachedEditor = (RecipeEditor) cache;
            }
        }

        private static readonly GUIContent[] TAB_NAMES =
        {
            new GUIContent("Catalogue"),
            new GUIContent("Types"),
            new GUIContent("Recipes"),
            new GUIContent("Settings")
        };

        // PROPERTIES: -------------------------------------------------------------------------------------------------

        private int tabIndex;

        public SerializedProperty spItems;
        public SerializedProperty spRecipes;
        public SerializedProperty spItemTypes;

        private SerializedProperty spContainerUIPrefab;
        private SerializedProperty spMerchantUIPrefab;
        private SerializedProperty spInventoryUIPrefab;
        private SerializedProperty spItemOnDragGrabItem;
        private SerializedProperty spItemCursorDrag;
        private SerializedProperty spSaveInventory;

        private SerializedProperty spItemCursorDragHotspot;

        private SerializedProperty spItemDragToCombine;
        private SerializedProperty spInventoryStopTime;
        private SerializedProperty spCanDropItems;
        private SerializedProperty spDropMaxDistance;

        private SerializedProperty spLimitWeight;
        private SerializedProperty spMaxWeight;

        private List<ItemsData> itemsData;
        private List<RecipeData> recipesData;

        private GUIStyle searchFieldStyle;
        private GUIStyle searchCloseOnStyle;
        private GUIStyle searchCloseOffStyle;

        public string searchText = "";
        public bool searchFocus = true;

        public EditorSortableList editorSortableListItems;
        public EditorSortableList editorSortableListRecipes;

        public Dictionary<int, Rect> itemsHandleRect = new Dictionary<int, Rect>();
        public Dictionary<int, Rect> recipesHandleRect = new Dictionary<int, Rect>();

        public Dictionary<int, Rect> itemsHandleRectRow = new Dictionary<int, Rect>();
        public Dictionary<int, Rect> recipesHandleRectRow = new Dictionary<int, Rect>();

        // INITIALIZE: -------------------------------------------------------------------------------------------------

        private void OnEnable()
        {
            if (target == null || serializedObject == null) return;

            SerializedProperty spInventoryCatalogue = serializedObject.FindProperty(PROP_INVENTORY_CATALOGUE);
            spItems = spInventoryCatalogue.FindPropertyRelative(PROP_INVENTORY_CATALOGUE_ITEMS);
            spRecipes = spInventoryCatalogue.FindPropertyRelative(PROP_INVENTORY_CATALOGUE_RECIPE);
            spItemTypes = spInventoryCatalogue.FindPropertyRelative(PROP_INVENTORY_CATALOGUE_TYPES);

            SerializedProperty spInventorySettings = serializedObject.FindProperty(PROP_INVENTORY_SETTINGS);
            spMerchantUIPrefab = spInventorySettings.FindPropertyRelative(PROP_MERCHANT_UI_PREFAB);
            spContainerUIPrefab = spInventorySettings.FindPropertyRelative(PROP_CONTAINER_UI_PREFAB);
            spInventoryUIPrefab = spInventorySettings.FindPropertyRelative(PROP_INVENTORY_UI_PREFAB);
            spItemOnDragGrabItem = spInventorySettings.FindPropertyRelative(PROP_INVENTORY_ONDRAG_GRABITEM);
            spItemCursorDrag = spInventorySettings.FindPropertyRelative(PROP_ITEM_CURSOR_DRAG);
            spSaveInventory = spInventorySettings.FindPropertyRelative(PROP_SAVE_INVENTORY);

            spItemCursorDragHotspot = spInventorySettings.FindPropertyRelative(PROP_ITEM_CURSOR_DRAG_HS);

            spItemDragToCombine = spInventorySettings.FindPropertyRelative(PROP_ITEM_DRAG_TO_COMBINE);
            spInventoryStopTime = spInventorySettings.FindPropertyRelative(PROP_STOPTIME_ONOPEN);
            spCanDropItems = spInventorySettings.FindPropertyRelative(PROP_DROP_ITEM_OUTSIDE);
            spDropMaxDistance = spInventorySettings.FindPropertyRelative(PROP_DROP_MAX_DISTANCE);

            spLimitWeight = spInventorySettings.FindPropertyRelative(PROP_LIMIT_WEIGHT);
            spMaxWeight = spInventorySettings.FindPropertyRelative(PROP_MAX_WEIGHT);

            int itemsSize = spItems.arraySize;
            itemsData = new List<ItemsData>();
            for (int i = 0; i < itemsSize; ++i)
            {
                itemsData.Add(new ItemsData(spItems.GetArrayElementAtIndex(i)));
            }

            int recipesSize = spRecipes.arraySize;
            recipesData = new List<RecipeData>();
            for (int i = 0; i < recipesSize; ++i)
            {
                recipesData.Add(new RecipeData(spRecipes.GetArrayElementAtIndex(i)));
            }

            editorSortableListItems = new EditorSortableList();
            editorSortableListRecipes = new EditorSortableList();
        }

        // OVERRIDE METHODS: -------------------------------------------------------------------------------------------

        public override string GetName()
        {
            return "Inventory";
        }

        public override bool CanBeDecoupled()
        {
            return true;
        }

        // GUI METHODS: ------------------------------------------------------------------------------------------------

        public override void OnInspectorGUI()
        {
            OnPreferencesWindowGUI();
        }

        public override void OnPreferencesWindowGUI()
        {
            serializedObject.Update();

            int prevTabIndex = tabIndex;
            tabIndex = GUILayout.Toolbar(tabIndex, TAB_NAMES);
            if (prevTabIndex != tabIndex) ResetSearch();

            EditorGUILayout.Space();

            switch (tabIndex)
            {
                case 0:
                    PaintCatalogue();
                    break;
                case 1:
                    PaintTypes();
                    break;
                case 2:
                    PaintRecipes();
                    break;
                case 3:
                    PaintSettings();
                    break;
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private void PaintCatalogue()
        {
            ItemEditor.ItemReturnOperation returnOp = new ItemEditor.ItemReturnOperation();
            int removeIndex = -1;
            int duplicateIndex = -1;

            PaintSearch();

            int itemsCatalogueSize = spItems.arraySize;
            if (itemsCatalogueSize == 0)
            {
                EditorGUILayout.HelpBox(MSG_EMPTY_CATALOGUE, MessageType.Info);
            }

            for (int i = 0; i < itemsCatalogueSize; ++i)
            {
                if (itemsData[i].cachedEditor == null) continue;
                returnOp = itemsData[i].cachedEditor.OnPreferencesWindowGUI(this, i);
                if (returnOp.removeIndex) removeIndex = i;
                if (returnOp.duplicateIndex) duplicateIndex = i;
            }

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Create Item", GUILayout.MaxWidth(200)))
            {
                ResetSearch();

                int insertIndex = itemsCatalogueSize;
                spItems.InsertArrayElementAtIndex(insertIndex);

                Item item = Item.CreateItemInstance();
                spItems.GetArrayElementAtIndex(insertIndex).objectReferenceValue = item;
                itemsData.Insert(insertIndex, new ItemsData(spItems.GetArrayElementAtIndex(insertIndex)));
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            if (removeIndex != -1)
            {
                itemsData[removeIndex].cachedEditor.OnDestroyItem();
                Object deleteItem = itemsData[removeIndex].cachedEditor.target;
                spItems.RemoveFromObjectArrayAt(removeIndex);
                itemsData.RemoveAt(removeIndex);

                string path = AssetDatabase.GetAssetPath(deleteItem);
                DestroyImmediate(deleteItem, true);
                AssetDatabase.ImportAsset(path);
            }
            else if (duplicateIndex != -1)
            {
                ResetSearch();

                int srcIndex = duplicateIndex;
                int insertIndex = duplicateIndex + 1;

                spItems.InsertArrayElementAtIndex(insertIndex);

                Item item = Item.CreateItemInstance();
                EditorUtility.CopySerialized(
                    spItems.GetArrayElementAtIndex(srcIndex).objectReferenceValue,
                    item
                );

                SerializedProperty newItem = spItems.GetArrayElementAtIndex(insertIndex);
                newItem.objectReferenceValue = item;

                newItem.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                newItem.serializedObject.Update();

                ItemsData newItemData = new ItemsData(newItem);

                newItemData.cachedEditor.serializedObject
                    .FindProperty("actionsOnClick")
                    .objectReferenceValue = MakeCopyOf(item.actionsOnClick);

                newItemData.cachedEditor.serializedObject
                    .FindProperty("actionsOnEquip")
                    .objectReferenceValue = MakeCopyOf(item.actionsOnEquip);

                newItemData.cachedEditor.serializedObject
                    .FindProperty("actionsOnUnequip")
                    .objectReferenceValue = MakeCopyOf(item.actionsOnUnequip);

                newItemData.cachedEditor.serializedObject
                    .FindProperty("conditionsEquip")
                    .objectReferenceValue = MakeCopyOf(item.conditionsEquip);

                int uuid = Mathf.Abs(Guid.NewGuid().GetHashCode());
                newItemData.cachedEditor.spUUID.intValue = uuid;

                newItemData.cachedEditor.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                newItemData.cachedEditor.serializedObject.Update();
                newItemData.cachedEditor.OnEnable();

                itemsData.Insert(insertIndex, newItemData);
            }

            EditorSortableList.SwapIndexes swapIndexes = editorSortableListItems.GetSortIndexes();
            if (swapIndexes != null)
            {
                spItems.MoveArrayElement(swapIndexes.src, swapIndexes.dst);

                ItemsData tempItem = itemsData[swapIndexes.src];
                itemsData[swapIndexes.src] = itemsData[swapIndexes.dst];
                itemsData[swapIndexes.dst] = tempItem;
            }
        }

        private GameObject MakeCopyOf(Object original)
        {
            string originalPath = AssetDatabase.GetAssetPath(original);
            string targetPath = AssetDatabase.GenerateUniqueAssetPath(originalPath);

            AssetDatabase.CopyAsset(originalPath, targetPath);
            AssetDatabase.Refresh();

            return AssetDatabase.LoadAssetAtPath<GameObject>(targetPath);
        }

        private void PaintTypes()
        {
            spItemTypes.arraySize = ItemType.MAX;
            for (int i = 0; i < ItemType.MAX; ++i)
            {
                SerializedProperty spItemType = spItemTypes.GetArrayElementAtIndex(i);
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.PropertyField(
                    spItemType.FindPropertyRelative("id"),
                    new GUIContent(string.Format("ID: {0}", i + 1))
                );
                EditorGUILayout.PropertyField(spItemType.FindPropertyRelative("name"), GUIContent.none);

                EditorGUILayout.EndHorizontal();
            }
        }

        private void PaintRecipes()
        {
            int removeIndex = -1;

            int recipeCatalogueSize = spRecipes.arraySize;
            if (recipeCatalogueSize == 0)
            {
                EditorGUILayout.HelpBox(MSG_EMPTY_RECIPES, MessageType.Info);
            }

            for (int i = 0; i < recipeCatalogueSize; ++i)
            {
                bool removeRecipe = recipesData[i].cachedEditor.OnPreferencesWindowGUI(this, i);
                if (removeRecipe) removeIndex = i;
            }

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Create Recipe", GUILayout.MaxWidth(200)))
            {
                ResetSearch();

                int insertIndex = recipeCatalogueSize;
                spRecipes.InsertArrayElementAtIndex(insertIndex);

                Recipe recipe = Recipe.CreateRecipeInstance();
                spRecipes.GetArrayElementAtIndex(insertIndex).objectReferenceValue = recipe;
                recipesData.Insert(insertIndex, new RecipeData(spRecipes.GetArrayElementAtIndex(insertIndex)));
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            if (removeIndex != -1)
            {
                recipesData[removeIndex].cachedEditor.OnDestroyRecipe();
                Object deleteRecipe = recipesData[removeIndex].cachedEditor.target;

                spRecipes.RemoveFromObjectArrayAt(removeIndex);
                recipesData.RemoveAt(removeIndex);

                string path = AssetDatabase.GetAssetPath(deleteRecipe);
                DestroyImmediate(deleteRecipe, true);
                AssetDatabase.ImportAsset(path);
            }

            EditorSortableList.SwapIndexes swapIndexes = editorSortableListRecipes.GetSortIndexes();
            if (swapIndexes != null)
            {
                spRecipes.MoveArrayElement(swapIndexes.src, swapIndexes.dst);

                RecipeData tempRecipt = recipesData[swapIndexes.src];
                recipesData[swapIndexes.src] = recipesData[swapIndexes.dst];
                recipesData[swapIndexes.dst] = tempRecipt;
            }
        }

        private void PaintSettings()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField("User Interface", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(spMerchantUIPrefab, GC_MERCHANT);
            EditorGUILayout.PropertyField(spContainerUIPrefab, GC_CONTAINER);
            EditorGUILayout.PropertyField(spInventoryUIPrefab, GC_INVENTORY);
            EditorGUILayout.PropertyField(spItemOnDragGrabItem);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spItemCursorDrag);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(spItemCursorDragHotspot.displayName);
            EditorGUILayout.PropertyField(spItemCursorDragHotspot, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Behavior Configuration", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(spItemDragToCombine);
            EditorGUILayout.PropertyField(spInventoryStopTime);
            EditorGUILayout.PropertyField(spSaveInventory);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spCanDropItems);
            EditorGUI.BeginDisabledGroup(!spCanDropItems.boolValue);
            EditorGUILayout.PropertyField(spDropMaxDistance);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spLimitWeight);
            EditorGUI.indentLevel++;
            EditorGUI.BeginDisabledGroup(!spLimitWeight.boolValue);
            EditorGUILayout.PropertyField(spMaxWeight);
            EditorGUI.EndDisabledGroup();
            EditorGUI.indentLevel++;

            EditorGUILayout.EndVertical();
        }

        // PRIVATE METHODS: --------------------------------------------------------------------------------------------

        private void PaintSearch()
        {
            if (searchFieldStyle == null) searchFieldStyle = new GUIStyle(GUI.skin.FindStyle("SearchTextField"));
            if (searchCloseOnStyle == null) searchCloseOnStyle = new GUIStyle(GUI.skin.FindStyle("SearchCancelButton"));
            if (searchCloseOffStyle == null)
                searchCloseOffStyle = new GUIStyle(GUI.skin.FindStyle("SearchCancelButtonEmpty"));

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(5f);

            GUI.SetNextControlName(SEARCHBOX_NAME);
            searchText = EditorGUILayout.TextField(searchText, searchFieldStyle);

            if (searchFocus)
            {
                EditorGUI.FocusTextInControl(SEARCHBOX_NAME);
                searchFocus = false;
            }

            GUIStyle style = string.IsNullOrEmpty(searchText)
                ? searchCloseOffStyle
                : searchCloseOnStyle;

            if (GUILayout.Button("", style))
            {
                ResetSearch();
            }

            GUILayout.Space(5f);
            EditorGUILayout.EndHorizontal();
        }

        private void ResetSearch()
        {
            searchText = "";
            GUIUtility.keyboardControl = 0;
            GUIUtility.keyboardControl = 0;
            searchFocus = true;
        }
    }
}