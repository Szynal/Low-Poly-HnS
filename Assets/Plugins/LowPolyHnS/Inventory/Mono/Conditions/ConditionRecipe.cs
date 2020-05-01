using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ConditionRecipe : ICondition
    {
        public ItemHolder item1;
        public ItemHolder item2;

        // EXECUTABLE: -------------------------------------------------------------------------------------------------

        public override bool Check()
        {
            return InventoryManager.Instance.ExistsRecipe(item1.item.uuid, item2.item.uuid);
        }

        // +-----------------------------------------------------------------------------------------------------------+
        // | EDITOR                                                                                                    |
        // +-----------------------------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public const string CUSTOM_ICON_PATH = "Assets/Plugins/LowPolyHnS/Inventory/Icons/Conditions/";

        public static new string NAME = "Inventory/Exists Recipe";
        private const string NODE_TITLE = "Exists recipe {0} + {1}";

        // PROPERTIES: -------------------------------------------------------------------------------------------------

        private SerializedProperty spItem1;
        private SerializedProperty spItem2;

        // INSPECTOR METHODS: ------------------------------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                item1.item == null ? "(none)" : item1.item.itemName.content,
                item2.item == null ? "(none)" : item2.item.itemName.content
            );
        }

        protected override void OnEnableEditorChild()
        {
            spItem1 = serializedObject.FindProperty("item1");
            spItem2 = serializedObject.FindProperty("item2");
        }

        protected override void OnDisableEditorChild()
        {
            spItem1 = null;
            spItem2 = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spItem1);
            EditorGUILayout.PropertyField(spItem2);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}