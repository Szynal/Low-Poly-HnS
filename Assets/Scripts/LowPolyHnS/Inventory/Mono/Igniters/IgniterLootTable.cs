﻿using LowPolyHnS.Core;
using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
    [AddComponentMenu("")]
    public class IgniterLootTable : Igniter
    {
#if UNITY_EDITOR
        public new static string NAME = "Inventory/On Loot Table";
        public new static string ICON_PATH = "Assets/Content/Icons/Inventory/Igniters/";
        public const string CUSTOM_ICON_PATH = "Assets/Content/Icons/Inventory/Igniters/";
#endif

        [Space] [VariableFilter(Variable.DataType.String)]
        public VariableProperty storeItemName = new VariableProperty();

        [Space] [VariableFilter(Variable.DataType.Number)]
        public VariableProperty storeItemAmount = new VariableProperty();

        private void Start()
        {
            LootObject.AddListener(OnUseLootTable);
        }

        private void OnDestroy()
        {
            if (isExitingApplication) return;
            LootObject.RemoveListener(OnUseLootTable);
        }

        private void OnUseLootTable(LootObject.LootResult result)
        {
            string itemName = result.item.itemName.GetText();
            float itemAmount = result.amount;

            storeItemName.Set(itemName, gameObject);
            storeItemAmount.Set(itemAmount, gameObject);

            ExecuteTrigger(gameObject);
        }
    }
}