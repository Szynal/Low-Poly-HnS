using System.IO;
using LowPolyHnS.Core;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
    [CustomEditor(typeof(Item))]
    public class ItemEditor : Editor
    {
        private const float ANIM_BOOL_SPEED = 3.0f;
        private const float BTN_HEIGHT = 24f;

        private const string PATH_PREFAB_CONSUME = "Assets/Scripts/LowPolyHnSData/Inventory/Prefabs/Consumables/";
        private const string NAME_PREFAB_CONSUME = "consume.prefab";

        private const string PATH_PREFAB_EQUIP_COND = "Assets/Scripts/LowPolyHnSData/Inventory/Prefabs/Conditions/";
        private const string NAME_PREFAB_EQUIP_COND = "equip.prefab";

        private const string PATH_PREFAB_EQUP = "Assets/Scripts/LowPolyHnSData/Inventory/Prefabs/Equip/";
        private const string NAME_PREFAB_EQUIP = "equip.prefab";
        private const string NAME_PREFAB_UNEQUIP = "unequip.prefab";

        private static readonly GUIContent[] EQUIPABLE_OPTIONS =
        {
            new GUIContent("Conditions"),
            new GUIContent("On Equip"),
            new GUIContent("On Unequip")
        };

        private static readonly string[] EQUIP_DESC =
        {
            "Requirements that must be met in order to equip this item",
            "Actions executed when equipping this item",
            "Actions executed when un-equipping this item"
        };

        private const string PROP_UUID = "uuid";
        private const string PROP_NAME = "itemName";
        private const string PROP_DESCRIPTION = "itemDescription";

        private const string PROP_STRENGTH_BONUS = "StrengthBonus";
        private const string PROP_AGILITY_BONUS = "AgilityBonus";
        private const string PROP_INTELLIGENCE_BONUS = "IntelligenceBonus";
        private const string PROP_STRENGTH_PERCENT_BONUS = "StrengthPercentBonus";
        private const string PROP_AGILITY_PERCENT_BONUS = "AgilityPercentBonus";
        private const string PROP_INTELLIGENCE_PERCENT_BONUS = "IntelligencePercentBonus";

        private const string PROP_FIRE_RESISTANCE_BONUS = "FireResistanceBonus";
        private const string PROP_COLD_RESISTANCE_BONUS = "ColdResistanceBonus";
        private const string PROP_POISON_RESISTANCE_BONUS = "PoisonResistanceBonus";

        private const string PROP_COLOR = "itemColor";
        private const string PROP_SPRITE = "sprite";
        private const string PROP_PREFAB = "prefab";
        private const string PROP_PRICE = "price";
        private const string PROP_CANBESOLD = "canBeSold";
        private const string PROP_MAXSTACK = "maxStack";
        private const string PROP_WEIGHT = "weight";
        private const string PROP_ITEMTYPES = "itemTypes";

        private const string PROP_ONCLICK = "onClick";
        private const string PROP_CONSUMEITEM = "consumeItem";
        private const string PROP_ACTIONONCLICK = "actionsOnClick";

        private const string PROP_EQUIPABLE = "equipable";
        private const string PROP_FILLALLTYPES = "fillAllTypes";
        private const string PROP_CONDITIONSEQUIP = "conditionsEquip";
        private const string PROP_ACTIONSEQUIP = "actionsOnEquip";
        private const string PROP_ACTIONSUNEQUIP = "actionsOnUnequip";

        public class ItemReturnOperation
        {
            public bool removeIndex;
            public bool duplicateIndex;
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public SerializedProperty spUUID;
        private SerializedProperty spName;
        private SerializedProperty spDescription;

        private SerializedProperty spStrengthBonus;
        private SerializedProperty spAgilityBonus;
        private SerializedProperty spIntelligenceBonus;
        private SerializedProperty spStrengthPercentBonus;
        private SerializedProperty spAgilityPercentBonus;
        private SerializedProperty spIntelligencePercentBonus;

        private SerializedProperty spFireResistanceBonus;
        private SerializedProperty spColdResistanceBonus;
        private SerializedProperty spPoisonResistanceBonus;

        private SerializedProperty spColor;
        private SerializedProperty spSprite;
        private SerializedProperty spPrefab;
        private SerializedProperty spCanBeSold;
        private SerializedProperty spPrice;
        private SerializedProperty spMaxStack;
        private SerializedProperty spWeight;
        private SerializedProperty spItemTypes;

        private SerializedProperty spOnClick;
        private SerializedProperty spConsumeItem;
        private SerializedProperty spActionsOnClick;

        private IActionsListEditor actionsOnClick;

        private int equipToolbarIndex;
        private SerializedProperty spEquipable;
        private SerializedProperty spFillAllTypes;

        private SerializedProperty spConditionsEquip;
        private SerializedProperty spActionsOnEquip;
        private SerializedProperty spActionsOnUnequip;

        private IConditionsListEditor conditionsEquipEditor;
        private IActionsListEditor actionsOnEquipEditor;
        private IActionsListEditor actionsOnUnequipEditor;

        private AnimBool animUnfold;

        public void OnEnable()
        {
            if (target == null || serializedObject == null) return;

            spUUID = serializedObject.FindProperty(PROP_UUID);
            spName = serializedObject.FindProperty(PROP_NAME);
            spDescription = serializedObject.FindProperty(PROP_DESCRIPTION);

            spStrengthBonus = serializedObject.FindProperty(PROP_STRENGTH_BONUS);
            spAgilityBonus = serializedObject.FindProperty(PROP_AGILITY_BONUS);
            spIntelligenceBonus = serializedObject.FindProperty(PROP_INTELLIGENCE_BONUS);
            spStrengthPercentBonus = serializedObject.FindProperty(PROP_STRENGTH_PERCENT_BONUS);
            spAgilityPercentBonus = serializedObject.FindProperty(PROP_AGILITY_PERCENT_BONUS);
            spIntelligencePercentBonus = serializedObject.FindProperty(PROP_INTELLIGENCE_PERCENT_BONUS);

            spFireResistanceBonus = serializedObject.FindProperty(PROP_FIRE_RESISTANCE_BONUS);
            spColdResistanceBonus = serializedObject.FindProperty(PROP_COLD_RESISTANCE_BONUS);
            spPoisonResistanceBonus = serializedObject.FindProperty(PROP_POISON_RESISTANCE_BONUS);

            spColor = serializedObject.FindProperty(PROP_COLOR);
            spSprite = serializedObject.FindProperty(PROP_SPRITE);
            spPrefab = serializedObject.FindProperty(PROP_PREFAB);
            spCanBeSold = serializedObject.FindProperty(PROP_CANBESOLD);
            spPrice = serializedObject.FindProperty(PROP_PRICE);
            spMaxStack = serializedObject.FindProperty(PROP_MAXSTACK);
            spWeight = serializedObject.FindProperty(PROP_WEIGHT);
            spItemTypes = serializedObject.FindProperty(PROP_ITEMTYPES);

            spOnClick = serializedObject.FindProperty(PROP_ONCLICK);
            spConsumeItem = serializedObject.FindProperty(PROP_CONSUMEITEM);
            spActionsOnClick = serializedObject.FindProperty(PROP_ACTIONONCLICK);

            SetupActionsList(
                ref spActionsOnClick,
                ref actionsOnClick,
                PATH_PREFAB_CONSUME,
                NAME_PREFAB_CONSUME
            );

            spEquipable = serializedObject.FindProperty(PROP_EQUIPABLE);
            spFillAllTypes = serializedObject.FindProperty(PROP_FILLALLTYPES);

            spConditionsEquip = serializedObject.FindProperty(PROP_CONDITIONSEQUIP);
            spActionsOnEquip = serializedObject.FindProperty(PROP_ACTIONSEQUIP);
            spActionsOnUnequip = serializedObject.FindProperty(PROP_ACTIONSUNEQUIP);

            SetupConditionsList(
                ref spConditionsEquip,
                ref conditionsEquipEditor,
                PATH_PREFAB_EQUIP_COND,
                NAME_PREFAB_EQUIP_COND
            );

            SetupActionsList(
                ref spActionsOnEquip,
                ref actionsOnEquipEditor,
                PATH_PREFAB_EQUP,
                NAME_PREFAB_EQUIP
            );

            SetupActionsList(
                ref spActionsOnUnequip,
                ref actionsOnUnequipEditor,
                PATH_PREFAB_EQUP,
                NAME_PREFAB_UNEQUIP
            );

            animUnfold = new AnimBool(false);
            animUnfold.speed = ANIM_BOOL_SPEED;
            animUnfold.valueChanged.AddListener(Repaint);
        }

        private void SetupConditionsList(ref SerializedProperty sp, ref IConditionsListEditor editor,
            string prefabPath, string prefabName)
        {
            if (sp.objectReferenceValue == null)
            {
                LowPolyHnSUtilities.CreateFolderStructure(prefabPath);
                string conditionsPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(
                    prefabPath, prefabName
                ));

                GameObject sceneInstance = new GameObject("Conditions");
                sceneInstance.AddComponent<IConditionsList>();

                GameObject prefabInstance = PrefabUtility.SaveAsPrefabAsset(sceneInstance, conditionsPath);
                DestroyImmediate(sceneInstance);

                sp.objectReferenceValue = prefabInstance.GetComponent<IConditionsList>();
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
                serializedObject.Update();
            }

            editor = CreateEditor(
                sp.objectReferenceValue,
                typeof(IConditionsListEditor)
            ) as IConditionsListEditor;
        }

        private void SetupActionsList(ref SerializedProperty sp, ref IActionsListEditor editor,
            string prefabPath, string prefabName)
        {
            if (sp.objectReferenceValue == null)
            {
                LowPolyHnSUtilities.CreateFolderStructure(prefabPath);
                string actionsPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(
                    prefabPath, prefabName
                ));

                GameObject sceneInstance = new GameObject("Actions");
                sceneInstance.AddComponent<Actions>();

                GameObject prefabInstance = PrefabUtility.SaveAsPrefabAsset(sceneInstance, actionsPath);
                DestroyImmediate(sceneInstance);

                Actions prefabActions = prefabInstance.GetComponent<Actions>();
                prefabActions.destroyAfterFinishing = true;
                sp.objectReferenceValue = prefabActions.actionsList;
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
                serializedObject.Update();
            }

            editor = CreateEditor(
                sp.objectReferenceValue,
                typeof(IActionsListEditor)
            ) as IActionsListEditor;
        }

        public void OnDestroyItem()
        {
            if (spActionsOnClick.objectReferenceValue != null)
            {
                IActionsList list1 = (IActionsList) spActionsOnClick.objectReferenceValue;
                IActionsList list2 = (IActionsList) spActionsOnEquip.objectReferenceValue;
                IActionsList list3 = (IActionsList) spActionsOnUnequip.objectReferenceValue;
                IConditionsList cond1 = (IConditionsList) spConditionsEquip.objectReferenceValue;

                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(list1.gameObject));
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(list2.gameObject));
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(list3.gameObject));
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(cond1.gameObject));
                AssetDatabase.SaveAssets();
            }
        }

        public override void OnInspectorGUI()
        {
            if (target == null || serializedObject == null) return;
            EditorGUILayout.HelpBox(
                "This Item can only be edited in the Inventory section of the Preferences window",
                MessageType.Info
            );

            if (GUILayout.Button("Open Preferences"))
            {
                PreferencesWindow.OpenWindow();
            }
        }

        public ItemReturnOperation OnPreferencesWindowGUI(DatabaseInventoryEditor inventoryEditor, int index)
        {
            serializedObject.Update();
            inventoryEditor.searchText = inventoryEditor.searchText.ToLower();
            string spNameString = spName.FindPropertyRelative("content").stringValue;
            string spDescString = spDescription.FindPropertyRelative("content").stringValue;

            if (!string.IsNullOrEmpty(inventoryEditor.searchText) &&
                !spNameString.ToLower().Contains(inventoryEditor.searchText) &&
                !spDescString.ToLower().Contains(inventoryEditor.searchText))
            {
                return new ItemReturnOperation();
            }

            ItemReturnOperation result = PaintHeader(inventoryEditor, index);
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

        private ItemReturnOperation PaintHeader(DatabaseInventoryEditor inventoryEditor, int index)
        {
            bool removeItem = false;
            bool duplicateIndex = false;

            EditorGUILayout.BeginHorizontal();

            bool forceSortRepaint = false;
            if (inventoryEditor.itemsHandleRect.ContainsKey(index))
            {
                EditorGUIUtility.AddCursorRect(inventoryEditor.itemsHandleRect[index], MouseCursor.Pan);
                forceSortRepaint = inventoryEditor.editorSortableListItems.CaptureSortEvents(
                    inventoryEditor.itemsHandleRect[index], index
                );
            }

            if (forceSortRepaint) inventoryEditor.Repaint();

            GUILayout.Label("=", CoreGUIStyles.GetButtonLeft(), GUILayout.Width(25f), GUILayout.Height(BTN_HEIGHT));
            if (Event.current.type == EventType.Repaint)
            {
                Rect dragRect = GUILayoutUtility.GetLastRect();
                if (inventoryEditor.itemsHandleRect.ContainsKey(index))
                {
                    inventoryEditor.itemsHandleRect[index] = dragRect;
                }
                else
                {
                    inventoryEditor.itemsHandleRect.Add(index, dragRect);
                }
            }

            if (inventoryEditor.itemsHandleRectRow.ContainsKey(index))
            {
                inventoryEditor.editorSortableListItems.PaintDropPoints(
                    inventoryEditor.itemsHandleRectRow[index],
                    index,
                    inventoryEditor.spItems.arraySize
                );
            }

            string name = animUnfold.target ? "▾ " : "▸ ";
            string spNameString = spName.FindPropertyRelative("content").stringValue;
            name += string.IsNullOrEmpty(spNameString) ? "No-name" : spNameString;

            GUIStyle style = animUnfold.target
                ? CoreGUIStyles.GetToggleButtonMidOn()
                : CoreGUIStyles.GetToggleButtonMidOff();

            if (GUILayout.Button(name, style, GUILayout.Height(BTN_HEIGHT)))
            {
                animUnfold.target = !animUnfold.value;
            }

            GUIContent gcDuplicate = ClausesUtilities.Get(ClausesUtilities.Icon.Duplicate);
            if (GUILayout.Button(gcDuplicate, CoreGUIStyles.GetButtonMid(), GUILayout.Width(25),
                GUILayout.Height(BTN_HEIGHT)))
            {
                duplicateIndex = true;
            }

            GUIContent gcDelete = ClausesUtilities.Get(ClausesUtilities.Icon.Delete);
            if (GUILayout.Button(gcDelete, CoreGUIStyles.GetButtonRight(), GUILayout.Width(25),
                GUILayout.Height(BTN_HEIGHT)))
            {
                removeItem = true;
            }

            EditorGUILayout.EndHorizontal();
            if (Event.current.type == EventType.Repaint)
            {
                Rect rect = GUILayoutUtility.GetLastRect();
                if (inventoryEditor.itemsHandleRectRow.ContainsKey(index))
                {
                    inventoryEditor.itemsHandleRectRow[index] = rect;
                }
                else
                {
                    inventoryEditor.itemsHandleRectRow.Add(index, rect);
                }
            }

            ItemReturnOperation result = new ItemReturnOperation();
            if (removeItem) result.removeIndex = true;
            if (duplicateIndex) result.duplicateIndex = true;

            return result;
        }

        private void PaintContent()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(spUUID);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.PropertyField(spName);
            EditorGUILayout.PropertyField(spDescription);

            EditorGUILayout.PropertyField(spStrengthBonus);
            EditorGUILayout.PropertyField(spAgilityBonus);
            EditorGUILayout.PropertyField(spIntelligenceBonus);
            EditorGUILayout.PropertyField(spStrengthPercentBonus);
            EditorGUILayout.PropertyField(spAgilityPercentBonus);
            EditorGUILayout.PropertyField(spIntelligencePercentBonus);

            EditorGUILayout.PropertyField(spFireResistanceBonus);
            EditorGUILayout.PropertyField(spColdResistanceBonus);
            EditorGUILayout.PropertyField(spPoisonResistanceBonus);

            EditorGUILayout.PropertyField(spColor);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(spSprite);
            EditorGUILayout.PropertyField(spPrefab);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(spCanBeSold);
            EditorGUI.BeginDisabledGroup(!spCanBeSold.boolValue);
            EditorGUILayout.PropertyField(spPrice);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.PropertyField(spMaxStack);
            EditorGUILayout.PropertyField(spWeight);
            EditorGUILayout.PropertyField(spItemTypes);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spOnClick);
            EditorGUI.BeginDisabledGroup(!spOnClick.boolValue);
            EditorGUILayout.PropertyField(spConsumeItem);

            if (actionsOnClick != null)
            {
                actionsOnClick.OnInspectorGUI();
            }

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spEquipable);
            EditorGUI.BeginDisabledGroup(!spEquipable.boolValue);

            EditorGUILayout.PropertyField(spFillAllTypes);
            equipToolbarIndex = GUILayout.Toolbar(equipToolbarIndex, EQUIPABLE_OPTIONS);
            EditorGUILayout.HelpBox(EQUIP_DESC[equipToolbarIndex], MessageType.Info);
            switch (equipToolbarIndex)
            {
                case 0:
                    if (conditionsEquipEditor != null) conditionsEquipEditor.OnInspectorGUI();
                    break;
                case 1:
                    if (actionsOnEquipEditor != null) actionsOnEquipEditor.OnInspectorGUI();
                    break;
                case 2:
                    if (actionsOnUnequipEditor != null) actionsOnUnequipEditor.OnInspectorGUI();
                    break;
            }

            EditorGUI.EndDisabledGroup();
        }
    }
}