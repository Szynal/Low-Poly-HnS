using System;
using System.Collections.Generic;
using LowPolyHnS;
using UnityEngine;

public enum EInputMode
{
    Full,
    InventoryOnly,
    MovementOnly,
    None
}

//This class handles accepting player input and delegating it to behaviours that handle it as  well as saving/loading player info
public class FE_PlayerInputController : MonoBehaviour, ISaveable
{
    public static FE_PlayerInputController Instance;

    [Header("States")] public bool AllowInventoryInput = true;
    public bool AllowInput = true;
    public bool UsingLadder = false;

    [Header("ID used for saving. Change only by using IDManager!")] [SerializeField]
    public int SaveableID = -1;

    public delegate void OpenCloseInventoryDelegate();

    public OpenCloseInventoryDelegate OnInventoryOpened;

    public delegate void InventoryInputDelegate(float _value);

    public InventoryInputDelegate InventoryLeftRightInput;
    public InventoryInputDelegate InventoryUpDownInput;

    private bool usedAboutFace;
    private FE_PlayerInventoryInteraction inventoryInteractionManager;
    private CharacterHealth healthScript;

    //In awake we check for references to handlers for given actions
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            inventoryInteractionManager = GetComponent<FE_PlayerInventoryInteraction>();
            healthScript = GetComponent<CharacterHealth>();
        }
    }


    public void OnDestroy()
    {
        if (SceneLoader.Instance == null)
        {
            return;
        }

        SceneLoader.Instance.ActiveSaveables.Remove(this);
    }

    public void ChangeInputMode(EInputMode newMode)
    {
        switch (newMode)
        {
            case EInputMode.None:
                AllowInput = false;
                AllowInventoryInput = false;
                break;

            case EInputMode.Full:
                AllowInput = true;
                AllowInventoryInput = true;
                break;

            case EInputMode.InventoryOnly:
                AllowInput = false;
                AllowInventoryInput = true;
                break;

            case EInputMode.MovementOnly:
                AllowInput = true;
                AllowInventoryInput = false;
                break;
        }
    }

    #region Saving/loading

    public void OnSave(SceneSave saveTo)
    {
        FE_PlayerSaveState saveState = new FE_PlayerSaveState
        {
            SaveableID = SaveableID, Health = healthScript.HealthCurrent
        };


        GameManager.Instance.SavedCharactersInventory.PlayerDisguised = saveState.PlayerDisguised;

        foreach (FE_Item item in inventoryInteractionManager.Inventory)
        {
            saveState.InventoryIDList_Player.Add(item.itemID);
        }

        GameManager.Instance.SavedCharactersInventory.PlayerInvList = saveState.InventoryIDList_Player;

        saveState.AmmoTable_Player = inventoryInteractionManager.Ammunition;
        GameManager.Instance.SavedCharactersInventory.PlayerSpellList = inventoryInteractionManager.Ammunition;


        saveState.Position_X = transform.position.x;
        saveState.Position_Y = transform.position.y;
        saveState.Position_Z = transform.position.z;

        saveState.Rotation_Y = transform.rotation.eulerAngles.y;

        saveTo.RecordSaveableState(saveState);
    }

    public void OnLoad(FE_EnemySaveState loadState)
    {
        throw new NotImplementedException();
    }

    public void OnLoad(FE_PickupState loadState)
    {
        throw new NotImplementedException();
    }

    public void OnLoad(FE_PlayerSaveState loadState)
    {
        if (loadState.Health > 0)
        {
            healthScript.HealthCurrent = loadState.Health;
        }

        int[] desiredAmmoTable = new int[0];

        if (loadState.AmmoTable_Player != null && loadState.AmmoTable_Player.Length > 0)
        {
            desiredAmmoTable = loadState.AmmoTable_Player;
        }
        else if (GameManager.Instance.SavedCharactersInventory.PlayerSpellList.Length > 0)
        {
            desiredAmmoTable = GameManager.Instance.SavedCharactersInventory.PlayerSpellList;
        }


        if (desiredAmmoTable != null && desiredAmmoTable.Length > 0)
        {
            inventoryInteractionManager.Ammunition = desiredAmmoTable;
        }

        List<int> desiredInvList = loadState.InventoryIDList_Player.Count > 0
            ? loadState.InventoryIDList_Player
            : GameManager.Instance.SavedCharactersInventory.PlayerInvList;


        if (desiredInvList != null && desiredInvList.Count > 0)
        {
            inventoryInteractionManager.Inventory.Clear();
            foreach (int id in desiredInvList)
            {
                inventoryInteractionManager.Inventory.Add(GameManager.Instance.ItemDatabase.GetItemByID(id));
            }
        }

        inventoryInteractionManager.InitializeInventory();

        if (loadState.InventoryIDList_Player.Count > 0)
        {
            GameManager.Instance.SavedCharactersInventory.PlayerInvList = loadState.InventoryIDList_Player;
            GameManager.Instance.SavedCharactersInventory.PlayerSpellList = loadState.AmmoTable_Player;
        }

        GameManager.Instance.SavedCharactersInventory.PlayerDisguised = loadState.PlayerDisguised;

        Vector3 loadPosition = new Vector3(loadState.Position_X, loadState.Position_Y, loadState.Position_Z);
        Quaternion loadRotation = Quaternion.Euler(0f, loadState.Rotation_Y, 0f);
    }

    public void OnLoad(MultipleStateObjectManagerState _loadState)
    {
        throw new NotImplementedException();
    }

    public void OnLoad(FE_ActionTriggerState _loadState)
    {
        throw new NotImplementedException();
    }

    #endregion
}