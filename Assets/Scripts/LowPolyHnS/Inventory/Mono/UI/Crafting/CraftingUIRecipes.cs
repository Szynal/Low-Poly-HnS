using System;
using UnityEngine;
using UnityEngine.UI;

namespace LowPolyHnS.Crafting
{
    public class CraftingUIRecipes : CraftingUIItem
    {
        [Space] public CanvasGroup CanvasGroup;

        [Space] public GameObject WrapAmount;
        public Text TextCurrentAmount;
        public Text TextMaxAmount;

        public override void UpdateUI()
        {
            throw new NotImplementedException();
        }

        public override void OnClickButton()
        {
            throw new NotImplementedException();
        }
    }
}