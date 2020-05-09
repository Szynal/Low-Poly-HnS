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

            UpdateUI();
        }

        #endregion

        #region PUBLIC METHODS

        public override void UpdateUI()
        {
            if (item == null) return;

            if (Image != null && item.sprite != null) Image.sprite = item.sprite;
            if (TextName != null) TextName.text = item.itemName.GetText();
            if (TextDescription != null) TextDescription.text = item.itemDescription.GetText();

            GameObject player = HookPlayer.Instance != null ? HookPlayer.Instance.gameObject : null;
        }

        #endregion

        public override void OnClickButton()
        {
            throw new NotImplementedException();
        }
    }
}