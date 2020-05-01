using System;
using UnityEngine;

//Parent class for both Useable and Weapon
public class FE_Item : ScriptableObject, IComparable<FE_Item>
{
    [Header("Inherited from Item.cs")] public int itemID;
    public string ItemName = "New Item"; //Name as shown in inventory
    public Sprite ItemSprite;
    public int ItemUses = 1;

    //Called when using useable/equiping weapon
    public virtual void Activate(FE_PlayerInventoryInteraction _instigator)
    {
    }

    public int CompareTo(FE_Item _compareTo)
    {
        if (_compareTo == null)
        {
            return 1;
        }

        return itemID.CompareTo(_compareTo.itemID);
    }
}