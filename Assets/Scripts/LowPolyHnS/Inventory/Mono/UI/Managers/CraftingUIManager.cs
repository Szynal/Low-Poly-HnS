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
        private const int TIME_LAYER = 201;

        public static CraftingUIManager Instance;
        private static DatabaseInventory DATABASE_INVENTORY;

        private const string DEFAULT_UI_PATH = "Assets/Content/Prefabs/UI/PlayerUI/CraftingUI";

        [Serializable]
        public class CraftingEvent : UnityEvent<int>
        {
        }

        public ScrollRect ScrollRecipes;
        public ScrollRect ScrollPlayer;

        [Space] public Text TextTitle;
        public Text TextDescription;

        [Space] public GameObject itemUIPrefabSeller;
        public GameObject itemUIPrefabPlayer;

        private bool isOpen;

        [Space] public CraftingEvent OnCraft = new CraftingEvent();
        public CraftingEvent OnCantCraft = new CraftingEvent();

        private Dictionary<int, CraftingUIRecipes> craftingRecipes;
        private Dictionary<int, CraftingUIItemPlayer> playerItems;


        #region INITIALIZERS

        private void Awake()
        {
            Instance = this;
        }

        #endregion


        #region PUBLIC METHODS

        public void Open()
        {
        }

        public void Close()
        {
        }

        #endregion


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
            if (Instance == null) return false;
            return Instance.isOpen;
        }

        private static void RequireInstance()
        {
            if (DATABASE_INVENTORY == null) DATABASE_INVENTORY = DatabaseInventory.Load();
            if (Instance == null)
            {
                EventSystemManager.Instance.Wakeup();
                if (DATABASE_INVENTORY.inventorySettings == null)
                {
                    Debug.LogError("No inventory database found");
                }
            }
        }

        #endregion
    }
}