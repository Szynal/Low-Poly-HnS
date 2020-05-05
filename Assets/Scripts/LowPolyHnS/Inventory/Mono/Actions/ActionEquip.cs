using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionEquip : IAction
    {
        public TargetGameObject character = new TargetGameObject(TargetGameObject.Target.Player);
        public ItemHolder item = new ItemHolder();

        [InventorySingleItemType] public int type = 0;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            GameObject targetObject = character.GetGameObject(target);
            if (target == null)
            {
                Debug.LogError("No target found in Action: Equip");
                return true;
            }

            InventoryManager.Instance.Equip(
                targetObject,
                item.item.uuid,
                type
            );

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public const string CUSTOM_ICON_PATH = "Assets/Content/Icons/Inventory/Actions/";

        public static new string NAME = "Inventory/Equip Item";
        private const string NODE_TITLE = "Equip {0} on {1}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spCharacter;
        private SerializedProperty spItem;
        private SerializedProperty spType;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                item.item == null ? "nothing" : item.item.itemName.content,
                character
            );
        }

        protected override void OnEnableEditorChild()
        {
            spCharacter = serializedObject.FindProperty("character");
            spItem = serializedObject.FindProperty("item");
            spType = serializedObject.FindProperty("type");
        }

        protected override void OnDisableEditorChild()
        {
            spCharacter = null;
            spItem = null;
            spType = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spCharacter);
            EditorGUILayout.PropertyField(spItem);
            EditorGUILayout.PropertyField(spType);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}