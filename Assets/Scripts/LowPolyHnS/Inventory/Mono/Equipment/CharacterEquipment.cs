using System;
using System.Collections;
using System.Collections.Generic;
using LowPolyHnS.Core;
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
            int counter = 0;
            for (int i = 0; i < equipment.items.Length; ++i)
            {
                if (equipment.items[i].isEquipped &&
                    equipment.items[i].itemID == itemID)
                {
                    counter += 1;
                }
            }

            return counter;
        }

        public bool HasEquipTypes(int itemTypes)
        {
            for (int i = 0; i < ItemType.MAX; ++i)
            {
                if (((itemTypes >> i) & 1) > 0 && !equipment.items[i].isEquipped)
                {
                    return false;
                }
            }

            return true;
        }

        public int GetEquip(int itemType)
        {
            return equipment.items[itemType].isEquipped
                ? equipment.items[itemType].itemID
                : 0;
        }

        public bool EquipItem(int itemID, int itemType, Action onEquip = null)
        {
            Inventory.Item item = InventoryManager.Instance.itemsCatalogue[itemID];
            if (!item.conditionsEquip.Check(gameObject))
            {
                return false;
            }

            List<int> itemTypes = new List<int>();
            if (InventoryManager.Instance.itemsCatalogue[itemID].fillAllTypes)
            {
                for (int i = 0; i < ItemType.MAX; ++i)
                {
                    if (((item.itemTypes >> i) & 1) > 0)
                    {
                        itemTypes.Add(i);
                    }
                }
            }
            else
            {
                itemTypes.Add(itemType);
            }

            int numToUnequip = 0;
            int numUnequipped = 0;

            for (int i = 0; i < itemTypes.Count; ++i)
            {
                if (equipment.items[itemTypes[i]].isEquipped)
                {
                    numToUnequip += 1;
                    UnequipItem(equipment.items[itemTypes[i]].itemID, () =>
                    {
                        numUnequipped += 1;
                        if (numUnequipped >= numToUnequip)
                        {
                            ExecuteActions(item.actionsOnEquip, onEquip);
                        }
                    });
                }

                equipment.items[itemTypes[i]].isEquipped = true;
                equipment.items[itemTypes[i]].itemID = itemID;
            }

            if (numToUnequip == 0) ExecuteActions(item.actionsOnEquip, onEquip);

            return true;
        }

        public bool UnequipItem(int itemID, Action onUnequip = null)
        {
            bool unequipped = false;

            int itemsToUnequip = 0;
            int itemsUnequipped = 0;

            for (int i = 0; i < equipment.items.Length; ++i)
            {
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

                    Actions actions = instance.GetComponent<Actions>();
                    actions.destroyAfterFinishing = true;
                    actions.onFinish.AddListener(() =>
                    {
                        itemsUnequipped += 1;
                        if (itemsUnequipped >= itemsToUnequip && onUnequip != null)
                        {
                            onUnequip.Invoke();
                        }
                    });

                    actions.Execute(gameObject);
                    unequipped = true;
                }
            }

            return unequipped;
        }

        public int[] UnequipTypes(int itemTypes)
        {
            List<int> unequipped = new List<int>();
            for (int i = 0; i < ItemType.MAX; ++i)
            {
                if (((itemTypes >> i) & 1) > 0)
                {
                    if (equipment.items[i].isEquipped)
                    {
                        unequipped.Add(equipment.items[i].itemID);
                        equipment.items[i].isEquipped = false;

                        GameObject instance = Instantiate(
                            InventoryManager
                                .Instance
                                .itemsCatalogue[equipment.items[i].itemID]
                                .actionsOnUnequip.gameObject,
                            transform.position,
                            transform.rotation
                        );

                        Actions actions = instance.GetComponent<Actions>();
                        actions.destroyAfterFinishing = true;
                        actions.Execute(gameObject);
                    }
                }
            }

            return unequipped.ToArray();
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void ExecuteActions(IActionsList list, Action callback = null)
        {
            GameObject instance = Instantiate(
                list.gameObject,
                transform.position,
                transform.rotation
            );

            Actions actions = instance.GetComponent<Actions>();
            actions.destroyAfterFinishing = true;
            actions.onFinish.AddListener(() =>
            {
                if (callback != null) callback.Invoke();
            });

            actions.Execute(gameObject);
        }

        // IGAMESAVE: -----------------------------------------------------------------------------

        public string GetUniqueName()
        {
            return string.Format("equip:{0}", GetUniqueID());
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
            if (!saveEquipment) return null;
            return equipment;
        }

        public void ResetData()
        {
            for (int i = 0; i < equipment.items.Length; ++i)
            {
                if (equipment.items[i].isEquipped)
                {
                    UnequipItem(equipment.items[i].itemID);
                }
            }

            equipment = new Equipment();
        }

        public void OnLoad(object generic)
        {
            Equipment savedEquipment = generic as Equipment;
            if (!saveEquipment || savedEquipment == null) return;

            StartCoroutine(OnLoadNextFrame(savedEquipment));
        }

        private IEnumerator OnLoadNextFrame(Equipment savedEquipment)
        {
            Debug.Log("On Load");

            yield return null;
            for (int i = 0; i < savedEquipment.items.Length; ++i)
            {
                if (savedEquipment.items[i].isEquipped)
                {
                    EquipItem(savedEquipment.items[i].itemID, i);
                }
            }
        }
    }
}