using LowPolyHnS.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace LowPolyHnS.Crafting
{
    public abstract class ICraftingUIItem : MonoBehaviour
    {
        protected static DatabaseInventory DATABASE_INVENTORY;

        #region PROPERTIES

        protected CraftingUIManager CraftingUIManager;

        public Image Image;
        public Text TextName;
        public Text TextDescription;
        public Text TextPrice;

        #endregion


        #region CONSTRUCTOR & UPDATER

        public virtual void Setup(CraftingUIManager craftingUIManager, params object[] parameters)
        {
            CraftingUIManager = craftingUIManager;
        }

        #endregion

    }
}