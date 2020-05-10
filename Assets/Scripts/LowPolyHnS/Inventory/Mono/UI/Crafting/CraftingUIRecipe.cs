using LowPolyHnS.Core;
using LowPolyHnS.Inventory;
using UnityEngine;

namespace LowPolyHnS.Crafting
{
    public class CraftingUIRecipe : CraftingUIItem
    {
        private static DatabaseInventory DATABASE_INVENTORY;

        public ItemHolder ItemHolder;
        public GameObject CraftTablePrefab;

        [Space] public CanvasGroup CanvasGroup;
        public RectTransform CraftTable;

        public override void OnClickButton()
        {
            if (DATABASE_INVENTORY == null) DATABASE_INVENTORY = DatabaseInventory.Load();

            EventSystemManager.Instance.Wakeup();
            if (DATABASE_INVENTORY.inventorySettings == null)
            {
                Debug.LogError("No inventory database found");
                return;
            }

            Recipe[] catalogueRecipes = DATABASE_INVENTORY.inventoryCatalogue.recipes;

            foreach (var recipe in catalogueRecipes)
            {
                Debug.Log(recipe);

                var oldContent = CraftTable.GetComponentsInChildren<RectTransform>();
                foreach (var content in oldContent)
                {
                    Destroy(content);
                }

                Instantiate(CraftTablePrefab, Vector3.zero, Quaternion.identity, CraftTable.transform);
            }
        }


        public void InitRecipe()
        {
            CanvasGroup.gameObject.transform.name = ItemHolder.item.itemName.content;
            Image.sprite = ItemHolder.item.sprite;
            TextName.text = ItemHolder.item.itemName.content;
            TextDescription.text = ItemHolder.item.itemDescription.content;
        }
    }
}