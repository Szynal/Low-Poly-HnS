using System;
using System.Collections.Generic;
using LowPolyHnS;
using UnityEngine;

public enum EStateMessage
{
    DoorOpened,
    DoorClosed,
    ObjectDestroyed,
    FirstButtonUsed,
    SecondButtonUsed,
    ThirdButtonUsed,
    FourthButtonUsed,
    FifthButtonUsed,
    PuzzleFinished,
    FirstTriggerEntered,
    SecondTriggerEntered,
    ThirdTriggerEntered,
    FourthTriggerEntered,
    FifthTriggerEntered,
    BossDefeated,
    BossStunned
}

[Serializable]
public class FE_CauseEffectAssociation
{
    public EStateMessage Key;
    public int Value;
}

public class FE_MultipleStateObject : MonoBehaviour, ISaveable
{
    public int CurrentStateID;
    public List<GameObject> States = new List<GameObject>();
    public List<FE_CauseEffectAssociation> CauseEffectAssociations = new List<FE_CauseEffectAssociation>();

    [Header("ID used for saving. Change only by using IDManager!")] [SerializeField]
    public int SaveableID = -1;

    private void Start()
    {
        if (MultipleStateObjectManager.Instance.GetStateByID(SaveableID) == -1)
        {
            MultipleStateObjectManager.Instance.RecordState(SaveableID, CurrentStateID, gameObject.name);
        }
    }

    private void handleState(int _stateID)
    {
        if (_stateID != CurrentStateID)
        {
            CurrentStateID = _stateID;

            for (int i = 0; i < States.Count; i++)
            {
                if (States[i] != null)
                {
                    if (i != _stateID)
                    {
                        States[i].SetActive(false);
                    }
                    else
                    {
                        States[i].SetActive(true);
                    }
                }
                else
                {
                    Debug.LogWarning("Multi state object " + name + " has a null state with index " + i +
                                     " . It may cause problems!");
                }
            }
        }
    }

    public void ChangeState(EStateMessage _newState, bool _changeObjects)
    {
        foreach (FE_CauseEffectAssociation _union in CauseEffectAssociations)
        {
            if (_union.Key == _newState)
            {
                if (_changeObjects)
                {
                    handleState(_union.Value);
                }
                else
                {
                    CurrentStateID = _union.Value;
                }

                MultipleStateObjectManager.Instance.RecordState(SaveableID, _union.Value, gameObject.name);
                return;
            }
        }
    }

    public void ChangeStateByID(int _newStateID, bool _changeObjects)
    {
        foreach (FE_CauseEffectAssociation _union in CauseEffectAssociations)
        {
            if (_union.Value == _newStateID)
            {
                ChangeState(_union.Key, _changeObjects);
                return;
            }
        }
    }

    public void OnDestroy()
    {
        SceneLoader.Instance.ActiveSaveables.Remove(this);
    }

    public void OnLoad(PlayerSaveState _loadState)
    {
        throw new NotImplementedException();
    }

    public void OnLoad(EnemySaveState _loadState)
    {
        throw new NotImplementedException();
    }

    public void OnLoad(PickupState _loadState)
    {
        throw new NotImplementedException();
    }

    public void OnLoad(MultipleStateObjectManagerState _loadState)
    {
        throw new NotImplementedException();
    }

    public void OnLoad(int _loadedState)
    {
        handleState(_loadedState);
    }

    public void OnLoad(ActionTriggerState _loadState)
    {
        throw new NotImplementedException();
    }

    public void OnSave(SceneSave _saveTo)
    {
        /*   MultipleStateObjectManagerState _saveState = new MultipleStateObjectManagerState();
           _saveState.SaveableID = SaveableID;
           _saveState.StateID = CurrentStateID;
   
           _saveTo.RecordSaveableState(_saveState);*/
    }
}