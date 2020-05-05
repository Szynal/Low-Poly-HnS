using LowPolyHnS.Core.Hooks;
using UnityEngine;
using UnityEngine.UI;

namespace LowPolyHnS.Inventory
{
    public class MerchantUIItemPlayer : IMerchantUIItem
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        private Item item;

        public GameObject wrapAmount;
        public Text textAmount;

        // CONSTRUCTOR & UPDATER: -----------------------------------------------------------------

        public override void Setup(MerchantUIManager merchantUIManager, params object[] parameters)
        {
            base.Setup(merchantUIManager, parameters);
            item = parameters[0] as Item;

            UpdateUI();
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public override void UpdateUI()
        {
            if (item == null) return;

            if (merchantUIManager.currentMerchant == null) return;
            Merchant merchant = merchantUIManager.currentMerchant;

            if (image != null && item.sprite != null) image.sprite = item.sprite;
            if (textName != null) textName.text = item.itemName.GetText();
            if (textDescription != null) textDescription.text = item.itemDescription.GetText();

            GameObject player = HookPlayer.Instance != null ? HookPlayer.Instance.gameObject : null;
            float percent = merchant.sellPercent.GetValue(player);
            int price = Mathf.FloorToInt(item.price * percent);

            if (textPrice != null) textPrice.text = price.ToString();

            int curAmount = InventoryManager.Instance.GetInventoryAmountOfItem(item.uuid);
            if (wrapAmount != null)
            {
                wrapAmount.SetActive(curAmount != 1);
                if (textAmount != null) textAmount.text = curAmount.ToString();
            }
        }

        public override void OnClickButton()
        {
            Merchant merchant = merchantUIManager.currentMerchant;
            if (MerchantManager.Instance.SellToMerchant(merchant, item, 1))
            {
                UpdateUI();
                if (merchantUIManager.onSell != null)
                {
                    merchantUIManager.onSell.Invoke(item.uuid);
                }
            }
            else
            {
                if (merchantUIManager.onCantSell != null)
                {
                    merchantUIManager.onCantSell.Invoke(item.uuid);
                }
            }
        }
    }
}