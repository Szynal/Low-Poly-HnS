using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FE_SaveableState
{
    public int SaveableID;
}

[System.Serializable]
public class FE_PlayerSaveState : FE_SaveableState
{
    public int Health;
    public int[] AmmoTable_Player;
    public int[] AmmoTable_Glas;
    public int[] AmmoTable_Deke;
    public List<int> InventoryIDList_Player = new List<int>();
    public List<int> InventoryIDList_Glas = new List<int>();
    public List<int> InventoryIDList_Deke = new List<int>();
    public int CurrentWeaponID_Player;
    public int CurrentWeaponID_Glas;
    public int CurrentWeaponID_Deke;
    public int CurrentWeaponAmmo_Player;
    public int CurrentWeaponAmmo_Glas;
    public int CurrentWeaponAmmo_Deke;
    public bool PlayerDisguised;

    public float Position_X;
    public float Position_Y;
    public float Position_Z;

    public float Rotation_Y;
}

public class FE_PlayerLoadParams
{
    public Vector3 CustomLoadPos;
    public Vector3 CustomLoadRot;
    public bool OverwriteTransform;
    public bool OverwriteInventory;
    public bool OverwriteHealth;

    public FE_PlayerLoadParams(bool _transformOverwrite = false, Vector3 _customPos = default, Vector3 _customRot = default, bool _inventoryOverwrite = false, bool _healthOverwrite = false)
    {
        OverwriteTransform = _transformOverwrite;
        CustomLoadPos = _customPos;
        CustomLoadRot = _customRot;
        OverwriteInventory = _inventoryOverwrite;
        OverwriteHealth = _healthOverwrite;
    }
}

[System.Serializable]
public class FE_EnemySaveState : FE_SaveableState
{
    public int TaskIndex;
    public bool IsSearching;

    public int Health;

    public float Position_X;
    public float Position_Y;
    public float Position_Z;

    public float Rotation_Y;
}

[System.Serializable]
public class FE_PickupState : FE_SaveableState
{
    public int ItemID;
    public int AmmoID;
    public int AmmoAmount;

    public float Position_X;
    public float Position_Y;
    public float Position_Z;
}

[System.Serializable]
public class MultipleStateObjectManagerState : FE_SaveableState
{
    public List<int> Keys = new List<int>();
    public List<int> Values = new List<int>();
    public List<string> Names = new List<string>();
}

[System.Serializable]
public class FE_ActionTriggerState : FE_SaveableState
{
    public bool WasUsedAlready;
}