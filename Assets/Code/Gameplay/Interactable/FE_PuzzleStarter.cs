using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FE_PuzzleStarter : FE_InteractableObject
{
    [Header("Properties for puzzle starting")]
    [SerializeField] FearEffect.Puzzle.FE_InteractionPuzzle puzzle = null;
    [SerializeField] FE_MultipleStateObject managerScript = null;

    [Header("State changer properties")]
    public List<FE_ActionContainer> AfterUseObjectChanges = new List<FE_ActionContainer>();

    protected override void Awake()
    {
        base.Awake();

        if (managerScript == null)
        {
            managerScript = GetComponentInParent<FE_MultipleStateObject>();
        }
    }

    protected override void onActivation(FE_PlayerInventoryInteraction _instigator)
    {
        base.onActivation(_instigator);

        //  SetCanInteract(false);

        FE_UIController.Instance.gameObject.SetActive(false);
        FE_PlayerInputController.Instance.ChangeInputMode(EInputMode.None);
        puzzle.StartPuzzle(this);
    }

    public void HandleLeavingPuzzle(bool _puzzleCompleted, bool _success)
    {
        // SetCanInteract(true);

        FE_UIController.Instance.gameObject.SetActive(true);
        FE_PlayerInputController.Instance.ChangeInputMode(EInputMode.Full);

        if (_puzzleCompleted == true)
        {
            if (_success == true)
            {
                managerScript.ChangeState(EStateMessage.PuzzleFinished, true);

                foreach (FE_ActionContainer _state in AfterUseObjectChanges)
                {
                    FE_StateChanger.HandleObjectChange(_state);
                }
            }
            else
            {
                OnPuzzleFailed();
            }
        }
    }

    private void OnPuzzleFailed()
    {

    }
}
