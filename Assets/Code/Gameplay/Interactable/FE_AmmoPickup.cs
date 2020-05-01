using System;
using LowPolyHnS;
using UnityEngine;

public class FE_AmmoPickup : FE_InteractableObject, ISaveable
{
    [Header("Properties for AmmoPickup")] [SerializeField]
    private Sprite ammoSprite = default;

    public Sprite GetSprite()
    {
        return ammoSprite;
    }

    [SerializeField] private EAmmoType pickupAmmoType;

    public EAmmoType GetAmmoType()
    {
        return pickupAmmoType;
    }

    [SerializeField] private int pickupAmount;

    public int GetAmount()
    {
        return pickupAmount;
    }

    [SerializeField] private float rotationSpeed = 35f;

    [Header("ID used for saving. Change only by using IDManager!")] [SerializeField]
    public int SaveableID = -1;

    protected override void Awake()
    {
        base.Awake();

        if (SaveableID < 0)
        {
            Debug.LogWarning("Saveable object " + gameObject.name +
                             " has an invalid saveableID set up. It will cause erroneous behaviour when saving or loading the game!");
        }
    }

    protected override void onActivation(FE_PlayerInventoryInteraction _instigator)
    {
        OnPickup(_instigator);
    }

    private void Update()
    {
        transform.Rotate(new Vector3(0f, rotationSpeed * Time.deltaTime, 0f));
    }

    private void OnPickup(FE_PlayerInventoryInteraction _playerInventory)
    {
        if (_playerInventory.AddAmmo(pickupAmmoType, pickupAmount))
        {
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Ammo on " + gameObject.name + " couldn't be picked up...");
        }
    }

    public void OnSave(SceneSave _saveTo)
    {
        PickupState _saveState = new PickupState();

        _saveState.SaveableID = SaveableID;

        _saveState.AmmoID = (int) pickupAmmoType;
        _saveState.AmmoAmount = pickupAmount;

        _saveState.Position_X = transform.position.x;
        _saveState.Position_Y = transform.position.y;
        _saveState.Position_Z = transform.position.z;

        _saveTo.RecordSaveableState(_saveState);
    }

    public void OnLoad(PlayerSaveState _loadState)
    {
        throw new NotImplementedException();
    }

    public void OnLoad(EnemySaveState _loadState)
    {
        throw new NotImplementedException();
    }

    public void OnLoad(MultipleStateObjectManagerState _loadState)
    {
        throw new NotImplementedException();
    }

    public void OnLoad(PickupState _loadState)
    {
        pickupAmmoType = (EAmmoType) _loadState.AmmoID;
        pickupAmount = _loadState.AmmoAmount;

        transform.position = new Vector3(_loadState.Position_X, _loadState.Position_Y, _loadState.Position_Z);
    }

    protected override void OnDestroy()
    {
        SceneLoader.Instance.ActiveSaveables.Remove(this);
        base.OnDestroy();
    }

    public void OnLoad(ActionTriggerState _loadState)
    {
        throw new NotImplementedException();
    }

    void ISaveable.OnDestroy()
    {
        OnDestroy();
    }
}