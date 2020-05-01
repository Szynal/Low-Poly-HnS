using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "FearEffect/Generic/ItemDatabase", order = 1)]
public class FE_ItemDatabase : ScriptableObject
{
    public List<FE_Weapon> Weapons = new List<FE_Weapon>();
    public List<FE_UseableItem> Useables = new List<FE_UseableItem>();

    public FE_Item GetItemByID(int _id)
    {
        foreach (FE_Weapon _wep in Weapons)
        {
            if (_wep.itemID == _id)
            {
                return _wep;
            }
        }

        foreach (FE_UseableItem _item in Useables)
        {
            if (_item.itemID == _id)
            {
                return _item;
            }
        }

        Debug.LogError("Item with ID: " + _id + " couldn't be found in ItemDatabase!");
        return null;
    }

    public void SortLists()
    {
        Weapons.Sort();
        Useables.Sort();
    }
}