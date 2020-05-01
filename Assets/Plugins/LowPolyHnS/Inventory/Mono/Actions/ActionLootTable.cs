using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionLootTable : IAction
    {
        public enum Target
        {
            PlayerInventory,
            Container
        }

        public LootTable lootTable;

        public Target target = Target.PlayerInventory;
        public TargetGameObject container = new TargetGameObject(TargetGameObject.Target.Invoker);

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (lootTable == null) return true;
            LootTable.LootResult loot = lootTable.Get();

            if (loot.item != null && loot.amount > 0)
            {
                switch (this.target)
                {
                    case Target.PlayerInventory:
                        InventoryManager.Instance.AddItemToInventory(
                            loot.item.uuid,
                            loot.amount
                        );
                        break;

                    case Target.Container:
                        Container containerInstance = container.GetComponent<Container>(target);
                        if (containerInstance != null)
                        {
                            containerInstance.AddItem(
                                loot.item.uuid,
                                loot.amount
                            );
                        }

                        break;
                }
            }

            return true;
        }

#if UNITY_EDITOR

        public static new string NAME = "Inventory/Use Loot Table";
        public const string CUSTOM_ICON_PATH = "Assets/Plugins/LowPolyHnS/Inventory/Icons/Actions/";

        private const string NODE_TITLE = "Use {0}";

        private SerializedProperty spLootTable;
        private SerializedProperty spTarget;
        private SerializedProperty spContainer;

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                lootTable == null ? "(none)" : lootTable.name
            );
        }

        protected override void OnEnableEditorChild()
        {
            spLootTable = serializedObject.FindProperty("lootTable");
            spTarget = serializedObject.FindProperty("target");
            spContainer = serializedObject.FindProperty("container");
        }

        protected override void OnDisableEditorChild()
        {
            spLootTable = null;
            spTarget = null;
            spContainer = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spLootTable);
            EditorGUILayout.PropertyField(spTarget);

            if (spTarget.enumValueIndex == (int) Target.Container)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(spContainer);
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}