using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FearEffect.Puzzle
{
    public class FE_LadderPuzzleController : FE_InteractionPuzzle
    {
        [SerializeField] private EventSystem puzzleSystem = null;
        private EventSystem oldSys;
        [Space(5f)] [SerializeField] private GameObject selectionObject = null;
        [SerializeField] private Image firstFuse = null;
        [SerializeField] private Image secondFuse = null;
        [Space(5f)]
        [SerializeField] private Toggle leftPowerSwitch = null;
        [SerializeField] private List<Selectable> leftNodes = new List<Selectable>();
        [SerializeField] private Toggle middlePowerSwitch = null;
        [SerializeField] private List<Selectable> middleNodes = new List<Selectable>();
        [SerializeField] private Toggle rightPowerSwitch = null;
        [SerializeField] private List<Selectable> rightNodes = new List<Selectable>();

        [Header("Starting position for fuses")]
        [SerializeField] private Selectable rightPowerNode = null;
        [SerializeField] private Selectable alarmNode = null;

        [Header("Positions needed for the first step")]
        [SerializeField] private Selectable middlePowerNode = null;
        [SerializeField] private Selectable lock3Node = null;

        [Header("Positions needed for the second step")]
        [SerializeField] private Selectable leftPowerNode = null;
        [SerializeField] private Selectable ladderNode = null;

        private Selectable firstFusePosition;
        private Selectable secondFusePosition;

        private Selectable oldFirstFusePos;
        private Selectable oldSecondFusePos;
        private Image carriedFuse;

        private bool clearedFirstStep;

        [SerializeField] protected EStateMessage messageToSendOnTrigger = EStateMessage.FirstTriggerEntered;

        [Header("State changer properties")]
        public List<FE_ActionContainer> AfterUseObjectChanges = new List<FE_ActionContainer>();


        private void Awake()
        {
            firstFusePosition = alarmNode;
            firstFuse.transform.position = alarmNode.transform.position;

            secondFusePosition = rightPowerNode;
            secondFuse.transform.position = rightPowerNode.transform.position;
        }

        private void OnEnable()
        {
            oldSys = EventSystem.current;

            EventSystem.current = puzzleSystem;
            puzzleSystem.firstSelectedGameObject.GetComponent<Selectable>().Select();
            OnSelect(puzzleSystem.firstSelectedGameObject.GetComponent<Selectable>());
        }

        private void OnDisable()
        {
            if (oldSys != null)
            {
                EventSystem.current = oldSys;
            }
        }

        public override void OnCancelInteraction()
        {
            if (oldFirstFusePos != null)
            {
                Debug.Log("Trying to on click target: " + oldFirstFusePos);
                firstFuse.transform.position = oldFirstFusePos.transform.position;
                OnClick(oldFirstFusePos);
            }
            else if (oldSecondFusePos != null)
            {
                Debug.Log("Trying to on click target: " + oldSecondFusePos);
                secondFuse.transform.position = oldSecondFusePos.transform.position;
                OnClick(oldSecondFusePos);
            }

            base.OnCancelInteraction();
        }

        public void OnSelect(Selectable _newSelection)
        {
            alignSelectionToTarget(_newSelection);
        }

        public void OnClick(Selectable _clickedSelectable)
        {
            if (_clickedSelectable is Toggle == false)
            {
                if (checkForDamage(_clickedSelectable)) //If we use a node that's still powered on, we will take damage
                {
                    Debug.Log("Ouch! Damage taken.");
                    //------------------------------------TODO: actual dealing of damage
                    OnCancelInteraction();
                }
                else
                {
                    if (_clickedSelectable == firstFusePosition && carriedFuse == null)
                    {
                        carriedFuse = firstFuse;
                        oldFirstFusePos = firstFusePosition;
                        firstFusePosition = null;
                        leftPowerSwitch.interactable = false;
                        middlePowerSwitch.interactable = false;
                        rightPowerSwitch.interactable = false;
                    }
                    else if (_clickedSelectable == secondFusePosition && carriedFuse == null)
                    {
                        carriedFuse = secondFuse;
                        oldSecondFusePos = secondFusePosition;
                        secondFusePosition = null;
                        leftPowerSwitch.interactable = false;
                        middlePowerSwitch.interactable = false;
                        rightPowerSwitch.interactable = false;
                    }
                    else if (carriedFuse != null && _clickedSelectable != firstFusePosition && _clickedSelectable != secondFusePosition)
                    {
                        if (carriedFuse == firstFuse)
                        {
                            firstFusePosition = _clickedSelectable;
                            oldFirstFusePos = null;
                        }
                        else
                        {
                            secondFusePosition = _clickedSelectable;
                            oldSecondFusePos = null;
                        }

                        leftPowerSwitch.interactable = true;
                        middlePowerSwitch.interactable = true;
                        rightPowerSwitch.interactable = true;
                        carriedFuse = null;
                    }
                }
            }

            checkForConfiguration(); //In theory it is impossible for the configuration to be done after placing or removing a fuse, should we place this call in a different place?
        }

        private void alignSelectionToTarget(Selectable _target)
        {
            selectionObject.transform.position = _target.transform.position;

            Rect _targetRect = _target.GetComponent<RectTransform>().rect;

            selectionObject.GetComponent<RectTransform>().sizeDelta = new Vector2(_targetRect.width + 20f, _targetRect.height + 20f);

            if (carriedFuse != null)
            {
                carriedFuse.transform.position = _target.transform.position;
            }
        }

        private bool checkForDamage(Selectable _clickedSelectable)
        {
            if (carriedFuse == null && _clickedSelectable != firstFusePosition &&  _clickedSelectable != secondFusePosition) //We don't take damage if we touch an empty space, since we can't really interact with it anyways
            {
                return false;
            }

            if (leftPowerSwitch.isOn && leftNodes.Contains(_clickedSelectable))
            {
                return true;
            }

            if (middlePowerSwitch.isOn && middleNodes.Contains(_clickedSelectable))
            {
                return true;
            }

            if (rightPowerSwitch.isOn && rightNodes.Contains(_clickedSelectable))
            {
                return true;
            }

            return false;
        }

        private void checkForConfiguration()
        {
            //If we didn't clear the first step, we don't even need to check if the second one is done
            if (clearedFirstStep == false)
            {
                if ((firstFusePosition == middlePowerNode && secondFusePosition == lock3Node || firstFusePosition == lock3Node && secondFusePosition == middlePowerNode) && middlePowerSwitch.isOn)
                {
                    Debug.Log("Cleared the first step.");
                    clearedFirstStep = true;

                    foreach (FE_ActionContainer _state in AfterUseObjectChanges)
                    {
                        FE_StateChanger.HandleObjectChange(_state);
                    }
                }
            }
            else
            {
                if ((firstFusePosition == leftPowerNode && secondFusePosition == ladderNode || firstFusePosition == ladderNode && secondFusePosition == leftPowerNode) && leftPowerSwitch.isOn)
                {
                    Debug.Log("Cleared the second step.");
                    //------------------------------------TODO: shit that happens when the puzzle is completed
                    OnPuzzleFinished(true);
                }
            }
        }
    }
}