using System;
using System.Collections;
using System.Collections.Generic;
using LowPolyHnS.Attributes;
using LowPolyHnS.Characters;
using LowPolyHnS.Core;
using LowPolyHnS.Core.Hooks;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
    [DisallowMultipleComponent]
    [AddComponentMenu("LowPolyHnS/Characters/Equipment")]
    public class CharacterEquipment : GlobalID, IGameSave
    {
        [Serializable]
        public class Item
        {
            public bool isEquipped;
            public int itemID;

            public Item()
            {
                isEquipped = false;
                itemID = 0;
            }

            public Item(int itemID)
            {
                isEquipped = true;
                this.itemID = itemID;
            }
        }

        [Serializable]
        public class Equipment
        {
            public Item[] items;

            public Equipment()
            {
                items = new Item[ItemType.MAX]
                {
                    new Item(), new Item(), new Item(), new Item(),
                    new Item(), new Item(), new Item(), new Item(),
                    new Item(), new Item(), new Item(), new Item(),
                    new Item(), new Item(), new Item(), new Item(),
                    new Item(), new Item(), new Item(), new Item(),
                    new Item(), new Item(), new Item(), new Item(),
                    new Item(), new Item(), new Item(), new Item(),
                    new Item(), new Item(), new Item(), new Item()
                };
            }
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public Equipment equipment = new Equipment();
        public bool saveEquipment = true;

        // INITIALIZERS: --------------------------------------------------------------------------

        private void Start()
        {
            if (!Application.isPlaying) return;
            SaveLoadManager.Instance.Initialize(this);
        }

        private void OnDestroy()
        {
            OnDestroyGID();

            if (!Application.isPlaying) return;
            if (exitingApplication) return;
            SaveLoadManager.Instance.OnDestroyIGameSave(this);
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public virtual bool CanEquip(int itemID)
        {
            return true;
        }

        public int HasEquip(int itemID)
        {
            var counter = 0;
            for (var i = 0; i < equipment.items.Length; ++i)
                if (equipment.items[i].isEquipped &&
                    equipment.items[i].itemID == itemID)
                    counter += 1;

            return counter;
        }

        public bool HasEquipTypes(int itemTypes)
        {
            for (var i = 0; i < ItemType.MAX; ++i)
                if (((itemTypes >> i) & 1) > 0 && !equipment.items[i].isEquipped)
                    return false;

            return true;
        }

        public int GetEquip(int itemType)
        {
            return equipment.items[itemType].isEquipped
                ? equipment.items[itemType].itemID
                : 0;
        }

        public virtual bool EquipItem(int itemID, int itemType, Action onEquip = null)
        {
            var item = InventoryManager.Instance.itemsCatalogue[itemID];
            if (!item.conditionsEquip.Check(gameObject)) return false;

            var itemTypes = new List<int>();
            if (InventoryManager.Instance.itemsCatalogue[itemID].fillAllTypes)
            {
                for (var i = 0; i < ItemType.MAX; ++i)
                    if (((item.itemTypes >> i) & 1) > 0)
                        itemTypes.Add(i);
            }
            else
            {
                itemTypes.Add(itemType);
            }

            var numToUnequip = 0;
            var numUnequipped = 0;

            for (var i = 0; i < itemTypes.Count; ++i)
            {
                if (equipment.items[itemTypes[i]].isEquipped)
                {
                    numToUnequip += 1;
                    UnequipItem(equipment.items[itemTypes[i]].itemID, () =>
                    {
                        numUnequipped += 1;
                        if (numUnequipped >= numToUnequip) ExecuteActions(item.actionsOnEquip, onEquip);
                    });
                }

                AddCharacterModifierFromItem(item);

                equipment.items[itemTypes[i]].isEquipped = true;
                equipment.items[itemTypes[i]].itemID = itemID;
            }

            if (numToUnequip == 0) ExecuteActions(item.actionsOnEquip, onEquip);

            return true;
        }

        public bool UnequipItem(int itemID, Action onUnequip = null)
        {
            var unequipped = false;

            var itemsToUnequip = 0;
            var itemsUnequipped = 0;

            for (var i = 0; i < equipment.items.Length; ++i)
                if (equipment.items[i].isEquipped && equipment.items[i].itemID == itemID)
                {
                    Inventory.Item item = InventoryManager.Instance.itemsCatalogue[itemID];

                    equipment.items[i].isEquipped = false;

                    GameObject instance = Instantiate(
                        item.actionsOnUnequip.gameObject,
                        transform.position,
                        transform.rotation
                    );

                    itemsToUnequip += 1;

                    var actions = instance.GetComponent<Actions>();
                    actions.destroyAfterFinishing = true;
                    actions.onFinish.AddListener(() =>
                    {
                        itemsUnequipped += 1;
                        if (itemsUnequipped >= itemsToUnequip) onUnequip?.Invoke();
                    });

                    RemoveCharacterModifierFromSource(item);
                    actions.Execute(gameObject);
                    unequipped = true;
                }

            return unequipped;
        }

        public int[] UnequipTypes(int itemTypes)
        {
            var unequipped = new List<int>();
            for (var i = 0; i < ItemType.MAX; ++i)
                if (((itemTypes >> i) & 1) > 0)
                    if (equipment.items[i].isEquipped)
                    {
                        unequipped.Add(equipment.items[i].itemID);
                        equipment.items[i].isEquipped = false;

                        var instance = Instantiate(
                            InventoryManager
                                .Instance
                                .itemsCatalogue[equipment.items[i].itemID]
                                .actionsOnUnequip.gameObject,
                            transform.position,
                            transform.rotation
                        );

                        var actions = instance.GetComponent<Actions>();
                        actions.destroyAfterFinishing = true;
                        actions.Execute(gameObject);
                    }

            return unequipped.ToArray();
        }

        public void AddCharacterModifierFromItem(Inventory.Item item)
        {
            if (HookPlayer.Instance == null) return;

            var playerCharacter = HookPlayer.Instance.GetComponent<PlayerCharacter>();

            if (playerCharacter == null) return;

            if (item.StrengthBonus != 0)
            {
                playerCharacter.Strength.AddModifier(
                    new AttributeModifier(item.StrengthBonus, AttributeModifierType.Normal, item));
            }

            if (item.AgilityBonus != 0)
            {
                playerCharacter.Agility.AddModifier(new AttributeModifier(item.AgilityBonus, AttributeModifierType.Normal, item));
            }

            if (item.IntelligenceBonus != 0)
            {
                playerCharacter.Intelligence.AddModifier(new AttributeModifier(item.IntelligenceBonus,
                    AttributeModifierType.Normal, item));
            }

            if (Math.Abs(item.StrengthPercentBonus) > 0)
            {
                playerCharacter.Strength.AddModifier(new AttributeModifier(item.StrengthPercentBonus,
                    AttributeModifierType.PercentMultiply, item));
            }

            if (Math.Abs(item.AgilityPercentBonus) > 0)
            {
                playerCharacter.Agility.AddModifier(new AttributeModifier(item.AgilityPercentBonus,
                    AttributeModifierType.PercentMultiply, item));
            }

            if (Math.Abs(item.IntelligencePercentBonus) > 0)
            {
                playerCharacter.Intelligence.AddModifier(new AttributeModifier(item.IntelligencePercentBonus,
                    AttributeModifierType.PercentMultiply, item));
            }

            if (InventoryUIManager.Instance == null) return;

            if (InventoryUIManager.Instance.AttributesUIManager != null)
                InventoryUIManager.Instance.AttributesUIManager.UpdateAttributes(playerCharacter);
        }


        public void RemoveCharacterModifierFromSource(Inventory.Item item)
        {
            if (HookPlayer.Instance == null) return;

            PlayerCharacter playerCharacter = HookPlayer.Instance.GetComponent<PlayerCharacter>();

            if (playerCharacter == null) return;

            if (item.StrengthBonus != 0)
            {
                playerCharacter.Strength.RemoveAllModifiersFromSource(item);
            }

            if (item.AgilityBonus != 0)
            {
                playerCharacter.Agility.RemoveAllModifiersFromSource(item);
            }

            if (item.IntelligenceBonus != 0)
            {
                playerCharacter.Intelligence.RemoveAllModifiersFromSource(item);
            }


            if (InventoryUIManager.Instance == null)
            {
                return;
            }

            if (InventoryUIManager.Instance.AttributesUIManager != null)
            {
                InventoryUIManager.Instance.AttributesUIManager.UpdateAttributes(playerCharacter);
            }
        }

        #region PRIVATE METHODS

        private void ExecuteActions(IActionsList list, Action callback = null)
        {
            var instance = Instantiate(
                list.gameObject,
                transform.position,
                transform.rotation
            );

            var actions = instance.GetComponent<Actions>();
            actions.destroyAfterFinishing = true;
            actions.onFinish.AddListener(() => { callback?.Invoke(); });

            actions.Execute(gameObject);
        }

        #endregion


        #region IGAMESAVE

        public string GetUniqueName()
        {
            return $"equip:{GetUniqueID()}";
        }

        protected virtual string GetUniqueID()
        {
            return GetID();
        }

        public Type GetSaveDataType()
        {
            return typeof(Equipment);
        }

        public object GetSaveData()
        {
            return !saveEquipment ? null : equipment;
        }

        public void ResetData()
        {
            foreach (Item item in equipment.items)
            {
                if (item.isEquipped)
                {
                    UnequipItem(item.itemID);
                }
            }

            equipment = new Equipment();
        }

        public void OnLoad(object generic)
        {
            var savedEquipment = generic as Equipment;
            if (!saveEquipment || savedEquipment == null) return;

            StartCoroutine(OnLoadNextFrame(savedEquipment));
        }

        private IEnumerator OnLoadNextFrame(Equipment savedEquipment)
        {
            Debug.Log("On Load");

            yield return null;
            for (var i = 0; i < savedEquipment.items.Length; ++i)
                if (savedEquipment.items[i].isEquipped)
                    EquipItem(savedEquipment.items[i].itemID, i);
        }

        #endregion
    }
}