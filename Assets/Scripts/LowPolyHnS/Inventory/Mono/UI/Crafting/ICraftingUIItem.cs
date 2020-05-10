using LowPolyHnS.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace LowPolyHnS.Crafting
{
    public abstract class CraftingUIItem : MonoBehaviour
    {
        #region PROPERTIES

        protected CraftingUIManager CraftingUIManager;

        public Image Image = null;
        public Text TextName = null;
        public Text TextDescription = null;

        #endregion


        #region CONSTRUCTOR & UPDATER

        public virtual void Setup(CraftingUIManager craftingUIManager, params object[] parameters)
        {
            CraftingUIManager = craftingUIManager;
        }

        #endregion


        #region PUBLIC METHODS

        public abstract void OnClickButton();

        #endregion
    }
}