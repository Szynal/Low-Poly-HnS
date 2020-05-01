using UnityEditor;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
    [CustomPropertyDrawer(typeof(InventorySingleItemTypeAttribute))]
    public class InventorySingleItemTypeAttributePD : PropertyDrawer
    {
        private static DatabaseInventory INVENTORY;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (INVENTORY == null)
            {
                INVENTORY = DatabaseInventory.Load();
            }

            string[] ids = INVENTORY.GetItemTypesIDs();
            int[] values = new int[ids.Length];
            for (int i = 0; i < values.Length; ++i) values[i] = i;

            property.intValue = EditorGUI.IntPopup(
                position,
                label.text,
                property.intValue,
                ids,
                values
            );
        }
    }
}