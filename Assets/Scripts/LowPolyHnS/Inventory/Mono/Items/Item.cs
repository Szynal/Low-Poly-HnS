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
        // PROPERTIES: -------------------------------------------------------------------------------------------------

        public int uuid = -1;
        [LocStringNoPostProcess] public LocString itemName = new LocString();
        [LocStringNoPostProcess] public LocString itemDescription = new LocString();
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

        // CONSTRUCTOR: ------------------------------------------------------------------------------------------------

#if UNITY_EDITOR

        public static Item CreateItemInstance()
        {
            Item item = CreateInstance<Item>();
            Guid guid = Guid.NewGuid();

            item.name = "item." + Mathf.Abs(guid.GetHashCode());
            item.uuid = Mathf.Abs(guid.GetHashCode());

            item.itemName = new LocString();
            item.itemDescription = new LocString();
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