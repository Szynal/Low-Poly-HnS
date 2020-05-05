using LowPolyHnS.Core.Hooks;
using UnityEngine;
using UnityEngine.UI;

namespace LowPolyHnS.Inventory
{
    public class MerchantUIItemSeller : IMerchantUIItem
    {
        private Merchant.Ware ware;

        [Space] public CanvasGroup canvasGroup;

        [Space] public GameObject wrapAmount;
        public Text textCurrentAmount;
        public Text textMaxAmount;

        // CONSTRUCTOR & UPDATER: -----------------------------------------------------------------

        public override void Setup(MerchantUIManager merchantUIManager, params object[] parameters)
        {
            base.Setup(merchantUIManager, parameters);
            ware = parameters[0] as Merchant.Ware;

            UpdateUI();
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public override void UpdateUI()
        {
            if (ware == null) return;
            Item item = ware.item.item;
            Merchant merchant = merchantUIManager.currentMerchant;

            int curAmount = MerchantManager.Instance.GetMerchantAmount(
                merchant,
                ware.item.item
            );

            if (image != null && item.sprite != null) image.sprite = item.sprite;
            if (textName != null) textName.text = item.itemName.GetText();
            if (textDescription != null) textDescription.text = item.itemDescription.GetText();

            GameObject player = HookPlayer.Instance != null ? HookPlayer.Instance.gameObject : null;
            float percent = merchant.purchasePercent.GetValue(player);
            int price = Mathf.FloorToInt(item.price * percent);

            if (textPrice != null) textPrice.text = price.ToString();

            wrapAmount.SetActive(ware.limitAmount);

            if (textCurrentAmount != null)
            {
                textCurrentAmount.text = curAmount.ToString();
            }

            if (textMaxAmount != null)
            {
                textMaxAmount.text = ware.maxAmount.ToString();
            }

            canvasGroup.interactable = InventoryManager.Instance.GetCurrency() >= item.price &&
                                       (!ware.limitAmount || curAmount > 0);
        }

        public override void OnClickButton()
        {
            Merchant merchant = merchantUIManager.currentMerchant;
            if (MerchantManager.Instance.BuyFromMerchant(merchant, ware.item.item, 1))
            {
                UpdateUI();
                if (merchantUIManager.onBuy != null)
                {
                    merchantUIManager.onBuy.Invoke(ware.item.item.uuid);
                }
            }
            else
            {
                if (merchantUIManager.onCantBuy != null)
                {
                    merchantUIManager.onCantBuy.Invoke(ware.item.item.uuid);
                }
            }
        }
    }
}