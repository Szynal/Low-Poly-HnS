using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FearEffect.Puzzle
{
    public class FE_TrainPuzzleController : FE_InteractionPuzzle
    {
        [SerializeField] private EventSystem puzzleSystem = null;
        private EventSystem oldSys;

        private Selectable currentSelectedButton;
        private int buttonId;

        [Header("Selectable Puzzle Button")]
        [SerializeField] private List<Selectable> puzzleButtons = null;

        [SerializeField] private List<Text> puzzleButtonsText = null;

        [Header("Lamp Activator")]
        [SerializeField] private Image Activator01 = null;

        [SerializeField] private Image Activator02 = null;
        [SerializeField] private Image Activator03 = null;

        private char[] alphabet =
        {
            '0', '1', '2', '3', '4',
            '5', '6', '7', '8', '9',
            'A', 'B', 'C', 'I', 'L',
            'N', 'T', 'Y', 'Z'
        };

        private int[] puzzle = new int[12];

        private int[] puzzleAnswer = new int[12]
        {
            4, 15, 2, 11, 4, 2, 2, 12, 8, 0, 0, 0
        };

        private void Start()
        {
            InitPuzzleStateRandomly();
            SetPuzzleButtonTest();

            currentSelectedButton = puzzleButtons[0];
            buttonId = 0;
            puzzleButtons[0].GetComponent<Image>().color = Color.grey;
            currentSelectedButton.Select();
        }

        private void OnEnable()
        {
            oldSys = EventSystem.current;
            EventSystem.current = puzzleSystem;
            CheckAnswer();
        }

        private void OnDisable()
        {
            if (oldSys != null)
            {
                EventSystem.current = oldSys;
            }
        }

        public void Submit()
        {
            UpdatePuzzleButtonState();
            SetPuzzleButtonTest();
            SetLampActivator();
            CheckAnswer();
        }

        public void CancelInteraction()
        {
            OnCancelInteraction();
        }

        public void OnSelect(BaseEventData _eventData)
        {
            currentSelectedButton = _eventData.selectedObject.GetComponent<Selectable>();
            SetLampActivator();
        }

        public void SetSelectedButton(int _id)
        {
            buttonId = _id;
        }

        private void InitPuzzleStateRandomly()
        {
            for (int i = 0; i < puzzle.Length; i++)
            {
                puzzle[i] = Random.Range(0, 18);
            }
        }

        private void UpdatePuzzleButtonState()
        {
            puzzle[buttonId] += 1;
            if (puzzle[buttonId] > 18)
            {
                puzzle[buttonId] = 0;
            }
        }

        private void CheckAnswer()
        {
            for (int i = 0; i < puzzle.Length; i++)
            {
                if (puzzle[i] != puzzleAnswer[i])
                {
                    break;
                }

                if (i == puzzle.Length - 1)
                {
                    OnPuzzleFinished(true);
                }
            }
        }

        private void SetLampActivator()
        {
            for (int i = 0; i < 4; i++)
            {
                if (puzzle[i] != puzzleAnswer[i])
                {
                    Activator01.color = Color.red;
                    break;
                }

                if (i == 3)
                {
                    Activator01.color = Color.green;
                }
            }

            for (int i = 4; i < 8; i++)
            {
                if (puzzle[i] != puzzleAnswer[i])
                {
                    Activator02.color = Color.red;
                    break;
                }

                if (i == 7)
                {
                    Activator02.color = Color.green;
                }
            }

            for (int i = 8; i < 12; i++)
            {
                if (puzzle[i] != puzzleAnswer[i])
                {
                    Activator03.color = Color.red;
                    break;
                }

                if (i == 11)
                {
                    Activator03.color = Color.green;
                }
            }
        }

        private void SetPuzzleButtonTest()
        {
            if (!puzzleButtons.Any())
            {
                return;
            }

            for (int i = 0; i < puzzle.Length; i++)
            {
                puzzleButtonsText[i].text = alphabet[puzzle[i]].ToString();
            }
        }

        public void OnSelect(Selectable _newSelection)
        {
            AlignSelectionToTarget(_newSelection);
        }

        private void AlignSelectionToTarget(Selectable _target)
        {
            currentSelectedButton = _target;
            currentSelectedButton.GetComponent<Image>().color = Color.grey;
        }
    }
}