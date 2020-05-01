using System;
using System.Collections.Generic;
using UnityEngine;

public class FE_ActionTrigger : MonoBehaviour, ISaveable
{
    [Header("Step trigger properties")] [SerializeField]
    protected bool oneShotOnly = true;

    [SerializeField] protected bool hasParent;
    [SerializeField] protected bool triggeredByAnEnemy = false;
    protected FE_MultipleStateObject parentScript;

    [SerializeField] protected EStateMessage messageToSendOnTrigger = EStateMessage.FirstTriggerEntered;

    [Header("State changer properties")]
    public List<FE_ActionContainer> AfterUseObjectChanges = new List<FE_ActionContainer>();

    [Header("Saveable properties")] public int SaveableID = -1;

    public bool BeenUsedAlready;

    protected void Awake()
    {
        if (hasParent)
        {
            parentScript = GetComponentInParent<FE_MultipleStateObject>();

            if (parentScript == null)
            {
                Debug.LogError("StepInTrigger " + gameObject.name +
                               " has 'hasParent' set to true, but it couldn't find the parent script!");
                hasParent = false;
            }
        }
    }

    protected virtual void OnTriggerEnter(Collider _other)
    {
        if (canBeTriggered())
        {
            if (triggeredByAnEnemy == false && _other.tag != "Player")
            {
                return;
            }

            if (triggeredByAnEnemy && _other.tag == "Player")
            {
                return;
            }

            BeenUsedAlready = true;

            foreach (FE_ActionContainer _state in AfterUseObjectChanges)
            {
                FE_StateChanger.HandleObjectChange(_state);
            }

            if (hasParent)
            {
                parentScript.ChangeState(messageToSendOnTrigger, true);
            }
        }
    }

    protected bool canBeTriggered()
    {
        if (oneShotOnly && BeenUsedAlready)
        {
            return false;
        }

        return true;
    }

    public void OnSave(SceneSave _saveTo)
    {
        ActionTriggerState _saveState = new ActionTriggerState();

        _saveState.SaveableID = SaveableID;
        _saveState.WasUsedAlready = BeenUsedAlready;

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

    public void OnLoad(PickupState _loadState)
    {
        throw new NotImplementedException();
    }

    public void OnLoad(MultipleStateObjectManagerState _loadState)
    {
        throw new NotImplementedException();
    }

    public void OnLoad(ActionTriggerState _loadState)
    {
        BeenUsedAlready = _loadState.WasUsedAlready;
    }

    public void OnDestroy()
    {
    }
}