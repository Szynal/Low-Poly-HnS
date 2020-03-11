using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FearEffect.Puzzle
{

    public class FE_PuppetPuzzleController : FE_InteractionPuzzle
    {
        [SerializeField] EventSystem puzzleSystem = null;
        [SerializeField] GameObject selector = null;
        [Space(5f)]
        [SerializeField] Button resetButton = null;
        [SerializeField] Button firstPoseButton = null;
        [SerializeField] Button secondPoseButton = null;
        [SerializeField] Button thirdPoseButton = null;
        [SerializeField] Button fourthPoseButton = null;

        private EventSystem oldSys;

        private List<Button> pressedButtons = new List<Button>();
        private string puzzleAnswerString = "";

        private void Start()
        {
            puzzleSystem.firstSelectedGameObject.GetComponent<Selectable>().Select(); //We do it here because OnEnable starts before scaling the canvas, or something, and the selector is in the wrong place at first
        }

        private void OnEnable()
        {
            oldSys = EventSystem.current;

            EventSystem.current = puzzleSystem;
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
            resetPuzzle();
            base.OnCancelInteraction();
        }

        public void OnSelect(Selectable _newSelected)
        {
            selector.transform.position = _newSelected.transform.position;
        }

        public void OnDeselect(Selectable _objectDeselected)
        {
        }

        public void OnClick(Button _buttonPressed)
        {
            if (_buttonPressed == resetButton)
            {
                resetPuzzle();
            }
            else if (pressedButtons.Contains(_buttonPressed) == false)
            {
                if (_buttonPressed == firstPoseButton)
                {
                    puzzleAnswerString += "1";
                }
                else if (_buttonPressed == secondPoseButton)
                {
                    puzzleAnswerString += "2";
                }
                else if (_buttonPressed == thirdPoseButton)
                {
                    puzzleAnswerString += "3";
                }
                else if (_buttonPressed == fourthPoseButton)
                {
                    puzzleAnswerString += "4";
                }
                else
                {
                    puzzleAnswerString += "5";
                }

                _buttonPressed.image.color = _buttonPressed.colors.pressedColor;
                pressedButtons.Add(_buttonPressed);

                if (pressedButtons.Count >= 5)
                {
                    checkValidity();
                }
            }
        }

        private void resetPuzzle()
        {
            foreach (Button _b in pressedButtons)
            {
                _b.image.color = _b.colors.normalColor;
            }

            pressedButtons.Clear();
            puzzleAnswerString = "";
        }

        private void checkValidity()
        {
            if (puzzleAnswerString == "51342")
            {
                Debug.Log("Success!");
                //TODO: things to do when we succeed
                OnPuzzleFinished(true);
            }
            else
            {
                Debug.Log("Failure :(");
                //TODO: are we doing anything if we fail?
            }
        }
    }
}
