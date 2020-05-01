using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionUnequip : IAction
    {
        public enum Operation
        {
            Type,
            Item
        }

        public TargetGameObject character = new TargetGameObject(TargetGameObject.Target.Player);

        public Operation unequip = Operation.Type;
        [InventoryMultiItemType] public int types = 0;
        public ItemHolder item = new ItemHolder();

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            GameObject targetObject = character.GetGameObject(target);
            if (target == null)
            {
                Debug.LogError("No target found in Action: Unequip");
                return true;
            }

            switch (unequip)
            {
                case Operation.Item:
                    if (item.item == null)
                    {
                        Debug.LogError("Item not defined in Action: Unequip");
                        return true;
                    }

                    InventoryManager.Instance.Unequip(
                        targetObject,
                        item.item.uuid
                    );
                    break;

                case Operation.Type:
                    InventoryManager.Instance.UnequipTypes(
                        targetObject,
                        types
                    );
                    break;
            }

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public const string CUSTOM_ICON_PATH = "Assets/Plugins/LowPolyHnS/Inventory/Icons/Actions/";

        public static new string NAME = "Inventory/Unequip Item";
        private const string NODE_TITLE = "Unequip {0} from {1}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spCharacter;
        private SerializedProperty spUnequip;
        private SerializedProperty spItem;
        private SerializedProperty spTypes;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                unequip,
                character
            );
        }

        protected override void OnEnableEditorChild()
        {
            spCharacter = serializedObject.FindProperty("character");
            spUnequip = serializedObject.FindProperty("unequip");
            spItem = serializedObject.FindProperty("item");
            spTypes = serializedObject.FindProperty("types");
        }

        protected override void OnDisableEditorChild()
        {
            spCharacter = null;
            spUnequip = null;
            spItem = null;
            spTypes = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spCharacter);
            EditorGUILayout.PropertyField(spUnequip);

            EditorGUI.indentLevel++;
            switch (spUnequip.intValue)
            {
                case (int) Operation.Item:
                    EditorGUILayout.PropertyField(spItem);
                    break;

                case (int) Operation.Type:
                    EditorGUILayout.PropertyField(spTypes);
                    break;
            }

            EditorGUI.indentLevel--;

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}