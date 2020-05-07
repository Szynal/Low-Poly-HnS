using System;
using System.Collections.Generic;
using LowPolyHnS.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LowPolyHnS.Inventory
{
    public class ContainerUIManager : MonoBehaviour
    {
        private const int TIME_LAYER = 202;

        public static ContainerUIManager Instance { get; private set; }
        private static DatabaseInventory DATABASE_INVENTORY;

        private const string DEFAULT_UI_PATH = "LowPolyHnS/Inventory/ContainerUI";

        [Serializable]
        public class ContainerEvent : UnityEvent<int>
        {
        }

        [Serializable]
        public class ContainerTakeAllEvent : UnityEvent
        {
        }

        [Serializable]
        public class ContainerCloseEvent : UnityEvent
        {
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public ScrollRect scrollContainer;
        public ScrollRect scrollPlayer;

        [Space, InventoryMultiItemType, SerializeField]
        private int playerTypes = ~0;

        [Space] public GameObject itemUIPrefabContainer;
        public GameObject itemUIPrefabPlayer;

        [HideInInspector] public Container currentContainer;

        private Animator containerAnimator;
        private GameObject containerRoot;
        private bool isOpen;

        [Space] public ContainerEvent onAdd = new ContainerEvent();
        public ContainerEvent onRemove = new ContainerEvent();
        public ContainerTakeAllEvent onTakeAll = new ContainerTakeAllEvent();
        public ContainerCloseEvent CloseEvent = new ContainerCloseEvent();


        private Dictionary<int, ContainerUIItemBox> containerItems;
        private Dictionary<int, ContainerUIItemPlayer> playerItems;

        [Space] public Button buttonTakeAll;
        public Button buttonClose;

        // INITIALIZERS: --------------------------------------------------------------------------

        private void Awake()
        {
            Instance = this;

            containerItems = new Dictionary<int, ContainerUIItemBox>();
            playerItems = new Dictionary<int, ContainerUIItemPlayer>();

            if (transform.childCount >= 1)
            {
                containerRoot = transform.GetChild(0).gameObject;
                containerAnimator = containerRoot.GetComponent<Animator>();
            }

            if (buttonTakeAll != null)
            {
                buttonTakeAll.onClick.AddListener(GetAllItemsFromContainer);
            }

            if (buttonClose != null)
            {
                buttonClose.onClick.AddListener(CloseContainerUI);
            }
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Open(Container container)
        {
            currentContainer = container;
            ChangeState(true);

            if (DATABASE_INVENTORY.inventorySettings.pauseTimeOnUI)
            {
                TimeManager.Instance.SetTimeScale(0f, TIME_LAYER);
            }

            UpdateItems();

            InventoryManager.Instance.eventChangePlayerCurrency.AddListener(UpdatePlayerItems);
            InventoryManager.Instance.eventChangePlayerInventory.AddListener(UpdatePlayerItems);

            currentContainer.AddOnAddListener(UpdateContainerItems);
            currentContainer.AddOnRemoveListener(UpdateContainerItems);
        }

        public void Close()
        {
            if (!isOpen) return;
            if (DATABASE_INVENTORY.inventorySettings.pauseTimeOnUI)
            {
                TimeManager.Instance.SetTimeScale(1f, TIME_LAYER);
            }

            ChangeState(false);

            InventoryManager.Instance.eventChangePlayerCurrency.RemoveListener(UpdatePlayerItems);
            InventoryManager.Instance.eventChangePlayerInventory.RemoveListener(UpdatePlayerItems);

            currentContainer.RemoveOnAddListener(UpdateContainerItems);
            currentContainer.RemoveOnRemoveListener(UpdateContainerItems);
            currentContainer.Animate();
        }

        public void ChangePlayerTypes(int itemTypes)
        {
            playerTypes = itemTypes;
            UpdateItems();
        }

        public void UpdateItems()
        {
            UpdateContainerItems();
            UpdatePlayerItems();
        }

        // STATIC METHODS: ------------------------------------------------------------------------

        public static void OpenContainer(Container container)
        {
            RequireInstance(container);
            Instance.Open(container);
        }

        public static void CloseContainer()
        {
            if (!IsContainerOpen()) return;
            Instance.Close();
        }

        public static bool IsContainerOpen()
        {
            if (Instance == null) return false;
            return Instance.isOpen;
        }

        private static void RequireInstance(Container container)
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

                GameObject prefab = container.containerUI;
                if (prefab == null) prefab = DATABASE_INVENTORY.inventorySettings.containerUIPrefab;
                if (prefab == null) prefab = Resources.Load<GameObject>(DEFAULT_UI_PATH);

                Instantiate(prefab, Vector3.zero, Quaternion.identity);
            }
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void ChangeState(bool toOpen)
        {
            if (containerRoot == null)
            {
                Debug.LogError("Unable to find containerRoot");
                return;
            }

            isOpen = toOpen;

            if (containerAnimator == null)
            {
                containerRoot.SetActive(toOpen);
                return;
            }

            containerAnimator.SetBool("State", toOpen);
            InventoryManager.Instance.eventContainerUI.Invoke(
                toOpen,
                currentContainer.gameObject
            );
        }

        // PRIVATE CONTAINER METHODS: -------------------------------------------------------------

        private void UpdateContainerItems(int itemID = 0, int amount = 0)
        {
            Dictionary<int, ContainerUIItemBox> remainingItems = null;
            remainingItems = new Dictionary<int, ContainerUIItemBox>(containerItems);

            List<Container.ItemData> items = currentContainer.GetItems();
            for (int i = 0; i < items.Count; ++i)
            {
                Container.ItemData currentItem = items[i];
                if (currentItem.amount <= 0) continue;

                if (containerItems.ContainsKey(currentItem.uuid))
                {
                    containerItems[currentItem.uuid].UpdateUI();
                    remainingItems.Remove(currentItem.uuid);
                }
                else
                {
                    GameObject itemUIPrefab = itemUIPrefabContainer;
                    GameObject itemUIAsset = Instantiate(itemUIPrefab, scrollContainer.content);

                    ContainerUIItemBox itemUI = itemUIAsset.GetComponent<ContainerUIItemBox>();

                    itemUI.Setup(this, currentItem);
                    containerItems.Add(currentItem.uuid, itemUI);
                }
            }

            foreach (KeyValuePair<int, ContainerUIItemBox> entry in remainingItems)
            {
                containerItems.Remove(entry.Key);
                Destroy(entry.Value.gameObject);
            }
        }

        public void GetAllItemsFromContainer()
        {
            List<Container.ItemData> items = currentContainer.GetItems();
            for (int i = 0; i < items.Count; ++i)
            {
                Container.ItemData item = items[i];

                InventoryManager.Instance.AddItemToInventory(item.uuid, item.amount);
                currentContainer.RemoveItem(item.uuid, item.amount);
            }

            onTakeAll.Invoke();
        }


        public void CloseContainerUI()
        {
            CloseEvent.Invoke();
        }


        // PRIVATE PLAYER METHODS: ----------------------------------------------------------------

        private void UpdatePlayerItems()
        {
            Dictionary<int, ContainerUIItemPlayer> remainingItems = null;
            remainingItems = new Dictionary<int, ContainerUIItemPlayer>(playerItems);

            foreach (KeyValuePair<int, int> entry in InventoryManager.Instance.playerInventory.items)
            {
                Item currentItem = InventoryManager.Instance.itemsCatalogue[entry.Key];
                int currentItemAmount = InventoryManager.Instance.playerInventory.items[currentItem.uuid];

                if (currentItemAmount <= 0) continue;
                if ((currentItem.itemTypes & playerTypes) == 0) continue;

                if (playerItems != null && playerItems.ContainsKey(currentItem.uuid))
                {
                    playerItems[currentItem.uuid].UpdateUI();
                    remainingItems.Remove(currentItem.uuid);
                }
                else
                {
                    GameObject itemUIPrefab = itemUIPrefabPlayer;
                    GameObject itemUIAsset = Instantiate(itemUIPrefab, scrollPlayer.content);

                    ContainerUIItemPlayer itemUI = itemUIAsset.GetComponent<ContainerUIItemPlayer>();

                    itemUI.Setup(this, currentItem);
                    playerItems.Add(currentItem.uuid, itemUI);
                }
            }

            foreach (KeyValuePair<int, ContainerUIItemPlayer> entry in remainingItems)
            {
                playerItems.Remove(entry.Key);
                Destroy(entry.Value.gameObject);
            }
        }
    }
}