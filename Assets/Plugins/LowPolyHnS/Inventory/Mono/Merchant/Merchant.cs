using System;
using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
    [CreateAssetMenu(fileName = "New Merchant", menuName = "LowPolyHnS/Inventory/Merchant")]
    public class Merchant : ScriptableObject
    {
        [Serializable]
        public class Ware
        {
            public ItemHolder item = new ItemHolder();
            public bool limitAmount = false;
            public int maxAmount = 10;
        }

        [Serializable]
        public class Warehouse
        {
            public Ware[] wares = new Ware[0];
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public string uuid = "";
        public string title = "";
        public string description = "";

        [Space] public GameObject merchantUI;
        public Warehouse warehouse;

        [Space] public NumberProperty purchasePercent = new NumberProperty(1.0f);
        public NumberProperty sellPercent = new NumberProperty(0.8f);
    }
}