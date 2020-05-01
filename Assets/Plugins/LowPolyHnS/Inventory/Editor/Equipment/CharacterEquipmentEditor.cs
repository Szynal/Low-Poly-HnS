using LowPolyHnS.Core;
using UnityEditor;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
    [CustomEditor(typeof(CharacterEquipment))]
    public class CharacterEquipmentEditor : Editor
    {
        private static DatabaseInventory DATABASE_INVENTORY;
        private CharacterEquipment equipment;

        private SerializedProperty spSaveEquipment;

        // INITIALIZERS: --------------------------------------------------------------------------

        private void OnEnable()
        {
            if (target == null || serializedObject == null) return;
            if (DATABASE_INVENTORY == null) DATABASE_INVENTORY = DatabaseInventory.Load();
            equipment = (CharacterEquipment) target;

            spSaveEquipment = serializedObject.FindProperty("saveEquipment");
        }

        public override bool RequiresConstantRepaint()
        {
            return Application.isPlaying;
        }

        // PAINT METHOD: --------------------------------------------------------------------------

        public override void OnInspectorGUI()
        {
            if (target == null || serializedObject == null) return;
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Equipment information", MessageType.Info);
            }
            else
            {
                string[] typeNames = DATABASE_INVENTORY.GetItemTypesNames();
                for (int i = 0; i < equipment.equipment.items.Length; ++i)
                {
                    if (equipment.equipment.items[i].isEquipped)
                    {
                        int uuid = equipment.equipment.items[i].itemID;
                        string item = InventoryManager.Instance.itemsCatalogue[uuid].itemName.content;
                        EditorGUILayout.LabelField(typeNames[i], item);
                    }
                }
            }

            serializedObject.Update();

            EditorGUILayout.PropertyField(spSaveEquipment);

            serializedObject.ApplyModifiedProperties();

            if (PaintGlobalID())
            {
                EditorGUILayout.Space();
                GlobalEditorID.Paint(equipment);
            }
        }

        protected virtual bool PaintGlobalID()
        {
            return true;
        }
    }
}