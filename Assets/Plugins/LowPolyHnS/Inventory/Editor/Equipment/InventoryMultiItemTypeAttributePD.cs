using UnityEditor;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
    [CustomPropertyDrawer(typeof(InventoryMultiItemTypeAttribute))]
    public class InventoryMultiItemTypeAttributePD : PropertyDrawer
    {
        private static DatabaseInventory INVENTORY;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (INVENTORY == null)
            {
                INVENTORY = DatabaseInventory.Load();
            }

            property.intValue = EditorGUI.MaskField(
                position,
                label,
                property.intValue,
                INVENTORY.GetItemTypesIDs()
            );
        }
    }
}