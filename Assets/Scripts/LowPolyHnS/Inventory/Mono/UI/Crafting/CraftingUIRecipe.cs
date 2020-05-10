using LowPolyHnS.Core;
using LowPolyHnS.Inventory;
using UnityEngine;

namespace LowPolyHnS.Crafting
{
    public class CraftingUIRecipe : CraftingUIItem
    {
        private static DatabaseInventory DATABASE_INVENTORY;

        public ItemHolder ItemHolder;
        public int CatalogueKey;
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

            foreach (Transform child in CraftTable)
            {
                Destroy(child.gameObject);
            }

            var itemA = Instantiate(CraftTablePrefab, Vector3.zero, Quaternion.identity, CraftTable.transform);
            var itemToCombineA = itemA.GetComponent<CraftingUIItemToCombine>();
            var itemB = Instantiate(CraftTablePrefab, Vector3.zero, Quaternion.identity, CraftTable.transform);
            var itemToCombineB = itemB.GetComponent<CraftingUIItemToCombine>();

            SetIngredientInformation(itemToCombineA, itemToCombineB,
                DATABASE_INVENTORY.inventoryCatalogue.recipes[CatalogueKey]);
        }

        private static void SetIngredientInformation(CraftingUIItemToCombine itemToCombineA,
            CraftingUIItemToCombine itemToCombineB, Recipe recipe)
        {
            if (itemToCombineA == null) return;
            if (itemToCombineB == null) return;
            if (recipe == null) return;

            itemToCombineA.TextName.text = recipe.itemToCombineA.item.itemName.content;
            itemToCombineA.TextDescription.text = recipe.itemToCombineA.item.itemDescription.content;
            itemToCombineA.Image.sprite = recipe.itemToCombineA.item.sprite;
            itemToCombineA.TextAmount.text = recipe.amountA.ToString();

            itemToCombineB.TextName.text = recipe.itemToCombineB.item.itemName.content;
            itemToCombineB.TextDescription.text = recipe.itemToCombineB.item.itemDescription.content;
            itemToCombineB.Image.sprite = recipe.itemToCombineB.item.sprite;
            itemToCombineB.TextAmount.text = recipe.amountB.ToString();
        }

        public void InitRecipe()
        {
            CanvasGroup.transform.name = ItemHolder.item.itemName.content;
            Image.sprite = ItemHolder.item.sprite;
            TextName.text = ItemHolder.item.itemName.content;
            TextDescription.text = ItemHolder.item.itemDescription.content;
        }
    }
}