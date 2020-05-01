using System.Collections.Generic;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
    [AddComponentMenu("LowPolyHnS/UI/Items List")]
    public class ItemsListUI : MonoBehaviour
    {
        public RectTransform container;

        [Space] public GameObject prefabItem;

        [InventoryMultiItemType, SerializeField]
        protected int itemTypes = ~0;

        private Dictionary<int, ItemUI> currentItems = new Dictionary<int, ItemUI>();
        private bool isExitingApplication;

        // INITIALIZERS: --------------------------------------------------------------------------

        private void OnEnable()
        {
            InventoryManager.Instance.eventChangePlayerInventory.AddListener(UpdateItems);
            UpdateItems();
        }

        private void OnDisable()
        {
            if (isExitingApplication) return;
            InventoryManager.Instance.eventChangePlayerInventory.RemoveListener(UpdateItems);
        }

        private void OnApplicationQuit()
        {
            isExitingApplication = true;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public virtual void SetItemTypes(int itemTypes)
        {
            this.itemTypes = itemTypes;
            UpdateItems();
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        protected virtual void UpdateItems()
        {
            Dictionary<int, ItemUI> remainingItems = new Dictionary<int, ItemUI>(currentItems);

            foreach (KeyValuePair<int, int> entry in InventoryManager.Instance.playerInventory.items)
            {
                Item currentItem = InventoryManager.Instance.itemsCatalogue[entry.Key];
                int currentItemAmount = InventoryManager.Instance.playerInventory.items[currentItem.uuid];

                if (currentItemAmount <= 0) continue;
                if ((currentItem.itemTypes & itemTypes) == 0) continue;

                if (currentItems != null && currentItems.ContainsKey(currentItem.uuid))
                {
                    currentItems[currentItem.uuid].UpdateUI(currentItem, currentItemAmount);
                    remainingItems.Remove(currentItem.uuid);
                }
                else
                {
                    GameObject itemUIPrefab = prefabItem;
                    if (itemUIPrefab == null)
                    {
                        string error = "No inventory item UI prefab found. Fill the required field at {0}";
                        string errorPath = "LowPolyHnS/Preferences and head to Inventory -> Settings";
                        Debug.LogErrorFormat(error, errorPath);
                        return;
                    }

                    GameObject itemUIAsset = Instantiate(itemUIPrefab, container);
                    ItemUI itemUI = itemUIAsset.GetComponent<ItemUI>();
                    itemUI.Setup(currentItem, currentItemAmount);
                    currentItems.Add(currentItem.uuid, itemUI);
                }
            }

            foreach (KeyValuePair<int, ItemUI> entry in remainingItems)
            {
                currentItems.Remove(entry.Key);
                Destroy(entry.Value.gameObject);
            }
        }
    }
}