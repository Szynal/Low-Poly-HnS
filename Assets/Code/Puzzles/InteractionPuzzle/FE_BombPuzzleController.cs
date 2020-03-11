using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FearEffect.Puzzle
{

    public class FE_BombPuzzleController : FE_InteractionPuzzle
    {
        [SerializeField] List<Button> buttonsList = new List<Button>();
        int[] selectedCables;
        bool bHorisontalNavButtonWasPressed = false, bVerticalNavButtonWasPressed = false;
        int[] completeStatus = { 0, 0, 0 };
        bool failed = false;
        bool checkInProcessing = false;

        //Button selectedButton;
        int selectedIndex = -1;
        // Start is called before the first frame update
        void Start()
        {
            selectedCables = new int[buttonsList.Count];
            for (int i = 0; i < selectedCables.Length; i++)
                selectedCables[i] = -1;
        }

        // Update is called once per frame
        new void Update()
        {
            if (!failed)
            {
                if(FE_CrossInput.MenuCancel())
                {
                    OnCancelInteraction();
                }

                if (Mathf.Abs(FE_CrossInput.MoveX()) > 0 && !bHorisontalNavButtonWasPressed)
                {
                    bHorisontalNavButtonWasPressed = true;
                    HorisontalSelectButton(FE_CrossInput.MoveX());

                }
                else if (FE_CrossInput.MoveX() == 0)
                {
                    bHorisontalNavButtonWasPressed = false;
                }
                if (Input.GetButtonDown("UIAccept") == true)
                {
                    if (selectedIndex >= 0 && selectedIndex < buttonsList.Count)
                    {
                        if (selectedCables[selectedIndex] >= 0)
                        {
                            buttonsList[selectedIndex].GetComponent<FE_ButtonControlledCabels>().CutCable(selectedCables[selectedIndex]);
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

        }

        void CheckPuzzleStatus()
        {
            checkInProcessing = true;
            bool completed = true;
            for (int i = 0; i < buttonsList.Count; i++)
            {
                completeStatus[i] = buttonsList[i].GetComponent<FE_ButtonControlledCabels>().checkCables();
                //Debug.Log(i +" == "+completeStatus[i]);
            }
            foreach (int i in completeStatus)
            {
                if (i == -1)
                {
                    failed = true;
                    Debug.Log("Failed.");
                    completed = false;

                    OnPuzzleFinished(false);
                    break;
                }
                if (i == 0)
                {
                    GetComponent<Text>().text = "Processing";
                    completed = false;
                }
            }
            if (completed)
            {
                failed = true;
                Debug.Log("Success!");

                OnPuzzleFinished(true);
            }
            checkInProcessing = false;
        }
        void VerticalSelectButton(float currentAxis)
        {
            if (selectedIndex < 0 || selectedIndex >= buttonsList.Count)
                return;

            int newIndex;
            int currentCableIndex = selectedCables[selectedIndex];
            if (currentAxis < 0)
            {
                newIndex = 1;
            }
            else
            {
                newIndex = -1;
            }
            newIndex = currentCableIndex + newIndex;
            if (newIndex > selectedCables.Length - 1)
            {
                HideLastCable();
                selectedCables[selectedIndex] = 0;
            }
            else if (newIndex < 0)
            {
                HideLastCable();
                selectedCables[selectedIndex] = selectedCables.Length - 1;
            }
            else
            {
                HideLastCable();
                selectedCables[selectedIndex] = newIndex;
            }
            buttonsList[selectedIndex].GetComponent<FE_ButtonControlledCabels>().ShowCable(selectedCables[selectedIndex]);
        }

        void HideLastCable()
        {
            if (selectedIndex >= 0)
            {
                if (selectedCables[selectedIndex] >= 0)
                    buttonsList[selectedIndex].GetComponent<FE_ButtonControlledCabels>().HideCable(selectedCables[selectedIndex]);
            }
        }

        void HorisontalSelectButton(float currentAxis)
        {
            int _newIndex;

            if (currentAxis < 0)
            {
                _newIndex = -1;
            }
            else
            {
                _newIndex = 1;
            }

            _newIndex = selectedIndex + _newIndex;

            if (selectedIndex < 0 || _newIndex > buttonsList.Count - 1)
            {
                HideButtonsCursor();
                selectedIndex = 0;
            }
            else if (_newIndex < 0)
            {
                HideButtonsCursor(0);
                selectedIndex = buttonsList.Count - 1;
            }
            else
            {
                HideButtonsCursor();
                selectedIndex = _newIndex;
            }

            buttonsList[selectedIndex].GetComponent<FE_ButtonControlledCabels>().cursorImage.SetActive(true);
        }


        void HideButtonsCursor()
        {
            if (selectedIndex >= 0)
            {
                buttonsList[selectedIndex].GetComponent<FE_ButtonControlledCabels>().cursorImage.SetActive(false);
                buttonsList[selectedIndex].GetComponent<FE_ButtonControlledCabels>().HideCables();
            }
        }
        void HideButtonsCursor(int index)
        {
            if (index >= 0)
            {
                buttonsList[index].GetComponent<FE_ButtonControlledCabels>().cursorImage.SetActive(false);
                buttonsList[index].GetComponent<FE_ButtonControlledCabels>().HideCables();
            }
        }

    }
}

