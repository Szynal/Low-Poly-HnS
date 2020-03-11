using System;
using System.Collections.Generic;
using LowPolyHnS;
using UnityEngine;

public enum EPlayerCharacter
{
    Hana,
    Glas,
    Deke
}

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

    [Header("Settings")] [SerializeField] private bool useCharacterBasedMovement;
    public EPlayerCharacter CurrentCharacter = 0;

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

    private void Start()
    {
        useCharacterBasedMovement = GameManager.Instance.Settings.UseCharacterBasedMovement;
    }

    public void OnDestroy()
    {
        if (SceneLoader.Instance == null)
        {
            return;
        }

        SceneLoader.Instance.ActiveSaveables.Remove(this);
    }

    public void ChangeInputMode(EInputMode _newMode)
    {
        switch (_newMode)
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

    public void OnSave(SceneSave _saveTo)
    {
        FE_PlayerSaveState _saveState = new FE_PlayerSaveState();
        _saveState.SaveableID = SaveableID;

        _saveState.Health = healthScript.HealthCurrent;

        if (CurrentCharacter == EPlayerCharacter.Hana)
        {
            GameManager.Instance.SavedCharactersInventory.PlayerDisguised = _saveState.PlayerDisguised;

            foreach (FE_Item _item in inventoryInteractionManager.Inventory)
            {
                _saveState.InventoryIDList_Player.Add(_item.itemID);
            }

            GameManager.Instance.SavedCharactersInventory.PlayerInvList = _saveState.InventoryIDList_Player;

            _saveState.AmmoTable_Player = inventoryInteractionManager.Ammunition;
            GameManager.Instance.SavedCharactersInventory.PlayerSpellList = inventoryInteractionManager.Ammunition;
        }
        else
        {
            _saveState.PlayerDisguised = GameManager.Instance.SavedCharactersInventory.PlayerDisguised;

            foreach (FE_Item _item in inventoryInteractionManager.Inventory)
            {
                _saveState.InventoryIDList_Deke.Add(_item.itemID);
            }

            _saveState.AmmoTable_Deke = inventoryInteractionManager.Ammunition;
            _saveState.InventoryIDList_Player = GameManager.Instance.SavedCharactersInventory.PlayerInvList;
            _saveState.AmmoTable_Player = GameManager.Instance.SavedCharactersInventory.PlayerSpellList;
        }
        
        _saveState.Position_X = transform.position.x;
        _saveState.Position_Y = transform.position.y;
        _saveState.Position_Z = transform.position.z;

        _saveState.Rotation_Y = transform.rotation.eulerAngles.y;

        _saveTo.RecordSaveableState(_saveState);
    }

    public void OnLoad(FE_EnemySaveState _loadState)
    {
        throw new NotImplementedException();
    }

    public void OnLoad(FE_PickupState _loadState)
    {
        throw new NotImplementedException();
    }

    public void OnLoad(FE_PlayerSaveState _loadState)
    {
        if (_loadState.Health > 0)
        {
            healthScript.HealthCurrent = _loadState.Health;
        }

        int[] _desiredAmmoTable = new int[0];
        int _desiredWeaponID = -1;
        int _desiredWeaponAmmo = 0;


        if (_loadState.PlayerDisguised)
        {
        }

        _desiredWeaponID = _loadState.CurrentWeaponID_Player;
        _desiredWeaponAmmo = _loadState.CurrentWeaponAmmo_Player;
        if (_loadState.AmmoTable_Player != null && _loadState.AmmoTable_Player.Length > 0)
        {
            _desiredAmmoTable = _loadState.AmmoTable_Player;
        }
        else if (GameManager.Instance.SavedCharactersInventory.PlayerSpellList.Length > 0)
        {
            _desiredAmmoTable = GameManager.Instance.SavedCharactersInventory.PlayerSpellList;
        }


        if (_desiredAmmoTable != null && _desiredAmmoTable.Length > 0)
        {
            inventoryInteractionManager.Ammunition = _desiredAmmoTable;
        }

        List<int> _desiredInvList = new List<int>();

        if (_loadState.InventoryIDList_Player.Count > 0)
        {
            _desiredInvList = _loadState.InventoryIDList_Player;
        }
        else
        {
            _desiredInvList = GameManager.Instance.SavedCharactersInventory.PlayerInvList;
        }


        if (_desiredInvList != null && _desiredInvList.Count > 0)
        {
            inventoryInteractionManager.Inventory.Clear();
            foreach (int _id in _desiredInvList)
            {
                inventoryInteractionManager.Inventory.Add(GameManager.Instance.ItemDatabase.GetItemByID(_id));
            }
        }

        inventoryInteractionManager.InitializeInventory();

        if (_desiredWeaponID != -1)
        {
            foreach (FE_Item _item in inventoryInteractionManager.Inventory)
            {
                if (_item.itemID == _desiredWeaponID)
                {
                }
            }
        }


        if (_loadState.InventoryIDList_Player.Count > 0)
        {
            GameManager.Instance.SavedCharactersInventory.PlayerInvList = _loadState.InventoryIDList_Player;
            GameManager.Instance.SavedCharactersInventory.PlayerSpellList = _loadState.AmmoTable_Player;
        }

        GameManager.Instance.SavedCharactersInventory.PlayerDisguised = _loadState.PlayerDisguised;

        Vector3 _loadPosition = new Vector3(_loadState.Position_X, _loadState.Position_Y, _loadState.Position_Z);
        Quaternion _loadRotation = Quaternion.Euler(0f, _loadState.Rotation_Y, 0f);
        //    FE_PlayerCombatManager.Instance.HolsterWeapon(); //Why this?
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