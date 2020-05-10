using System;
using System.Collections.Generic;
using LowPolyHnS.Core;
using LowPolyHnS.Inventory;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LowPolyHnS.Crafting
{
    public class CraftingUIManager : MonoBehaviour
    {
        private const int TIME_LAYER = 203;

        public static CraftingUIManager Instance;
        private static DatabaseInventory DATABASE_INVENTORY;

        private const string DEFAULT_UI_PATH = "LowPolyHnS/Inventory/CraftingUI";

        [Serializable]
        public class CraftingEvent : UnityEvent<int>
        {
        }

        #region PROPERTIES

        public ScrollRect ScrollRecipes;
        public ScrollRect scrollPlayer;

        [Space] public Text textTitle;
        public Text textDescription;

        [InventoryMultiItemType, SerializeField]
        private int playerTypes = ~0;

        [Space] public GameObject ItemCraftingPlayerUI;
        public GameObject ItemCraftingRecipesUI;

        [HideInInspector] public Merchant currentMerchant;

        private Animator craftingAnimator;
        private GameObject craftingRoot;
        private bool isOpen;

        [Space] public CraftingEvent OnCraft = new CraftingEvent();
        public CraftingEvent onCantCraft = new CraftingEvent();

        private Dictionary<int, CraftingUIRecipe> merchantItems;
        private Dictionary<int, CraftingUIItemPlayer> playerItems;

        #endregion


        #region INITIALIZERS

        private void Awake()
        {
            Instance = this;

            Instance = this;
            if (transform.childCount < 1)
            {
                return;
            }

            craftingRoot = transform.GetChild(0).gameObject;
            craftingAnimator = craftingRoot.GetComponent<Animator>();
        }

        #endregion


        #region PUBLIC METHODS

        public void Open()
        {
            if (isOpen) return;

            ChangeState(true);

            if (DATABASE_INVENTORY.inventorySettings.pauseTimeOnUI)
            {
                TimeManager.Instance.SetTimeScale(0f, TIME_LAYER);
            }
        }

        public void Close()
        {
            if (!isOpen) return;

            if (DATABASE_INVENTORY.inventorySettings.pauseTimeOnUI)
            {
                TimeManager.Instance.SetTimeScale(1f, TIME_LAYER);
            }

            ChangeState(false);
        }

        #endregion

        private void ChangeState(bool toOpen)
        {
            if (craftingRoot == null)
            {
                Debug.LogError("Unable to find craftingRoot");
                return;
            }

            isOpen = toOpen;

            if (craftingAnimator == null)
            {
                craftingRoot.SetActive(toOpen);
                return;
            }

            craftingAnimator.SetBool("State", toOpen);
            InventoryManager.Instance.eventMerchantUI.Invoke(toOpen);
        }


        #region STATIC METHODS

        public static void OpenCraftingTable()
        {
            RequireInstance();
            Instance.Open();
        }

        public static void CloseCraftingTable()
        {
            if (!IsCraftingTableOpen()) return;
            Instance.Close();
        }

        public static void OpenCloseCraftingTable()
        {
            if (IsCraftingTableOpen())
            {
                CloseCraftingTable();
            }
            else
            {
                OpenCraftingTable();
            }
        }

        public static bool IsCraftingTableOpen()
        {
            return Instance != null && Instance.isOpen;
        }

        private static void RequireInstance()
        {
            if (DATABASE_INVENTORY == null) DATABASE_INVENTORY = DatabaseInventory.Load();

            if (Instance != null)
            {
                return;
            }

            EventSystemManager.Instance.Wakeup();
            if (DATABASE_INVENTORY.inventorySettings == null)
            {
                Debug.LogError("No inventory database found");
                return;
            }

            GameObject prefab = DATABASE_INVENTORY.inventorySettings.craftingUIPrefab;
            if (prefab == null) prefab = Resources.Load<GameObject>(DEFAULT_UI_PATH);

            Instantiate(prefab, Vector3.zero, Quaternion.identity);
        }

        #endregion
    }
}