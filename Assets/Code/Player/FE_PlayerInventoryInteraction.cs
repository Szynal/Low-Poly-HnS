using System.Collections;
using System.Collections.Generic;
using LowPolyHnS;
using UnityEngine;

[System.Serializable]
public class FE_PlayerInventoryInteraction : MonoBehaviour
{
    public static FE_PlayerInventoryInteraction Instance;

    //------------------------------------- Fields - inventory
    [Header("Inventory screen visibility properties")]             
    public List<FE_Item> Inventory = new List<FE_Item>();               
    private const int AMMO_MAX = 999;

    public int[] Ammunition = default;                        //Ammunition we posses. Index of given type is (int)EAmmoType
    public int GetCurrentAmmoCount(EAmmoType _ammoType)
    {
        if (Ammunition.Length <= (int)_ammoType)
        {
            Debug.LogError(_ammoType.ToString() + "  has higher index than ammunition table's length!");
            return -1;
        }
        return Ammunition[(int)_ammoType];
    }
    public void SetCurrentAmmoCount(EAmmoType _ammoType, int _amount)
    {
        if (Ammunition.Length <= (int)_ammoType)
        {
            Debug.LogError(_ammoType.ToString() + "  has higher index than ammunition table's length!");
            return;
        }

        Ammunition[(int)_ammoType] = _amount;
        Mathf.Clamp(Ammunition[(int)_ammoType], 0, AMMO_MAX);
    }

    //---------------------------------- Fields - interaction
    private List<FE_InteractableObject> objectsToUse = new List<FE_InteractableObject>();
    public List<FE_InteractableObject> GetObjectsToUse() { return objectsToUse; }

     public FE_PlayerInputController InputController = null;

    private FE_UIController uiController;
    private bool refreshingInteraction;

    #region Awake, Start, Initialization
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            InitializeInventory();
        }
    }

    private void Start()
    {
       
        uiController = FE_UIController.Instance;
        InputController = FE_PlayerInputController.Instance;
    }

    public void InitializeInventory()
    {
        if (Inventory.Count > 0)
        {
            for (int i = 0; i < Inventory.Count; i++)
            {
                if (Inventory[i] != null)
                {
                    Inventory[i] = Instantiate(Inventory[i]);
                }
            }
        }

        Inventory.Sort();
    }

    #endregion

    public void UseItem(FE_Item _itemToUse)
    {
        if(_itemToUse != null && inventoryContains(_itemToUse) == true)
        {
            uiController.CloseInventory();
            _itemToUse.Activate(this);
        }
    }

    #region Adding/removing things to inventory
    public void AddItem(FE_Item _newItem)
    {
        FE_UseableItem _useableToAdd = _newItem as FE_UseableItem;

        if (inventoryContains(_newItem) == false)
        {
            Inventory.Add(_newItem);
        }
        else if(_useableToAdd != null && _useableToAdd.IsStackable == true)
        {
            if(GetInventoryItemByID(_useableToAdd.itemID).ItemUses < _useableToAdd.MaxStacks - 1)
            {
                GetInventoryItemByID(_useableToAdd.itemID).ItemUses += 1;
            }
        }
        else
        {
            if(_newItem is FE_Weapon) //If we're picking up a weapon we already have, we take its' mag ammo and add to our max ammo
            {
                FE_Weapon _wep = (FE_Weapon)_newItem;
                AddAmmo(_wep.AmmoType, _wep.MagAmmoMax);

                //Adding the dual wielded weapon if we have only 1 of them
                if(_wep.DualWieldedVariant != null && inventoryContains(_wep.DualWieldedVariant) == false)
                {
                    AddItem(Instantiate(GameManager.Instance.ItemDatabase.GetItemByID(_wep.DualWieldedVariant.itemID)));
                }
            }
        }

        Inventory.Sort();
    }

    public void RemoveItem(FE_Item _itemToRemove)
    {
        if(_itemToRemove == null)
        {
            return;
        }

        if (inventoryContains(_itemToRemove) == true)
        {
            Inventory.Remove(GetInventoryItemByID(_itemToRemove.itemID));
        }

        Inventory.Sort();
    }

    public bool AddAmmo(EAmmoType _ammoType, int _amount)
    {
        int _ammoTypeIndex = (int)_ammoType;
        if (Ammunition.Length <= _ammoTypeIndex)
        {
            Debug.LogError(_ammoType.ToString() + "  has higher index than ammunition table's length!");
            return false;
        }

        if(AMMO_MAX > Ammunition[_ammoTypeIndex]) //We can only add more ammo if we're not at maximum
        {
            Ammunition[_ammoTypeIndex] += _amount;
            Mathf.Clamp(Ammunition[_ammoTypeIndex], 0, AMMO_MAX);

            return true;
        }

        return false;
    }
    #endregion

    #region Searching through inventory
    public List<FE_Item> GetUseableInventory()
    {
        List<FE_Item> _retList = new List<FE_Item>();

        foreach (FE_Item _item in Inventory)
        {
            if (_item is FE_Weapon == false)
            {
                _retList.Add(_item);
            }
        }

        return _retList;
    }

    public List<FE_Item> GetWeaponInventory()
    {
        List<FE_Item> _retList = new List<FE_Item>();

        foreach (FE_Item _item in Inventory)
        {
            if (_item is FE_Weapon == true)
            {
                _retList.Add(_item);
            }
        }

        return _retList;
    }

    private bool inventoryContains(FE_Item _searchedItem)
    {
        foreach(FE_Item _item in Inventory)
        {
            if(_item.itemID == _searchedItem.itemID)
            {
                return true;
            }
        }

        return false;
    }

    public FE_Item GetInventoryItemByID(int _id)
    {
        for(int i = 0; i < Inventory.Count; i++)
        {
            if(Inventory[i].itemID == _id)
            {
                return Inventory[i];
            }
        }

        return null;
    }
    #endregion

    #region Interaction
    //Called when trying to interact with the enviorment
    public void Interact()
    {
        if (objectsToUse.Count > 0 && FE_PlayerInputController.Instance.AllowInput == true) //We can't interact if we don't allow input
        {
            GetInteraction().Activate(this);
        }
    }

    //Adds an interaction to the interaction list
    public void AddInteraction(FE_InteractableObject _obj)
    {
        if (objectsToUse.Contains(_obj) == false)
        {
            objectsToUse.Add(_obj);
            uiController.ChangePanelVisibility(true, this);
            refreshingInteraction = true;
            StartCoroutine(refreshInteraction());
        }
        else
        {
            Debug.LogError(_obj.name + " wants to be added to objectsToUse, but it's already here!");
        }
    }
    //Removes interaction from the list
    public void RemoveInteraction(FE_InteractableObject _obj)
    {
        if (objectsToUse.Contains(_obj) == true)
        {
            objectsToUse.Remove(_obj);
            if (objectsToUse.Count <= 0)
            {
                uiController.ChangePanelVisibility(false, this);
                refreshingInteraction = false;
            }
            else
            {
                uiController.ChangePanelVisibility(true, this);
            }
        }
    }

    private IEnumerator refreshInteraction()
    {
        while (refreshingInteraction == true)
        {
            uiController.ChangePanelVisibility(true, this);

            yield return null;
        }
    }

    public void RefreshInteractions()
    {
        if (objectsToUse.Count <= 0)
        {
            FE_UIController.Instance.ChangePanelVisibility(false, this);
        }
        else
        {
            FE_UIController.Instance.ChangePanelVisibility(true, this);
        }
    }

    public bool HasInteraction()
    {
        if (objectsToUse.Count > 0)
        {
            return true;
        }

        return false;
    }

    public FE_InteractableObject GetInteraction()
    {
        if (objectsToUse.Count <= 0)
        {
            return null;
        }

        FE_InteractableObject _ret = null;
        float _closestDist = float.MaxValue;

        foreach (FE_InteractableObject _object in objectsToUse)
        {
            if (Vector3.Distance(transform.position, _object.transform.position) < _closestDist)
            {
                _ret = _object;
            }
        }

        return _ret;
    }
    #endregion
}
