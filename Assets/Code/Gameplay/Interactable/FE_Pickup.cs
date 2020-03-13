using System.Collections;
using System.Collections.Generic;
using LowPolyHnS;
using UnityEngine;

[System.Serializable]
public class FE_Pickup : FE_InteractableObject, ISaveable
{
    [Header("Properties for Pickup")]
    [SerializeField] float rotationSpeed = 35f;
    public int PickupItemID;

    [Header("ID used for saving. Change only by using IDManager!")]
    [SerializeField] public int SaveableID = -1;

    protected override void Awake()
    {
        base.Awake();

        if(SaveableID < 0)
        {
            Debug.LogWarning("Saveable object " + gameObject.name + " has an invalid saveableID set up. It will cause erroneous behaviour when saving or loading the game!");
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
        _playerInventory.AddItem(Instantiate(GetPickupItem()));
  
        Destroy(gameObject);
    }

    public FE_Item GetPickupItem()
    {
        return GameManager.Instance.ItemDatabase.GetItemByID(PickupItemID);
    }

    public void OnSave(SceneSave _saveTo)
    {
        PickupState _saveState = new PickupState();

        _saveState.SaveableID = SaveableID;

        _saveState.ItemID = PickupItemID;

        _saveState.Position_X = transform.localPosition.x;
        _saveState.Position_Y = transform.localPosition.y;
        _saveState.Position_Z = transform.localPosition.z;

        _saveTo.RecordSaveableState(_saveState);
    }

    public void OnLoad(PlayerSaveState _loadState)
    {
        throw new System.NotImplementedException();
    }

    public void OnLoad(EnemySaveState _loadState)
    {
        throw new System.NotImplementedException();
    }

    public void OnLoad(PickupState _loadState)
    {
        PickupItemID = _loadState.ItemID;

        transform.localPosition = new Vector3(_loadState.Position_X, _loadState.Position_Y, _loadState.Position_Z);
    }

    protected override void OnDestroy()
    {
        SceneLoader.Instance.ActiveSaveables.Remove(this);
        base.OnDestroy();
    }

    public void OnLoad(MultipleStateObjectManagerState _loadState)
    {
        throw new System.NotImplementedException();
    }

    public void OnLoad(ActionTriggerState _loadState)
    {
        throw new System.NotImplementedException();
    }

    void ISaveable.OnDestroy()
    {
        OnDestroy();
    }
}
