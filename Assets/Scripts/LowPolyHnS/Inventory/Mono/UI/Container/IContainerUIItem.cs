using UnityEngine;
using UnityEngine.UI;

namespace LowPolyHnS.Inventory
{
    public abstract class IContainerUIItem : MonoBehaviour
    {
        protected static DatabaseInventory DATABASE_INVENTORY;

        // PROPERTIES: ----------------------------------------------------------------------------

        protected ContainerUIManager containerUIManager;
        protected Item item = null;

        public Image image;
        public Text textName;
        public Text textDescription;

        [Space] public GameObject wrapAmount;
        public Text textAmount;

        // CONSTRUCTOR & UPDATER: -----------------------------------------------------------------

        public virtual void Setup(ContainerUIManager containerUIManager, object item)
        {
            this.containerUIManager = containerUIManager;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void UpdateUI()
        {
            if (item == null) return;

            if (image != null && item.sprite != null) image.sprite = item.sprite;
            if (textName != null) textName.text = item.itemName.GetText();
            if (textDescription != null) textDescription.text = item.itemDescription.GetText();

            int amount = GetAmount();
            if (wrapAmount != null)
            {
                wrapAmount.SetActive(amount != 1);
                if (textAmount != null) textAmount.text = amount.ToString();
            }
        }

        public abstract void OnClickButton();
        protected abstract int GetAmount();
    }
}