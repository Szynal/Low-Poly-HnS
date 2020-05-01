using System;
using LowPolyHnS.Core;
using LowPolyHnS.Localization;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
    [Serializable]
    public class ItemType
    {
        public const int MAX = 32;

        // PROPERTIES: ----------------------------------------------------------------------------

        public string id;

        [LocStringNoPostProcess] public LocString name;

        // INITIALIZERS: --------------------------------------------------------------------------

        public ItemType()
        {
            id = "";
            name = new LocString();
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    // ITEM ATTRIBUTE: ----------------------------------------------------------------------------

    public class InventoryMultiItemTypeAttribute : PropertyAttribute
    {
    }

    public class InventorySingleItemTypeAttribute : PropertyAttribute
    {
    }
}