using System.Collections.Generic;
using LowPolyHnS.Core;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
    [CustomEditor(typeof(Container))]
    public class ContainerEditor : Editor
    {
        private const string GCTOOLBAR_ICON_PATH = "Assets/Scripts/LowPolyHnS/Inventory/Icons/Toolbar/Container.png";
        private static readonly GUIContent GC_CONT_UI = new GUIContent("Container UI (optional)");

        // PROPERTIES: ----------------------------------------------------------------------------

        private Container container;

        private SerializedProperty spInitItems;
        private ReorderableList initItemsList;

        private SerializedProperty spSaveContainer;
        private SerializedProperty spAnimatorContainer;
        private SerializedProperty spContainerUI;

        // INIT METHODS: --------------------------------------------------------------------------

        private void OnEnable()
        {
            container = target as Container;

            spInitItems = serializedObject.FindProperty("initItems");
            spSaveContainer = serializedObject.FindProperty("saveContainer");
            spAnimatorContainer = serializedObject.FindProperty("Animator");
            spContainerUI = serializedObject.FindProperty("containerUI");

            initItemsList = new ReorderableList(
                serializedObject,
                spInitItems,
                true, true, true, true
            );

            initItemsList.drawHeaderCallback = InitContainer_Header;
            initItemsList.drawElementCallback = InitContainer_Paint;
            initItemsList.elementHeightCallback = InitContainer_Height;
        }


        // PAINT METHODS: -------------------------------------------------------------------------

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.Space();

            if (Application.isPlaying) PaintRuntime();
            else initItemsList.DoLayoutList();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spContainerUI, GC_CONT_UI);
            EditorGUILayout.PropertyField(spAnimatorContainer);
            EditorGUILayout.PropertyField(spSaveContainer);

            GlobalEditorID.Paint(container);

            serializedObject.ApplyModifiedProperties();
        }

        private void PaintRuntime()
        {
            foreach (KeyValuePair<int, Container.ItemData> element in container.data.items)
            {
                int uuid = element.Value.uuid;
                if (!InventoryManager.Instance.itemsCatalogue.ContainsKey(uuid)) continue;

                Item item = InventoryManager.Instance.itemsCatalogue[uuid];
                string itemName = item.itemName.content;

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField(
                    itemName,
                    element.Value.amount.ToString(),
                    EditorStyles.boldLabel
                );
                EditorGUILayout.EndVertical();
            }
        }

        // INIT ITEMS METHODS: --------------------------------------------------------------------

        private void InitContainer_Header(Rect rect)
        {
            EditorGUI.LabelField(rect, "Items");
        }

        private void InitContainer_Paint(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty spProperty = spInitItems.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, spProperty, true);
        }

        private float InitContainer_Height(int index)
        {
            return EditorGUI.GetPropertyHeight(spInitItems.GetArrayElementAtIndex(index)) +
                   EditorGUIUtility.standardVerticalSpacing;
        }

        // HIERARCHY CONTEXT MENU: ----------------------------------------------------------------

        [MenuItem("GameObject/LowPolyHnS/Inventory/Container", false, 0)]
        public static void CreateContainer()
        {
            GameObject container = CreateSceneObject.Create("Container");
            container.AddComponent<Container>();
        }
    }
}