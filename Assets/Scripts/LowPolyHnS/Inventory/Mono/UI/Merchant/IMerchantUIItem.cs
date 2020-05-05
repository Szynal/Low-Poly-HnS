using UnityEngine;
using UnityEngine.UI;

namespace LowPolyHnS.Inventory
{
    public abstract class IMerchantUIItem : MonoBehaviour
    {
        protected static DatabaseInventory DATABASE_INVENTORY;

        // PROPERTIES: ----------------------------------------------------------------------------

        protected MerchantUIManager merchantUIManager;

        public Image image;
        public Text textName;
        public Text textDescription;
        public Text textPrice;

        // CONSTRUCTOR & UPDATER: -----------------------------------------------------------------

        public virtual void Setup(MerchantUIManager merchantUIManager, params object[] parameters)
        {
            this.merchantUIManager = merchantUIManager;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public abstract void UpdateUI();
        public abstract void OnClickButton();
    }
}