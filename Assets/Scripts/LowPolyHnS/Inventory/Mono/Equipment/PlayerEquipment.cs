using UnityEngine;

namespace LowPolyHnS.Inventory
{
    [DisallowMultipleComponent]
    [AddComponentMenu("LowPolyHnS/Characters/Player Equipment")]
    public class PlayerEquipment : CharacterEquipment
    {
        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override bool CanEquip(int itemID)
        {
            int amountEquipped = HasEquip(itemID);
            int amountInventory = InventoryManager.Instance.GetInventoryAmountOfItem(itemID);

            return amountEquipped < amountInventory;
        }

        protected override string GetUniqueID()
        {
            return "player";
        }
    }
}