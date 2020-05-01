using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionInventoryInstantiate : IAction
    {
        public ItemHolder item;
        public TargetPosition target = new TargetPosition();

        // EXECUTABLE: -------------------------------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (item != null && item.item.prefab != null)
            {
                Vector3 position = this.target.GetPosition(target);
                Quaternion rotation = Quaternion.identity;
                Instantiate(item.item.prefab, position, rotation);
            }

            return true;
        }

        // +-----------------------------------------------------------------------------------------------------------+
        // | EDITOR                                                                                                    |
        // +-----------------------------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public const string CUSTOM_ICON_PATH = "Assets/Plugins/LowPolyHnS/Inventory/Icons/Actions/";

        public static new string NAME = "Inventory/Instantiate Item";
        private const string NODE_TITLE = "Instantiate item {0}";

        // PROPERTIES: -------------------------------------------------------------------------------------------------

        private SerializedProperty spItem;
        private SerializedProperty spTarget;

        // INSPECTOR METHODS: ------------------------------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                item == null || item.item == null ? "(none)" : item.item.itemName.content
            );
        }

        protected override void OnEnableEditorChild()
        {
            spItem = serializedObject.FindProperty("item");
            spTarget = serializedObject.FindProperty("target");
        }

        protected override void OnDisableEditorChild()
        {
            spItem = null;
            spTarget = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spItem);
            EditorGUILayout.PropertyField(spTarget);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}