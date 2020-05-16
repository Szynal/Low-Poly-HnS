using System;
using LowPolyHnS.Core;
using LowPolyHnS.Localization;
using UnityEngine;
using UnityEngine.Serialization;

namespace LowPolyHnS.Inventory
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [Serializable]
    public class Item : ScriptableObject
    {
        #region PROPERTIES

        public int uuid = -1;
        [LocStringNoPostProcess] public LocString itemName = new LocString();
        [LocStringNoPostProcess] public LocString itemDescription = new LocString();

        public int StrengthBonus;
        public int AgilityBonus;
        public int IntelligenceBonus;

        public float StrengthPercentBonus;
        public float AgilityPercentBonus;
        public float IntelligencePercentBonus;

        public float FireResistanceBonus;
        public float ColdResistanceBonus;
        public float PoisonResistanceBonus;
        
        public Color itemColor = Color.grey;

        public Sprite sprite;
        public GameObject prefab;

        public bool canBeSold = true;
        public int price;
        public int maxStack = 99;
        public float weight = 0f;

        [InventoryMultiItemType] public int itemTypes = 0;

        [FormerlySerializedAs("consumable")] public bool onClick = true;
        public bool consumeItem = true;

        [FormerlySerializedAs("actionsList")] public IActionsList actionsOnClick;

        public bool equipable = false;
        public bool fillAllTypes = false;
        public IConditionsList conditionsEquip;
        public IActionsList actionsOnEquip;
        public IActionsList actionsOnUnequip;

        #endregion

#if UNITY_EDITOR

        public static Item CreateItemInstance()
        {
            Item item = CreateInstance<Item>();
            Guid guid = Guid.NewGuid();

            item.name = "item." + Mathf.Abs(guid.GetHashCode());
            item.uuid = Mathf.Abs(guid.GetHashCode());

            item.itemName = new LocString();
            item.itemDescription = new LocString();

            item.StrengthBonus = 0;
            item.AgilityBonus = 0;
            item.IntelligenceBonus = 0;
            item.StrengthPercentBonus = 0;
            item.AgilityPercentBonus = 0;
            item.IntelligencePercentBonus = 0;

            item.FireResistanceBonus = 0;
            item.ColdResistanceBonus = 0;
            item.PoisonResistanceBonus = 0;

            item.price = 1;
            item.maxStack = 99;
            item.hideFlags = HideFlags.HideInHierarchy;

            DatabaseInventory databaseInventory = DatabaseInventory.Load();
            AssetDatabase.AddObjectToAsset(item, databaseInventory);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(item));
            return item;
        }

        private void OnDestroy()
        {
            DestroyAsset(actionsOnClick);
            DestroyAsset(conditionsEquip);
            DestroyAsset(actionsOnEquip);
            DestroyAsset(actionsOnUnequip);
        }

        private void DestroyAsset(MonoBehaviour reference)
        {
            if (reference == null) return;
            if (reference.gameObject == null) return;
            DestroyImmediate(reference.gameObject, true);
        }

#endif
    }
}