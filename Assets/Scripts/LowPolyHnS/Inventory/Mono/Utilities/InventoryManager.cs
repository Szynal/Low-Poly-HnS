using System;
using System.Collections.Generic;
using LowPolyHnS.Core;
using LowPolyHnS.Core.Hooks;
using UnityEngine;
using UnityEngine.Events;

namespace LowPolyHnS.Inventory
{
    [AddComponentMenu("LowPolyHnS/Managers/Inventory Manager", 100)]
    public class InventoryManager : Singleton<InventoryManager>, IGameSave
    {
        [Serializable]
        public class PlayerInventory
        {
            public Dictionary<int, int> items;
            public int currencyAmount;

            public bool weightDirty;
            public float weightCached;

            public PlayerInventory()
            {
                items = new Dictionary<int, int>();
                currencyAmount = 0;

                weightDirty = true;
                weightCached = 0f;
            }
        }

        [Serializable]
        protected class InventorySaveData
        {
            public int[] playerItemsUUIDS;
            public int[] playerItemsStack;
            public int playerCurrencyAmount;

            public InventorySaveData()
            {
                playerItemsUUIDS = new int[0];
                playerItemsStack = new int[0];
                playerCurrencyAmount = 0;
            }
        }

        public class InventoryEvent : UnityEvent
        {
        }

        public class EquipmentEvent : UnityEvent<GameObject, int>
        {
        }

        public class ContainerUIEvent : UnityEvent<bool, GameObject>
        {
        }

        public class InventoryUIEvent : UnityEvent<bool>
        {
        }

        public class MerchantUIEvent : UnityEvent<bool>
        {
        }

        private const string WARN_PLYR = "Adding <b>PlayerEquipment</b> component because none was found.";
        private const string WARN_CHAR = "Adding <b>CharacterEquipment</b> component because none was found.";
        private const string WARN_SOLT = "Consider adding one in the Editor";

        // PROPERTIES: ----------------------------------------------------------------------------

        protected static DatabaseInventory INVENTORY;

        public PlayerInventory playerInventory { private set; get; }
        public Dictionary<int, Item> itemsCatalogue { private set; get; }
        public Dictionary<Recipe.Key, Recipe> recipes { private set; get; }

        [HideInInspector] public InventoryEvent eventChangePlayerInventory = new InventoryEvent();
        [HideInInspector] public InventoryEvent eventChangePlayerCurrency = new InventoryEvent();

        [HideInInspector] public EquipmentEvent eventOnEquip = new EquipmentEvent();
        [HideInInspector] public EquipmentEvent eventOnUnequip = new EquipmentEvent();

        [HideInInspector] public InventoryUIEvent eventInventoryUI = new InventoryUIEvent();
        [HideInInspector] public ContainerUIEvent eventContainerUI = new ContainerUIEvent();
        [HideInInspector] public MerchantUIEvent eventMerchantUI = new MerchantUIEvent();

        // INITIALIZE: ----------------------------------------------------------------------------

        protected override void OnCreate()
        {
            DatabaseInventory dbInventory = DatabaseInventory.Load();
            if (INVENTORY == null) INVENTORY = dbInventory;

            eventChangePlayerInventory = new InventoryEvent();
            eventChangePlayerCurrency = new InventoryEvent();

            itemsCatalogue = new Dictionary<int, Item>();
            for (int i = 0; i < dbInventory.inventoryCatalogue.items.Length; ++i)
            {
                itemsCatalogue.Add(
                    dbInventory.inventoryCatalogue.items[i].uuid,
                    dbInventory.inventoryCatalogue.items[i]
                );
            }

            recipes = new Dictionary<Recipe.Key, Recipe>();
            for (int i = 0; i < dbInventory.inventoryCatalogue.recipes.Length; ++i)
            {
                recipes.Add(
                    new Recipe.Key(
                        dbInventory.inventoryCatalogue.recipes[i].itemToCombineA.item.uuid,
                        dbInventory.inventoryCatalogue.recipes[i].itemToCombineB.item.uuid
                    ),
                    dbInventory.inventoryCatalogue.recipes[i]
                );
            }

            playerInventory = new PlayerInventory();
            SaveLoadManager.Instance.Initialize(this, 25);

            SaveLoadManager.Instance.eventOnChangeProfile.AddListener(OnChangeProfile);
        }

        protected virtual void OnChangeProfile(int prevProfile, int nextProfile)
        {
            SaveLoadManager.Instance.eventOnChangeProfile.RemoveListener(OnChangeProfile);
            Destroy(gameObject);
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public virtual int AddItemToInventory(int uuid, int amount = 1)
        {
            if (!itemsCatalogue.ContainsKey(uuid))
            {
                Debug.LogError("Could not find item UUID in item catalogue");
                return 0;
            }

            Item item = itemsCatalogue[uuid];

            GameObject player = HookPlayer.Instance == null ? null : HookPlayer.Instance.gameObject;
            float maxWeight = INVENTORY.inventorySettings.maxInventoryWeight == null
                ? 100f
                : INVENTORY.inventorySettings.maxInventoryWeight.GetValue(player);

            int amountAdded = 0;
            bool limitWeight = INVENTORY.inventorySettings.limitInventoryWeight;

            for (int i = 0; i < amount; ++i)
            {
                float candidateWeight = GetCurrentWeight() + item.weight;
                if (limitWeight && candidateWeight > maxWeight) continue;

                if (playerInventory.items.ContainsKey(uuid))
                {
                    int candidateAmount = playerInventory.items[uuid] + 1;
                    if (candidateAmount <= itemsCatalogue[uuid].maxStack)
                    {
                        playerInventory.items[uuid] += 1;
                        amountAdded += 1;
                    }
                }
                else
                {
                    playerInventory.items.Add(uuid, 1);
                    amountAdded += 1;
                }
            }

            playerInventory.weightDirty = true;
            if (eventChangePlayerInventory != null)
            {
                eventChangePlayerInventory.Invoke();
            }

            return amountAdded;
        }

        public virtual int SubstractItemFromInventory(int uuid, int amount = 1)
        {
            if (!itemsCatalogue.ContainsKey(uuid))
            {
                Debug.LogError("Could not find item UUID in item catalogue");
                return 0;
            }

            if (playerInventory.items.ContainsKey(uuid))
            {
                int amountRemoved = 0;
                for (int i = 0; i < amount; ++i)
                {
                    if (playerInventory.items[uuid] > 0)
                    {
                        playerInventory.items[uuid] -= 1;
                        amountRemoved += 1;
                    }
                }

                GameObject player = HookPlayer.Instance != null ? HookPlayer.Instance.gameObject : null;
                int numEquipped = HasEquiped(player, uuid);
                while (numEquipped > playerInventory.items[uuid])
                {
                    Unequip(player, uuid);
                    --numEquipped;
                }

                if (playerInventory.items[uuid] <= 0)
                {
                    playerInventory.items.Remove(uuid);
                }

                playerInventory.weightDirty = true;
                if (eventChangePlayerInventory != null) eventChangePlayerInventory.Invoke();
                return amountRemoved;
            }

            return 0;
        }

        public virtual bool ConsumeItem(int uuid, GameObject target = null)
        {
            Item item = itemsCatalogue[uuid];
            target = target == null ? HookPlayer.Instance.gameObject : target;

            if (item == null) return false;
            if (!item.onClick) return false;
            if (item.actionsOnClick.isExecuting) return false;
            if (GetInventoryAmountOfItem(uuid) <= 0) return false;

            if (item.consumeItem)
            {
                int amount = SubstractItemFromInventory(item.uuid);
                if (amount == 0) return false;
            }

            Vector3 position = Vector3.zero;
            Quaternion rotation = Quaternion.identity;
            if (target != null)
            {
                position = target.transform.position;
                rotation = target.transform.rotation;
            }

            GameObject instance = Instantiate(item.actionsOnClick.gameObject, position, rotation);
            Actions actions = instance.GetComponent<Actions>();
            actions.destroyAfterFinishing = true;
            actions.Execute(target ?? gameObject);
            return true;
        }

        public virtual int GetInventoryAmountOfItem(int uuid)
        {
            if (!itemsCatalogue.ContainsKey(uuid))
            {
                Debug.LogError("Could not find item UUID in item catalogue");
                return 0;
            }

            if (playerInventory.items.ContainsKey(uuid))
            {
                return playerInventory.items[uuid];
            }

            return 0;
        }

        public virtual bool BuyItem(int uuid, int amount, Merchant merchant = null)
        {
            if (!itemsCatalogue.ContainsKey(uuid))
            {
                Debug.LogError("Could not find item UUID in item catalogue");
                return false;
            }

            GameObject player = HookPlayer.Instance != null ? HookPlayer.Instance.gameObject : null;
            float percent = merchant != null
                ? merchant.purchasePercent.GetValue(player)
                : 1.0f;

            int totalPrice = Mathf.FloorToInt(itemsCatalogue[uuid].price * amount * percent);
            if (totalPrice > playerInventory.currencyAmount) return false;

            int amountAdded = AddItemToInventory(uuid, amount);
            if (amountAdded > 0)
            {
                int finalPrice = Mathf.FloorToInt(
                    itemsCatalogue[uuid].price * amountAdded * percent
                );

                SubstractCurrency(finalPrice);
                return true;
            }

            return false;
        }

        public virtual bool SellItem(int uuid, int amount, Merchant merchant = null)
        {
            if (!itemsCatalogue.ContainsKey(uuid))
            {
                Debug.LogError("Could not find item UUID in item catalogue");
                return false;
            }

            if (GetInventoryAmountOfItem(uuid) < amount) return false;
            SubstractItemFromInventory(uuid, amount);

            int price = itemsCatalogue[uuid].price * amount;
            if (merchant != null)
            {
                GameObject player = HookPlayer.Instance != null ? HookPlayer.Instance.gameObject : null;
                float percent = merchant.sellPercent.GetValue(player);
                price = Mathf.FloorToInt(price * percent);
            }

            AddCurrency(price);
            return true;
        }

        public virtual void AddCurrency(int amount)
        {
            playerInventory.currencyAmount += amount;
            if (eventChangePlayerCurrency != null) eventChangePlayerCurrency.Invoke();
        }

        public virtual void SubstractCurrency(int amount)
        {
            playerInventory.currencyAmount -= amount;
            playerInventory.currencyAmount = Mathf.Max(0, playerInventory.currencyAmount);
            if (eventChangePlayerCurrency != null) eventChangePlayerCurrency.Invoke();
        }

        public virtual int GetCurrency()
        {
            return playerInventory.currencyAmount;
        }

        public virtual void SetCurrency(int value)
        {
            playerInventory.currencyAmount = Mathf.Max(value, 0);
            if (eventChangePlayerCurrency != null) eventChangePlayerCurrency.Invoke();
        }

        public virtual float GetCurrentWeight()
        {
            if (playerInventory.weightDirty)
            {
                float weight = 0f;
                foreach (KeyValuePair<int, int> item in playerInventory.items)
                {
                    int uuid = item.Key;
                    int amount = item.Value;
                    weight += itemsCatalogue[uuid].weight * amount;
                }

                playerInventory.weightCached = weight;
                playerInventory.weightDirty = false;
            }

            return playerInventory.weightCached;
        }

        // RECIPES: -------------------------------------------------------------------------------

        public virtual bool ExistsRecipe(int uuid1, int uuid2)
        {
            bool order1 = recipes.ContainsKey(new Recipe.Key(uuid1, uuid2));
            bool order2 = recipes.ContainsKey(new Recipe.Key(uuid2, uuid1));
            return order1 || order2;
        }

        public bool UseRecipe(int uuid1, int uuid2)
        {
            if (!ExistsRecipe(uuid1, uuid2)) return false;
            Recipe recipe = recipes[new Recipe.Key(uuid1, uuid2)];
            if (recipe == null) recipe = recipes[new Recipe.Key(uuid2, uuid1)];
            if (recipe == null) return false;
            if (recipe.actionsList.isExecuting) return false;

            if (recipe.itemToCombineA.item.uuid == uuid2 && recipe.itemToCombineB.item.uuid == uuid1)
            {
                int auxiliar = uuid1;
                uuid1 = uuid2;
                uuid2 = auxiliar;
            }

            if (GetInventoryAmountOfItem(uuid1) < recipe.amountA ||
                GetInventoryAmountOfItem(uuid2) < recipe.amountB)
            {
                return false;
            }

            if (recipe.removeItemsOnCraft)
            {
                SubstractItemFromInventory(recipe.itemToCombineA.item.uuid, recipe.amountA);
                SubstractItemFromInventory(recipe.itemToCombineB.item.uuid, recipe.amountB);
            }

            Vector3 position = Vector3.zero;
            Quaternion rotation = Quaternion.identity;
            if (HookPlayer.Instance != null)
            {
                position = HookPlayer.Instance.transform.position;
                rotation = HookPlayer.Instance.transform.rotation;
            }

            GameObject instance = Instantiate(recipe.actionsList.gameObject, position, rotation);
            Actions actions = instance.GetComponent<Actions>();
            actions.destroyAfterFinishing = true;
            actions.Execute();
            return true;
        }

        // EQUIPMENT: -----------------------------------------------------------------------------

        public virtual bool CanEquipItem(GameObject target, int uuid, int itemType)
        {
            CharacterEquipment equipment = RequireEquipment(target);
            return equipment.CanEquip(uuid) &&
                   itemsCatalogue[uuid].equipable &&
                   (itemsCatalogue[uuid].itemTypes & (1 << itemType)) > 0 &&
                   itemsCatalogue[uuid].conditionsEquip.Check(target);
        }

        public virtual int HasEquiped(GameObject target, int uuid)
        {
            CharacterEquipment equipment = RequireEquipment(target);
            return equipment != null ? equipment.HasEquip(uuid) : 0;
        }

        public virtual bool HasEquipedTypes(GameObject target, int itemTypes)
        {
            CharacterEquipment equipment = RequireEquipment(target);
            return equipment != null && equipment.HasEquipTypes(itemTypes);
        }

        public virtual Item GetEquip(GameObject target, int itemType)
        {
            CharacterEquipment equipment = RequireEquipment(target);
            int uuid = equipment.GetEquip(itemType);

            if (uuid != 0) return itemsCatalogue[uuid];
            return null;
        }

        public virtual bool Equip(GameObject target, int uuid, int itemType)
        {
            if (CanEquipItem(target, uuid, itemType))
            {
                CharacterEquipment equipment = RequireEquipment(target);
                if (equipment.CanEquip(uuid) && equipment.EquipItem(uuid, itemType))
                {
                    if (eventOnEquip != null) eventOnEquip.Invoke(target, uuid);
                    if (eventChangePlayerInventory != null) eventChangePlayerInventory.Invoke();
                    return true;
                }

                return false;
            }

            return false;
        }

        public virtual bool Unequip(GameObject target, int uuid)
        {
            CharacterEquipment equipment = RequireEquipment(target);
            if (equipment.UnequipItem(uuid))
            {
                if (eventOnUnequip != null) eventOnUnequip.Invoke(target, uuid);
                if (eventChangePlayerInventory != null) eventChangePlayerInventory.Invoke();
                return true;
            }

            return false;
        }

        public virtual bool UnequipTypes(GameObject target, int itemTypes)
        {
            CharacterEquipment equipment = RequireEquipment(target);
            int[] unequipped = equipment.UnequipTypes(itemTypes);

            for (int i = 0; i < unequipped.Length; ++i)
            {
                if (eventOnUnequip != null) eventOnUnequip.Invoke(target, unequipped[i]);
                if (eventChangePlayerInventory != null) eventChangePlayerInventory.Invoke();
            }

            return unequipped.Length > 0;
        }

        protected virtual CharacterEquipment RequireEquipment(GameObject target)
        {
            if (target == null) return null;
            CharacterEquipment equipment = target.GetComponent<CharacterEquipment>();
            if (equipment == null)
            {
                if (HookPlayer.Instance != null && target == HookPlayer.Instance.gameObject)
                {
                    equipment = target.AddComponent<PlayerEquipment>();
                    Debug.LogWarning(WARN_PLYR + " " + WARN_SOLT, target);
                }
                else
                {
                    equipment = target.AddComponent<CharacterEquipment>();
                    Debug.LogWarning(WARN_CHAR + " " + WARN_SOLT, target);
                }
            }

            return equipment;
        }

        // INTERFACE ISAVELOAD: -------------------------------------------------------------------

        public virtual string GetUniqueName()
        {
            return "inventory";
        }

        public virtual Type GetSaveDataType()
        {
            return typeof(InventorySaveData);
        }

        public virtual object GetSaveData()
        {
            InventorySaveData inventorySaveData = new InventorySaveData();
            if (DatabaseInventory.Load().inventorySettings.saveInventory &&
                playerInventory != null)
            {
                if (playerInventory.items != null && playerInventory.items.Count > 0)
                {
                    int playerInventoryItemsCount = playerInventory.items.Count;
                    inventorySaveData.playerItemsUUIDS = new int[playerInventoryItemsCount];
                    inventorySaveData.playerItemsStack = new int[playerInventoryItemsCount];

                    int itemIndex = 0;
                    foreach (KeyValuePair<int, int> entry in playerInventory.items)
                    {
                        inventorySaveData.playerItemsUUIDS[itemIndex] = entry.Key;
                        inventorySaveData.playerItemsStack[itemIndex] = entry.Value;
                        ++itemIndex;
                    }
                }

                inventorySaveData.playerCurrencyAmount = playerInventory.currencyAmount;
            }

            return inventorySaveData;
        }

        public virtual void ResetData()
        {
            playerInventory = new PlayerInventory();
        }

        public virtual void OnLoad(object generic)
        {
            InventorySaveData inventorySaveData = generic as InventorySaveData;

            playerInventory = new PlayerInventory();
            if (!DatabaseInventory.Load().inventorySettings.saveInventory ||
                inventorySaveData == null)
            {
                return;
            }

            SetCurrency(inventorySaveData.playerCurrencyAmount);
            int playerInventoryItemsCount = inventorySaveData.playerItemsUUIDS.Length;

            for (int i = 0; i < playerInventoryItemsCount; ++i)
            {
                playerInventory.items.Add(
                    inventorySaveData.playerItemsUUIDS[i],
                    inventorySaveData.playerItemsStack[i]
                );
            }
        }
    }
}