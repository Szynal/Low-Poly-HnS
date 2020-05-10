using System;
using LowPolyHnS.Core.Hooks;
using LowPolyHnS.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace LowPolyHnS.Crafting
{
    public class CraftingUIItemPlayer : CraftingUIItem
    {
        #region PROPERTIES

        private Item item;

        public GameObject WrapAmount = null;
        public Text TextAmount = null;

        #endregion

        #region CONSTRUCTOR & UPDATER

        public override void Setup(CraftingUIManager craftingUIManager, params object[] parameters)
        {
            base.Setup(craftingUIManager, parameters);
            item = parameters[0] as Item;

        }

        #endregion

        #region PUBLIC METHODS

        #endregion

        public override void OnClickButton()
        {
            throw new NotImplementedException();
        }
    }
}