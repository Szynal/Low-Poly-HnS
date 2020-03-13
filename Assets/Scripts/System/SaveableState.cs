using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveableState
{
    public int SaveableID;
}

[Serializable]
public class PlayerSaveState : SaveableState
{
    public int Health;
    public int[] AmmoTable_Player;
    public List<int> InventoryIDList_Player = new List<int>();
    public int CurrentWeaponID_Player;
    public int CurrentWeaponAmmo_Player;
    public bool PlayerDisguised;

    public float Position_X;
    public float Position_Y;
    public float Position_Z;

    public float Rotation_Y;
}

public class PlayerLoadParams
{
    public Vector3 CustomLoadPos;
    public Vector3 CustomLoadRot;
    public bool OverwriteTransform;
    public bool OverwriteInventory;
    public bool OverwriteHealth;

    public PlayerLoadParams(bool transformOverwrite = false, Vector3 customPos = default,
        Vector3 customRot = default, bool inventoryOverwrite = false, bool healthOverwrite = false)
    {
        OverwriteTransform = transformOverwrite;
        CustomLoadPos = customPos;
        CustomLoadRot = customRot;
        OverwriteInventory = inventoryOverwrite;
        OverwriteHealth = healthOverwrite;
    }
}

[Serializable]
public class EnemySaveState : SaveableState
{
    public int TaskIndex;
    public bool IsSearching;
    public int Health;

    public float Position_X;
    public float Position_Y;
    public float Position_Z;
    public float Rotation_Y;
}

[Serializable]
public class PickupState : SaveableState
{
    public int ItemID;
    public int AmmoID;
    public int AmmoAmount;

    public float Position_X;
    public float Position_Y;
    public float Position_Z;
}

[Serializable]
public class MultipleStateObjectManagerState : SaveableState
{
    public List<int> Keys = new List<int>();
    public List<int> Values = new List<int>();
    public List<string> Names = new List<string>();
}

[Serializable]
public class ActionTriggerState : SaveableState
{
    public bool WasUsedAlready;
}