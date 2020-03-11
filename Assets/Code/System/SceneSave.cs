using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public interface ISaveable
{
    void OnSave(SceneSave _saveTo);
    void OnLoad(FE_PlayerSaveState _loadState);
    void OnLoad(FE_EnemySaveState _loadState);
    void OnLoad(FE_PickupState _loadState);
    void OnLoad(MultipleStateObjectManagerState _loadState);
    void OnLoad(FE_ActionTriggerState _loadState);
    void OnDestroy();
}

[System.Serializable]
public class SceneSave
{
    public int SceneID;

    private FE_PlayerSaveState playerState = null;
    private MultipleStateObjectManagerState msoManagerState = null;
    private List<FE_EnemySaveState> enemyStates = new List<FE_EnemySaveState>();
    private List<FE_PickupState> pickupStates = new List<FE_PickupState>();
    private List<FE_ActionTriggerState> triggerStates = new List<FE_ActionTriggerState>();

    public void SaveScene(Scene _scene)
    {
        SceneID = _scene.buildIndex;

        foreach (MonoBehaviour _mb in Resources.FindObjectsOfTypeAll<MonoBehaviour>())
        {
            if (_mb.gameObject.scene.isLoaded == true && _mb.gameObject.scene == _scene)
            {
                if (_mb is ISaveable)
                {
                   // Debug.Log("Scene " + _scene.name + " is saving object: " + _mb.name);
                    ISaveable _saveable = (ISaveable)_mb;
                    _saveable.OnSave(this);
                }
                else if (_mb is MultipleStateObjectManager)
                {
                    MultipleStateObjectManager _msoManager = (MultipleStateObjectManager)_mb;
                    _msoManager.SaveState(this);
                }
            }
        }

        MultipleStateObjectManager.Instance.SaveState(this);
    }

    public void RecordSaveableState(FE_SaveableState _newState)
    {
        //Zapisujemy do konkretnego arraya/miejsca, zaleznie od tego jaki jest typ
        if(_newState is FE_PlayerSaveState)
        {
            playerState = (FE_PlayerSaveState)_newState;
        }
        else if(_newState is FE_EnemySaveState)
        {
            if (enemyStates.Contains((FE_EnemySaveState)_newState) == false)
            {
                enemyStates.Add((FE_EnemySaveState)_newState);
            }
        }
        else if(_newState is FE_PickupState)
        {
            if (pickupStates.Contains((FE_PickupState)_newState) == false)
            {
                pickupStates.Add((FE_PickupState)_newState);
            }
        }
        else if(_newState is MultipleStateObjectManagerState)
        {
            msoManagerState = (MultipleStateObjectManagerState)_newState;
        }
        else if(_newState is FE_ActionTriggerState)
        {
            triggerStates.Add((FE_ActionTriggerState)_newState);
        }
    }
    
    public FE_PlayerSaveState GetPlayerState()
    {
        return playerState;
    }

    public MultipleStateObjectManagerState GetMSOManagerState()
    {
        return msoManagerState;
    }

    public FE_EnemySaveState GetEnemyStateByID(int _id)
    {
        FE_EnemySaveState _ret = null;

        foreach (FE_EnemySaveState _state in enemyStates)
        {
            if (_state.SaveableID == _id)
            {
                _ret = _state;
            }
        }

        return _ret;
    }

    public FE_PickupState GetPickupStateByID(int _id)
    {
        FE_PickupState _ret = null;

        foreach (FE_PickupState _state in pickupStates)
        {
            if (_state.SaveableID == _id)
            {
                _ret = _state;
            }
        }

        return _ret;
    } 

    public FE_ActionTriggerState GetTriggerStateByID(int _id)
    {
        FE_ActionTriggerState _ret = null;

        foreach (FE_ActionTriggerState _state in triggerStates)
        {
            if (_state.SaveableID == _id)
            {
                _ret = _state;
            }
        }

        return _ret;
    }
}
