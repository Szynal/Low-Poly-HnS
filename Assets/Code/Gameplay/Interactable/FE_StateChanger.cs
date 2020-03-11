using System;
using System.Collections.Generic;
using LowPolyHnS;
using UnityEngine;
using UnityEngine.AI;

public class FE_StateChanger : FE_InteractableObject
{
    [Header("State changer properties")]
    public EStateMessage AfterUseState = EStateMessage.FirstButtonUsed;
    public List<FE_ActionContainer> AfterUseObjectChanges = new List<FE_ActionContainer>();

    private FE_MultipleStateObject parentScript;
    private bool hasParent = true;

    protected override void Awake()
    {
        base.Awake();

        parentScript = GetComponentInParent<FE_MultipleStateObject>();

        if (parentScript == null)
        {
            hasParent = false;
        }
    }

    protected override void onActivation(FE_PlayerInventoryInteraction _instigator)
    {
        base.onActivation(_instigator);

        SetCanInteract(false);

        if (hasParent)
        {
            parentScript.ChangeState(AfterUseState, true);
        }

        foreach (FE_ActionContainer _state in AfterUseObjectChanges)
        {
            HandleObjectChange(_state);
        }

        onInteractionEnd();
    }

    public static void HandleObjectChange(FE_ActionContainer _objectChange)
    {
        switch (_objectChange.StateType)
        {
            case EActionType.KillObject:
                if (_objectChange.TargetObject != null)
                {
                    FE_Health _objHealth = _objectChange.TargetObject.GetComponent<FE_Health>();
                    if (_objHealth != null)
                    {
                        _objHealth.TakeDamage(_objHealth.HealthCurrent, _objHealth.transform, true, true);
                    }
                    else
                    {
                        Destroy(_objectChange.TargetObject);
                    }
                }

                break;

            case EActionType.MoveObjectToPos:
                if (_objectChange.TargetObject != null && _objectChange.Pos != null)
                {
                    if (_objectChange.TargetObject.tag == "Player")
                    {
                    }

                    NavMeshAgent _foundNavAgent = _objectChange.TargetObject.GetComponent<NavMeshAgent>();
                    if (_foundNavAgent != null && _foundNavAgent.enabled)
                    {
                        _foundNavAgent.Warp(_objectChange.Pos);
                    }
                    else
                    {
                        if(_objectChange.TargetObject.CompareTag("Player"))
                        {
                        }
                        else
                        {
                            _objectChange.TargetObject.transform.position = _objectChange.Pos;
                        }
                    }
                }

                break;

            case EActionType.HealPlayer:
                FE_PlayerHealth _player = GameObject.FindWithTag("Player").GetComponent<FE_PlayerHealth>();
                if (_player != null)
                {
                    _player.HealToFull();
                }
                break;

            case EActionType.ChangeMSOStateByName:
                MultipleStateObjectManager.Instance.ChangeStateByName(_objectChange.MSOName, _objectChange.NewMSOState);
                break;

            case EActionType.GivePlayerItem:
                FE_PlayerInventoryInteraction.Instance.AddItem(Instantiate(GameManager.Instance.ItemDatabase.GetItemByID(_objectChange.ItemIDToGive)));
                break;

            case EActionType.UseSceneTeleporter:
                if (_objectChange.SceneTeleporterToUse != null)
                {
                    _objectChange.SceneTeleporterToUse.HandleTeleport();
                }
                break;

            case EActionType.SendMessage:
                IMessageReciever _msgReciever = _objectChange.MessageReciever as IMessageReciever;
                if (_msgReciever != null)
                {
                    _msgReciever.OnMessageRecieved(_objectChange.MessageToSend);
                }
                break;

            case EActionType.StartInsceneCutscene:
                if (_objectChange.CutsceneToStart != null)
                {
                    _objectChange.CutsceneToStart.PlayCutscene(FE_PlayerInventoryInteraction.Instance);
                }
                break;

            case EActionType.RotateObjectToAngle:
                if (_objectChange.TargetObject != null)
                {
                    _objectChange.TargetObject.transform.rotation = _objectChange.Rotation;

                    if (_objectChange.TargetObject.tag == "Player")
                    {
                    }
                }
                break;

            case EActionType.RemoveItemFromPlayer:
                FE_PlayerInventoryInteraction.Instance.RemoveItem(FE_PlayerInventoryInteraction.Instance.GetInventoryItemByID(_objectChange.ItemIDToGive));
                break;

            case EActionType.StartFinaleChooser:
                if(FE_FinaleChooser.Instance != null)
                {
                    FE_FinaleChooser.Instance.StartChoosingFinale();
                }
                else
                {
                    Debug.LogError("StartFinaleChooser was called, but there is no finale chooser in this scene!");                  
                }
                break;

            default:
                Debug.LogError("FE_StateChanger has encountered an unknown state in one of its' FE_NewObjectStateContainers!");
                break;
        }
    }
}

[Serializable]
public class FE_ActionContainer
{
    public EActionType StateType;
    public GameObject TargetObject;
    public Vector3 Pos;
    public string MSOName;
    public int NewMSOState;
    public int ItemIDToGive;
    public FE_SceneTeleporter SceneTeleporterToUse;
    public MonoBehaviour MessageReciever;
    public EMessage MessageToSend;
    public FE_CutsceneController CutsceneToStart;
    public Quaternion Rotation;
}

public enum EActionType
{
    KillObject,
    MoveObjectToPos,
    HealPlayer,
    ChangeMSOStateByName,
    GivePlayerItem,
    StartBossFight,
    UseSceneTeleporter,
    SendMessage,
    StartInsceneCutscene,
    RotateObjectToAngle,
    RemoveItemFromPlayer,
    StartFinaleChooser,
}

public enum EMessage
{
    ChangeState,
    ChangeDisguise
}

public interface IMessageReciever
{
    void OnMessageRecieved(EMessage _incomingMsg);
}