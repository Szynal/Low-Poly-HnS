using UnityEditor;

namespace LowPolyHnS.Inventory
{
    [CustomEditor(typeof(PlayerEquipment))]
    public class PlayerEquipmentEditor : CharacterEquipmentEditor
    {
        protected override bool PaintGlobalID()
        {
            return false;
        }
    }
}