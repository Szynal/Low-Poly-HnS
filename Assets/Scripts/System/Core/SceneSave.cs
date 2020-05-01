using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface ISaveable
{
    void OnSave(SceneSave _saveTo);
    void OnLoad(PlayerSaveState _loadState);
    void OnLoad(EnemySaveState _loadState);
    void OnLoad(PickupState _loadState);
    void OnLoad(MultipleStateObjectManagerState _loadState);
    void OnLoad(ActionTriggerState _loadState);
    void OnDestroy();
}

[Serializable]
public class SceneSave
{
    public int SceneID;

    private PlayerSaveState playerState;
    private MultipleStateObjectManagerState msoManagerState;
    private List<EnemySaveState> enemyStates = new List<EnemySaveState>();
    private List<PickupState> pickupStates = new List<PickupState>();
    private List<ActionTriggerState> triggerStates = new List<ActionTriggerState>();

    public void SaveScene(Scene _scene)
    {
        SceneID = _scene.buildIndex;

        foreach (MonoBehaviour _mb in Resources.FindObjectsOfTypeAll<MonoBehaviour>())
        {
            if (_mb.gameObject.scene.isLoaded && _mb.gameObject.scene == _scene)
            {
                if (_mb is ISaveable)
                {
                    // Debug.Log("Scene " + _scene.name + " is saving object: " + _mb.name);
                    ISaveable _saveable = (ISaveable) _mb;
                    _saveable.OnSave(this);
                }
                else if (_mb is MultipleStateObjectManager)
                {
                    MultipleStateObjectManager _msoManager = (MultipleStateObjectManager) _mb;
                    _msoManager.SaveState(this);
                }
            }
        }

        MultipleStateObjectManager.Instance.SaveState(this);
    }

    public void RecordSaveableState(SaveableState _newState)
    {
        //Zapisujemy do konkretnego arraya/miejsca, zaleznie od tego jaki jest typ
        if (_newState is PlayerSaveState)
        {
            playerState = (PlayerSaveState) _newState;
        }
        else if (_newState is EnemySaveState)
        {
            if (enemyStates.Contains((EnemySaveState) _newState) == false)
            {
                enemyStates.Add((EnemySaveState) _newState);
            }
        }
        else if (_newState is PickupState)
        {
            if (pickupStates.Contains((PickupState) _newState) == false)
            {
                pickupStates.Add((PickupState) _newState);
            }
        }
        else if (_newState is MultipleStateObjectManagerState)
        {
            msoManagerState = (MultipleStateObjectManagerState) _newState;
        }
        else if (_newState is ActionTriggerState)
        {
            triggerStates.Add((ActionTriggerState) _newState);
        }
    }

    public PlayerSaveState GetPlayerState()
    {
        return playerState;
    }

    public MultipleStateObjectManagerState GetMSOManagerState()
    {
        return msoManagerState;
    }

    public EnemySaveState GetEnemyStateByID(int _id)
    {
        EnemySaveState _ret = null;

        foreach (EnemySaveState _state in enemyStates)
        {
            if (_state.SaveableID == _id)
            {
                _ret = _state;
            }
        }

        return _ret;
    }

    public PickupState GetPickupStateByID(int _id)
    {
        PickupState _ret = null;

        foreach (PickupState _state in pickupStates)
        {
            if (_state.SaveableID == _id)
            {
                _ret = _state;
            }
        }

        return _ret;
    }

    public ActionTriggerState GetTriggerStateByID(int _id)
    {
        ActionTriggerState _ret = null;

        foreach (ActionTriggerState _state in triggerStates)
        {
            if (_state.SaveableID == _id)
            {
                _ret = _state;
            }
        }

        return _ret;
    }
}