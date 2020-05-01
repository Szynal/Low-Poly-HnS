using System;

namespace LowPolyHnS.Inventory
{
    [Serializable]
    public class ItemHolder
    {
        public Item item;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public override string ToString()
        {
            if (item == null) return "(none)";
            return item.itemName.content;
        }
    }
}