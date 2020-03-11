using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FearEffect.Puzzle
{

    public class FE_SymbolsPuzzleController : FE_InteractionPuzzle
    {
        [SerializeField] List<Button> buttonsList = new List<Button>();
        [SerializeField] List<Sprite> Sprites = new List<Sprite>();
        [SerializeField] int[] DefaultButtonsSpritesValues = new int[0];
        [SerializeField] int[] ButtonsSpritesValuesForComplete = new int[0];

        [SerializeField] Color unselectedColor = Color.blue;
        [SerializeField] Color selectedColor = Color.red;

        int[] selectedImages;
        bool[] fixedImages;
        bool bHorisontalNavButtonWasPressed = false, bVerticalNavButtonWasPressed = false;
        int[] completeStatus = { 0, 0, 0, 0 };
        bool failed = false;
        bool checkInProcessing = false;

        //Button selectedButton;
        int selectedIndex = -1;

        void Start()
        {
            selectedImages = DefaultButtonsSpritesValues;
            fixedImages = new bool[selectedImages.Length];
            for (int i = 0; i < buttonsList.Count; i++)
            {
                buttonsList[i].GetComponent<Image>().sprite = Sprites[Random.Range(0, Sprites.Count - 1)];
                buttonsList[i].GetComponent<Image>().color = unselectedColor;
            }
        }

        new void Update()
        {
            if (failed != true)
            {
                if (Mathf.Abs(FE_CrossInput.MoveX()) > 0 && bHorisontalNavButtonWasPressed == false)
                {
                    bHorisontalNavButtonWasPressed = true;
                    HorisontalSelectButton(FE_CrossInput.MoveX());

                }
                else if (FE_CrossInput.MoveX() == 0)
                {
                    bHorisontalNavButtonWasPressed = false;
                }
                if (Input.GetButtonDown("UIAccept") != false)
                {
                    if (selectedIndex >= 0 && selectedIndex < buttonsList.Count)
                    {
                        if (selectedImages[selectedIndex] >= 0)
                        {
                            if (fixedImages[selectedIndex] == false)
                            {
                                fixedImages[selectedIndex] = true;
                                buttonsList[selectedIndex].GetComponent<Image>().color = selectedColor;
                            }
                            else
                            {
                                fixedImages[selectedIndex] = false;
                                buttonsList[selectedIndex].GetComponent<Image>().color = unselectedColor;
                            }
                        }
                    }
                }

                if (Mathf.Abs(FE_CrossInput.MoveY()) > 0 && !bVerticalNavButtonWasPressed)
                {
                    bVerticalNavButtonWasPressed = true;
                    VerticalSelectButton(FE_CrossInput.MoveY());

                }
                else if (FE_CrossInput.MoveY() == 0)
                {
                    bVerticalNavButtonWasPressed = false;
                }
                if (!checkInProcessing)
                    CheckPuzzleStatus();
            }

            if (Input.GetButtonDown("UICancel") != false)
            {
                resetPuzzle();
                OnCancelInteraction();
            }
        }

        void CheckPuzzleStatus()
        {
            checkInProcessing = true;
            bool _completed = true;
            for (int i = 0; i < selectedImages.Length; i++)
            {
                if (fixedImages[i] == false)
                    _completed = false;
                else
                {
                    if (selectedImages[i] == ButtonsSpritesValuesForComplete[i])
                        completeStatus[i] = 1;
                    else
                        completeStatus[i] = -1;
                }
                //Debug.Log(i +" == "+completeStatus[i]);
            }
            if (_completed)
            {
                foreach (int i in completeStatus)
                {
                    if (i == -1)
                    {
                        //failed = true;
                        _completed = false;

                        //  OnCancelInteraction();
                        break;
                    }
                }
                if (_completed)
                {
                    failed = true;
                    Debug.Log("Success!");

                    OnPuzzleFinished(true);
                }
            }
            checkInProcessing = false;
        }

        void VerticalSelectButton(float currentAxis)
        {
            if (selectedIndex < 0 || selectedIndex >= buttonsList.Count || fixedImages[selectedIndex])
                return;

            int newIndex;
            int currentCableIndex = selectedImages[selectedIndex];
            if (currentAxis < 0)
            {
                newIndex = 1;
            }
            else
            {
                newIndex = -1;
            }
            newIndex = currentCableIndex + newIndex;
            if (newIndex > Sprites.Count - 1)
            {
                selectedImages[selectedIndex] = 0;
            }
            else if (newIndex < 0)
            {
                selectedImages[selectedIndex] = Sprites.Count - 1;
            }
            else
            {
                selectedImages[selectedIndex] = newIndex;
            }
            buttonsList[selectedIndex].GetComponent<Image>().sprite = Sprites[selectedImages[selectedIndex]];
        }

        void HorisontalSelectButton(float currentAxis)
        {
            int newIndex, lastIndex = selectedIndex;

            if (currentAxis < 0)
            {
                newIndex = -1;
            }
            else
            {
                newIndex = 1;
            }

            newIndex = selectedIndex + newIndex;

            if (selectedIndex < 0 || newIndex > buttonsList.Count - 1)
            {
                //HideButtonsCursor();
                selectedIndex = 0;
            }
            else if (newIndex < 0)
            {
                //HideButtonsCursor(0);
                selectedIndex = buttonsList.Count - 1;
            }
            else
            {
                //HideButtonsCursor();
                selectedIndex = newIndex;
            }
            if (lastIndex >= 0)
                buttonsList[lastIndex].GetComponent<FE_ButtonControlledCabels>().cursorImage.SetActive(false);

            buttonsList[selectedIndex].GetComponent<FE_ButtonControlledCabels>().cursorImage.SetActive(true);
        }

        private void resetPuzzle()
        {
            if (selectedIndex >= 0)
            {
                buttonsList[selectedIndex].GetComponent<FE_ButtonControlledCabels>().cursorImage.SetActive(false);
            }

            selectedIndex = -1;
            bHorisontalNavButtonWasPressed = false;
            bVerticalNavButtonWasPressed = false;
            failed = false;

            for (int i = 0; i < buttonsList.Count; i++)
            {
                fixedImages[i] = false;
                buttonsList[i].GetComponent<Image>().color = unselectedColor;
            }
        }
    }
}

