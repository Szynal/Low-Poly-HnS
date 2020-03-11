using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FE_ActionTrigger : MonoBehaviour, ISaveable
{
    [Header("Step trigger properties")]
    [SerializeField] protected bool oneShotOnly = true;
    [SerializeField] protected bool hasParent = false;
    [SerializeField] protected bool triggeredByAnEnemy = false;
    protected FE_MultipleStateObject parentScript;

    [SerializeField] protected EStateMessage messageToSendOnTrigger = EStateMessage.FirstTriggerEntered;

    [Header("State changer properties")]
    public List<FE_ActionContainer> AfterUseObjectChanges = new List<FE_ActionContainer>();

    [Header("Saveable properties")]
    public int SaveableID = -1; 

    public bool BeenUsedAlready = false;

    protected void Awake()
    {
        if(hasParent == true)
        {
            parentScript = GetComponentInParent<FE_MultipleStateObject>();

            if(parentScript == null)
            {
                Debug.LogError("StepInTrigger " + gameObject.name + " has 'hasParent' set to true, but it couldn't find the parent script!");
                hasParent = false;
            }
        }
    }

    protected virtual void OnTriggerEnter(Collider _other)
    {
        if(canBeTriggered() == true)
        {
            if(triggeredByAnEnemy == false && _other.tag != "Player")
            {
                return;
            }
            else if(triggeredByAnEnemy == true && _other.tag == "Player")
            {
                return;
            }

            BeenUsedAlready = true;

            foreach (FE_ActionContainer _state in AfterUseObjectChanges)
            {
                FE_StateChanger.HandleObjectChange(_state);
            }

            if (hasParent == true)
            {
                parentScript.ChangeState(messageToSendOnTrigger, true);
            }
        }
    }

    protected bool canBeTriggered()
    {
        if(oneShotOnly == true && BeenUsedAlready == true)
        {
            return false;
        }

        return true;
    }

    public void OnSave(SceneSave _saveTo)
    {
        FE_ActionTriggerState _saveState = new FE_ActionTriggerState();

        _saveState.SaveableID = SaveableID;
        _saveState.WasUsedAlready = BeenUsedAlready;

        _saveTo.RecordSaveableState(_saveState);
    }

    public void OnLoad(FE_PlayerSaveState _loadState)
    {
        throw new System.NotImplementedException();
    }

    public void OnLoad(FE_EnemySaveState _loadState)
    {
        throw new System.NotImplementedException();
    }

    public void OnLoad(FE_PickupState _loadState)
    {
        throw new System.NotImplementedException();
    }

    public void OnLoad(MultipleStateObjectManagerState _loadState)
    {
        throw new System.NotImplementedException();
    }

    public void OnLoad(FE_ActionTriggerState _loadState)
    {
        BeenUsedAlready = _loadState.WasUsedAlready;
    }

    public void OnDestroy()
    {
    }
}
