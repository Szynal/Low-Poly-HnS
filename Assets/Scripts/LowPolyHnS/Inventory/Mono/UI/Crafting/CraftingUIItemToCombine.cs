using System;
using LowPolyHnS.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace LowPolyHnS.Crafting
{
    public class CraftingUIItemToCombine : CraftingUIItem
    {
        #region PROPERTIES

        private Item item;
        public GameObject WrapAmount = null;
        public Text TextAmount = null;

        #endregion

        #region PUBLIC METHODS

        #endregion

        public override void ShowCraftTable()
        {
            throw new NotImplementedException();
        }
    }
}