using System;
using LowPolyHnS.Inventory;
using UnityEngine;

namespace LowPolyHnS.Crafting
{
    public class CraftingUIRecipe : CraftingUIItem
    {
        public static DatabaseInventory Database;



        public ItemHolder ItemHolder;
        [Space] public CanvasGroup CanvasGroup;


        public override void OnClickButton()
        {
            throw new NotImplementedException();
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