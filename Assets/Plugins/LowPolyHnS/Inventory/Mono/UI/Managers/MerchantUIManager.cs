using System;
using System.Collections.Generic;
using LowPolyHnS.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LowPolyHnS.Inventory
{
    public class MerchantUIManager : MonoBehaviour
    {
        private const int TIME_LAYER = 201;

        public static MerchantUIManager Instance { get; private set; }
        private static DatabaseInventory DATABASE_INVENTORY;

        private const string DEFAULT_UI_PATH = "LowPolyHnS/Inventory/MerchantUI";

        [Serializable]
        public class MerchantEvent : UnityEvent<int>
        {
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public ScrollRect scrollMerchant;
        public ScrollRect scrollPlayer;

        [Space] public Text textTitle;
        public Text textDescription;

        [InventoryMultiItemType, SerializeField]
        private int playerTypes = ~0;

        [Space] public GameObject itemUIPrefabSeller;
        public GameObject itemUIPrefabPlayer;

        [HideInInspector] public Merchant currentMerchant;

        private Animator merchantAnimator;
        private GameObject merchantRoot;
        private bool isOpen;

        [Space] public MerchantEvent onBuy = new MerchantEvent();
        public MerchantEvent onCantBuy = new MerchantEvent();

        [Space] public MerchantEvent onSell = new MerchantEvent();
        public MerchantEvent onCantSell = new MerchantEvent();

        private Dictionary<int, MerchantUIItemSeller> merchantItems;
        private Dictionary<int, MerchantUIItemPlayer> playerItems;

        // INITIALIZERS: --------------------------------------------------------------------------

        private void Awake()
        {
            Instance = this;

            merchantItems = new Dictionary<int, MerchantUIItemSeller>();
            playerItems = new Dictionary<int, MerchantUIItemPlayer>();

            if (transform.childCount >= 1)
            {
                merchantRoot = transform.GetChild(0).gameObject;
                merchantAnimator = merchantRoot.GetComponent<Animator>();
            }
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Open(Merchant merchant)
        {
            currentMerchant = merchant;
            ChangeState(true);

            if (DATABASE_INVENTORY.inventorySettings.pauseTimeOnUI)
            {
                TimeManager.Instance.SetTimeScale(0f, TIME_LAYER);
            }

            BuildSellerItemsUI(merchant);
            UpdatePlayerItems();

            if (textTitle != null) textTitle.text = merchant.title;
            if (textDescription != null) textDescription.text = merchant.description;

            InventoryManager.Instance.eventChangePlayerCurrency.AddListener(UpdateItems);
            InventoryManager.Instance.eventChangePlayerInventory.AddListener(UpdateItems);
        }

        public void Close()
        {
            if (!isOpen) return;

            if (DATABASE_INVENTORY.inventorySettings.pauseTimeOnUI)
            {
                TimeManager.Instance.SetTimeScale(1f, TIME_LAYER);
            }

            ChangeState(false);

            InventoryManager.Instance.eventChangePlayerCurrency.RemoveListener(UpdateItems);
            InventoryManager.Instance.eventChangePlayerInventory.RemoveListener(UpdateItems);
        }

        public void ChangePlayerTypes(int itemTypes)
        {
            playerTypes = itemTypes;
            UpdateItems();
        }

        // STATIC METHODS: ------------------------------------------------------------------------

        public static void OpenMerchant(Merchant merchant)
        {
            RequireInstance(merchant);
            Instance.Open(merchant);
        }

        public static void CloseMerchant()
        {
            if (!IsMerchantOpen()) return;
            Instance.Close();
        }

        public static bool IsMerchantOpen()
        {
            if (Instance == null) return false;
            return Instance.isOpen;
        }

        private static void RequireInstance(Merchant merchant)
        {
            if (DATABASE_INVENTORY == null) DATABASE_INVENTORY = DatabaseInventory.Load();
            if (Instance == null)
            {
                EventSystemManager.Instance.Wakeup();
                if (DATABASE_INVENTORY.inventorySettings == null)
                {
                    Debug.LogError("No inventory database found");
                    return;
                }

                GameObject prefab = merchant.merchantUI;
                if (prefab == null) prefab = DATABASE_INVENTORY.inventorySettings.merchantUIPrefab;
                if (prefab == null) prefab = Resources.Load<GameObject>(DEFAULT_UI_PATH);

                Instantiate(prefab, Vector3.zero, Quaternion.identity);
            }
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void ChangeState(bool toOpen)
        {
            if (merchantRoot == null)
            {
                Debug.LogError("Unable to find merchantRoot");
                return;
            }

            isOpen = toOpen;

            if (merchantAnimator == null)
            {
                merchantRoot.SetActive(toOpen);
                return;
            }

            merchantAnimator.SetBool("State", toOpen);
            InventoryManager.Instance.eventMerchantUI.Invoke(toOpen);
        }

        private void UpdateItems()
        {
            foreach (KeyValuePair<int, MerchantUIItemSeller> item in merchantItems)
            {
                if (item.Value != null) item.Value.UpdateUI();
            }

            UpdatePlayerItems();
        }

        // PRIVATE SELLER METHODS: ----------------------------------------------------------------

        private void BuildSellerItemsUI(Merchant merchant)
        {
            for (int i = scrollMerchant.content.childCount - 1; i >= 0; --i)
            {
                Destroy(scrollMerchant.content.GetChild(i).gameObject);
            }

            merchantItems = new Dictionary<int, MerchantUIItemSeller>();

            for (int i = 0; i < merchant.warehouse.wares.Length; ++i)
            {
                if (merchantItems.ContainsKey(merchant.warehouse.wares[i].item.item.uuid)) continue;

                GameObject instance = Instantiate(itemUIPrefabSeller, scrollMerchant.content);
                MerchantUIItemSeller item = instance.GetComponent<MerchantUIItemSeller>();
                merchantItems.Add(merchant.warehouse.wares[i].item.item.uuid, item);
                item.Setup(this, merchant.warehouse.wares[i]);
            }
        }

        // PRIVATE PLAYER METHODS: ----------------------------------------------------------------

        private void UpdatePlayerItems()
        {
            Dictionary<int, MerchantUIItemPlayer> remainingItems = null;
            remainingItems = new Dictionary<int, MerchantUIItemPlayer>(playerItems);

            foreach (KeyValuePair<int, int> entry in InventoryManager.Instance.playerInventory.items)
            {
                Item currentItem = InventoryManager.Instance.itemsCatalogue[entry.Key];
                int currentItemAmount = InventoryManager.Instance.playerInventory.items[currentItem.uuid];

                if (currentItemAmount <= 0) continue;
                if ((currentItem.itemTypes & playerTypes) == 0) continue;
                if (!currentItem.canBeSold) continue;

                if (playerItems != null && playerItems.ContainsKey(currentItem.uuid))
                {
                    playerItems[currentItem.uuid].UpdateUI();
                    remainingItems.Remove(currentItem.uuid);
                }
                else
                {
                    GameObject itemUIPrefab = itemUIPrefabPlayer;
                    GameObject itemUIAsset = Instantiate(itemUIPrefab, scrollPlayer.content);

                    MerchantUIItemPlayer itemUI = itemUIAsset.GetComponent<MerchantUIItemPlayer>();

                    itemUI.Setup(this, currentItem);
                    playerItems.Add(currentItem.uuid, itemUI);
                }
            }

            foreach (KeyValuePair<int, MerchantUIItemPlayer> entry in remainingItems)
            {
                playerItems.Remove(entry.Key);
                Destroy(entry.Value.gameObject);
            }
        }
    }
}