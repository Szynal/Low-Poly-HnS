using System;
using System.Collections.Generic;
using LowPolyHnS;
using UnityEngine;
using UnityEngine.AI;

public class FE_StateChanger : FE_InteractableObject
{
    [Header("State changer properties")] public EStateMessage AfterUseState = EStateMessage.FirstButtonUsed;
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

    protected override void onActivation()
    {
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
                    Health _objHealth = _objectChange.TargetObject.GetComponent<Health>();
                    if (_objHealth != null)
                    {
                        _objHealth.TakeDamage(_objHealth.HealthCurrent);
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
                        if (_objectChange.TargetObject.CompareTag("Player"))
                        {
                        }
                        else
                        {
                            _objectChange.TargetObject.transform.position = _objectChange.Pos;
                        }
                    }
                }

                break;

            case EActionType.ChangeMSOStateByName:
                MultipleStateObjectManager.Instance.ChangeStateByName(_objectChange.MSOName, _objectChange.NewMSOState);
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
                    _objectChange.CutsceneToStart.PlayCutscene();
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

            default:
                Debug.LogError(
                    "FE_StateChanger has encountered an unknown state in one of its' FE_NewObjectStateContainers!");
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
    public SceneTeleporter SceneTeleporterToUse;
    public MonoBehaviour MessageReciever;
    public EMessage MessageToSend;
    public CutsceneController CutsceneToStart;
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
    StartFinaleChooser
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