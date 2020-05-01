using System.IO;
using LowPolyHnS.Core;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
    [CustomEditor(typeof(Recipe))]
    public class RecipeEditor : Editor
    {
        private const float ITEM_SIZE = 50f;
        private const float ANIM_BOOL_SPEED = 3.0f;
        private const string NAME_FORMAT = "{0} x {1} + {2} x {3}";

        private const string PROP_ITEM_1 = "itemToCombineA";
        private const string PROP_AMOU_1 = "amountA";
        private const string PROP_ITEM_2 = "itemToCombineB";
        private const string PROP_AMOU_2 = "amountB";
        private const string PROP_ONCRAFT = "removeItemsOnCraft";
        private const string PROP_ACTION = "actionsList";

        private const string PATH_PREFAB_RECIPES = "Assets/Plugins/LowPolyHnSData/Inventory/Prefabs/Recipes/";
        private const string NAME_PREFAB_RECIPES = "recipe.prefab";

        // PROPERTIES: -------------------------------------------------------------------------------------------------

        private SerializedProperty spItemHolderToCombineA;
        private SerializedProperty spItemToCombineA;
        private SerializedProperty spAmountA;
        private SerializedProperty spItemHolderToCombineB;
        private SerializedProperty spItemToCombineB;
        private SerializedProperty spAmountB;

        private SerializedProperty spRemoveItemsOnCraft;
        private SerializedProperty spActionsList;
        private IActionsListEditor actionsListEditor;

        private AnimBool animUnfold;
        private GUIContent guiContentAmount = new GUIContent("Amount");

        // METHODS: ----------------------------------------------------------------------------------------------------

        private void OnEnable()
        {
            spItemHolderToCombineA = serializedObject.FindProperty(PROP_ITEM_1);
            spItemToCombineA = spItemHolderToCombineA.FindPropertyRelative("item");
            spAmountA = serializedObject.FindProperty(PROP_AMOU_1);
            spItemHolderToCombineB = serializedObject.FindProperty(PROP_ITEM_2);
            spItemToCombineB = spItemHolderToCombineB.FindPropertyRelative("item");
            spAmountB = serializedObject.FindProperty(PROP_AMOU_2);

            spRemoveItemsOnCraft = serializedObject.FindProperty(PROP_ONCRAFT);
            spActionsList = serializedObject.FindProperty(PROP_ACTION);
            if (spActionsList.objectReferenceValue == null)
            {
                LowPolyHnSUtilities.CreateFolderStructure(PATH_PREFAB_RECIPES);
                string actionsPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(
                    PATH_PREFAB_RECIPES, NAME_PREFAB_RECIPES)
                );

                GameObject sceneInstance = new GameObject("RecipeActions");
                sceneInstance.AddComponent<Actions>();

                GameObject prefabInstance = PrefabUtility.SaveAsPrefabAsset(sceneInstance, actionsPath);
                DestroyImmediate(sceneInstance);

                Actions prefabActions = prefabInstance.GetComponent<Actions>();
                prefabActions.destroyAfterFinishing = true;
                spActionsList.objectReferenceValue = prefabActions.actionsList;
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
                serializedObject.Update();
            }

            actionsListEditor = (IActionsListEditor) CreateEditor(
                spActionsList.objectReferenceValue, typeof(IActionsListEditor)
            );

            animUnfold = new AnimBool(false);
            animUnfold.speed = ANIM_BOOL_SPEED;
            animUnfold.valueChanged.AddListener(Repaint);
        }

        public void OnDestroyRecipe()
        {
            if (spActionsList.objectReferenceValue != null)
            {
                IActionsList list = (IActionsList) spActionsList.objectReferenceValue;
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(list.gameObject));
                AssetDatabase.SaveAssets();
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox(
                "This Recipe can only be edited in the Inventory section of the Preferences window",
                MessageType.Info
            );

            if (GUILayout.Button("Open Preferences"))
            {
                PreferencesWindow.OpenWindow();
            }
        }

        public bool OnPreferencesWindowGUI(DatabaseInventoryEditor inventoryEditor, int index)
        {
            serializedObject.Update();

            bool result = PaintHeader(inventoryEditor, index);
            using (var group = new EditorGUILayout.FadeGroupScope(animUnfold.faded))
            {
                if (group.visible)
                {
                    EditorGUILayout.BeginVertical(CoreGUIStyles.GetBoxExpanded());
                    PaintContent();
                    EditorGUILayout.EndVertical();
                }
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            return result;
        }

        private bool PaintHeader(DatabaseInventoryEditor inventoryEditor, int index)
        {
            bool removeItem = false;

            EditorGUILayout.BeginHorizontal();

            bool forceSortRepaint = false;
            if (inventoryEditor.recipesHandleRect.ContainsKey(index))
            {
                EditorGUIUtility.AddCursorRect(inventoryEditor.recipesHandleRect[index], MouseCursor.Pan);
                forceSortRepaint = inventoryEditor.editorSortableListRecipes.CaptureSortEvents(
                    inventoryEditor.recipesHandleRect[index], index
                );
            }

            if (forceSortRepaint) inventoryEditor.Repaint();

            GUILayout.Label("=", CoreGUIStyles.GetButtonLeft(), GUILayout.Width(25f));
            if (Event.current.type == EventType.Repaint)
            {
                Rect dragRect = GUILayoutUtility.GetLastRect();
                if (inventoryEditor.recipesHandleRect.ContainsKey(index))
                {
                    inventoryEditor.recipesHandleRect[index] = dragRect;
                }
                else
                {
                    inventoryEditor.recipesHandleRect.Add(index, dragRect);
                }
            }

            if (inventoryEditor.recipesHandleRectRow.ContainsKey(index))
            {
                inventoryEditor.editorSortableListRecipes.PaintDropPoints(
                    inventoryEditor.recipesHandleRectRow[index],
                    index,
                    inventoryEditor.spRecipes.arraySize
                );
            }

            string name = (animUnfold.target ? "▾ " : "▸ ") + GetHeaderName();
            GUIStyle style = animUnfold.target
                ? CoreGUIStyles.GetToggleButtonMidOn()
                : CoreGUIStyles.GetToggleButtonMidOff();

            if (GUILayout.Button(name, style))
            {
                animUnfold.target = !animUnfold.value;
            }

            if (GUILayout.Button("×", CoreGUIStyles.GetButtonRight(), GUILayout.Width(25)))
            {
                removeItem = true;
            }

            EditorGUILayout.EndHorizontal();
            if (Event.current.type == EventType.Repaint)
            {
                Rect rect = GUILayoutUtility.GetLastRect();
                if (inventoryEditor.recipesHandleRectRow.ContainsKey(index))
                {
                    inventoryEditor.recipesHandleRectRow[index] = rect;
                }
                else
                {
                    inventoryEditor.recipesHandleRectRow.Add(index, rect);
                }
            }

            return removeItem;
        }

        private void PaintContent()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.PropertyField(spItemHolderToCombineA);
            EditorGUILayout.PropertyField(spAmountA, guiContentAmount);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(spItemHolderToCombineB);
            EditorGUILayout.PropertyField(spAmountB, guiContentAmount);
            EditorGUILayout.EndVertical();

            spAmountA.intValue = Mathf.Max(0, spAmountA.intValue);
            spAmountB.intValue = Mathf.Max(0, spAmountB.intValue);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.PropertyField(spRemoveItemsOnCraft);
            if (actionsListEditor != null) actionsListEditor.OnInspectorGUI();
            EditorGUILayout.EndVertical();
        }

        // PRIVATE METHODS: --------------------------------------------------------------------------------------------

        private string GetHeaderName()
        {
            bool existsA = spItemToCombineA != null && spItemToCombineA.objectReferenceValue != null;
            bool existsB = spItemToCombineB != null && spItemToCombineB.objectReferenceValue != null;

            return string.Format(
                NAME_FORMAT,
                spAmountA.intValue.ToString(),
                existsA ? ((Item) spItemToCombineA.objectReferenceValue).itemName.content : "(none)",
                spAmountB.intValue.ToString(),
                existsB ? ((Item) spItemToCombineB.objectReferenceValue).itemName.content : "(none)"
            );
        }
    }
}