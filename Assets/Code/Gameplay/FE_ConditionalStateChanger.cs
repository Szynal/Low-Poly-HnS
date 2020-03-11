using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FE_ConditionalStateChanger : MonoBehaviour
{
    public enum EStateChangeCondition
    {
        EnemiesKilled,
        InteractablesUsed
    };

    [Header("Conditions for state change")]
    [SerializeField] EStateChangeCondition conditionToPass = 0;
    [SerializeField] List<FE_InteractableObject> interactablesToUse = new List<FE_InteractableObject>();

    [Header("State changer properties")]
    public EStateMessage AfterUseState = EStateMessage.FirstButtonUsed;
    public List<FE_ActionContainer> AfterUseObjectChanges = new List<FE_ActionContainer>();

    private List<FE_InteractableObject> alreadyUsedInteractables = new List<FE_InteractableObject>();
    private FE_MultipleStateObject parentScript;
    private bool hasParent = true;
    private int conditionsMet = 0;

    private void Awake()
    {
        parentScript = GetComponentInParent<FE_MultipleStateObject>();

        if (parentScript == null)
        {
            Debug.LogWarning("State changer " + name + " couldn't find FE_MultipleStateObject script in parent!");
            hasParent = false;
        }
    }

    private void Start()
    {
        if (conditionToPass == EStateChangeCondition.EnemiesKilled)
        {
            
        }
        else if(conditionToPass == EStateChangeCondition.InteractablesUsed)
        {
            foreach(FE_InteractableObject _interactable in interactablesToUse)
            {
                _interactable.UseDelegate += onInteractableUsed;
            }
        }
    }

    private void onEnemyKilled(Transform _deathTrans, bool _stealthKill)
    {
        conditionsMet += 1;
    }

    private void onInteractableUsed(FE_InteractableObject _justUsedInteractable)
    {
        alreadyUsedInteractables.Add(_justUsedInteractable);

        foreach(FE_InteractableObject _interactable in interactablesToUse)
        {
            if(alreadyUsedInteractables.Contains(_interactable) == false)
            {
                return;
            }
        }

        onConditionsMet();
    }

    private void onConditionsMet()
    {
        if(hasParent == true)
        {
            parentScript.ChangeState(AfterUseState, true);
        }

        foreach (FE_ActionContainer _state in AfterUseObjectChanges)
        {
            FE_StateChanger.HandleObjectChange(_state);
        }
    }
}
