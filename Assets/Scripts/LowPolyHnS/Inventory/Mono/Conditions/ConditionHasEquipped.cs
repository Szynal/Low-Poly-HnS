using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ConditionHasEquipped : ICondition
    {
        public enum Operation
        {
            Type,
            Item
        }

        public TargetGameObject character = new TargetGameObject(TargetGameObject.Target.Player);
        public Operation hasEquipped = Operation.Type;
        public ItemHolder item;

        [InventoryMultiItemType] public int types = ~0;

        // EXECUTABLE: -------------------------------------------------------------------------------------------------

        public override bool Check(GameObject target)
        {
            GameObject charTarget = character.GetGameObject(target);
            if (charTarget != null)
            {
                switch (hasEquipped)
                {
                    case Operation.Type:
                        return InventoryManager.Instance.HasEquipedTypes(
                            charTarget,
                            types
                        );

                    case Operation.Item:
                        return InventoryManager.Instance.HasEquiped(
                                   charTarget,
                                   item.item.uuid
                               ) > 0;
                }
            }

            return false;
        }

        // +-----------------------------------------------------------------------------------------------------------+
        // | EDITOR                                                                                                    |
        // +-----------------------------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public const string CUSTOM_ICON_PATH = "Assets/Content/Icons/Inventory/Conditions/";

        public static new string NAME = "Inventory/Has Equipped";
        private const string NODE_TITLE = "Has {0} equipped {1}";

        // PROPERTIES: -------------------------------------------------------------------------------------------------

        private SerializedProperty spCharacter;
        private SerializedProperty spHasEquipped;
        private SerializedProperty spItem;
        private SerializedProperty spTypes;

        // INSPECTOR METHODS: ------------------------------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                character,
                hasEquipped
            );
        }

        protected override void OnEnableEditorChild()
        {
            spCharacter = serializedObject.FindProperty("character");
            spHasEquipped = serializedObject.FindProperty("hasEquipped");
            spItem = serializedObject.FindProperty("item");
            spTypes = serializedObject.FindProperty("types");
        }

        protected override void OnDisableEditorChild()
        {
            spCharacter = null;
            spHasEquipped = null;
            spItem = null;
            spTypes = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spCharacter);
            EditorGUILayout.PropertyField(spHasEquipped);

            switch (spHasEquipped.intValue)
            {
                case (int) Operation.Type:
                    EditorGUILayout.PropertyField(spTypes);
                    break;
                case (int) Operation.Item:
                    EditorGUILayout.PropertyField(spItem);
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}