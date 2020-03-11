using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FearEffect.Puzzle
{
    public class FE_MapPuzzleController : FE_InteractionPuzzle
    {
        public enum EItemSelected
        {
            None,
            Scroll,
            Eye,
            LeftTree,
            RightTree
        };

        [SerializeField] Image selectorImg = null;
        [SerializeField] Image markerImg = null;
        [Space(10f)]
        [SerializeField] List<Selectable> inventorySelectables = null;
        [SerializeField] List<Selectable> locationSelectables = null;

        private EItemSelected currentlySelectedItem = EItemSelected.None;
        private int correctlyPlacedAmount = 0;

        public override void StartPuzzle(FE_PuzzleStarter _startingScript)
        {
            base.StartPuzzle(_startingScript);

            changeSelectaleStates(locationSelectables, false);
            changeSelectaleStates(inventorySelectables, true);

            inventorySelectables[0].Select();
        }

        public void OnSelect(Selectable _newSelected)
        {
            repositionSelector(selectorImg, _newSelected);
        }

        public void OnClick(Selectable _clickedSelectable)
        {
            //Handle selecting inventory item
            if(currentlySelectedItem == EItemSelected.None)
            {
                if(inventorySelectables.Contains(_clickedSelectable) == true)
                {
                    currentlySelectedItem = (EItemSelected)(inventorySelectables.IndexOf(_clickedSelectable) + 1);

                    repositionSelector(markerImg, _clickedSelectable);

                    //Activate the locations
                    changeSelectaleStates(locationSelectables, true);

                    for(int i = 0; i < locationSelectables.Count; i++) //We find first interactable location and move selection there
                    {
                        if(locationSelectables[i].interactable == true)
                        {
                            locationSelectables[i].Select();
                            break;
                        }
                    }

                    //Deactivate inventory
                    changeSelectaleStates(inventorySelectables, false);
                }
                else
                {
                    Debug.LogError("Tried to select an inventory item, but selectable " + _clickedSelectable.gameObject + " isn't part of the inventory list");
                }
            }
            else //If we're clicking on a location, with an item already selected
            {
                FE_MapPuzzleLocation _locationScript = _clickedSelectable.GetComponent<FE_MapPuzzleLocation>();
                if(_locationScript == null)
                {
                    Debug.Log("Selectable " + _clickedSelectable.gameObject + " has no MP_MapPuzzleLocation!");
                    _clickedSelectable.enabled = false;
                    resetPuzzle();
                    return;
                }

                if(_locationScript.ItemToRespondTo == currentlySelectedItem)
                {
                    onCorrectPosition(_clickedSelectable);
                }
                else
                {
                    Debug.Log("Wrong place for this item.");
                    resetPuzzle();
                }
            }
        }

        private void onCorrectPosition(Selectable _selectedLocation)
        {
            correctlyPlacedAmount += 1;
            if(correctlyPlacedAmount >= 4)
            {
                OnPuzzleFinished(true);
                return;
            }

            FE_MapPuzzleLocation _locationScript = _selectedLocation.GetComponent<FE_MapPuzzleLocation>();
            _locationScript.WasUsedAlready = true;
            _locationScript.VisualizeInteractibility(false);

            FE_MapPuzzleLocation _inventoryScript = inventorySelectables[(int)(_locationScript.ItemToRespondTo) - 1].GetComponent<FE_MapPuzzleLocation>();
            _inventoryScript.WasUsedAlready = true;
            _inventoryScript.VisualizeInteractibility(false);

            changeSelectaleStates(inventorySelectables, true);

            //Activate the inventory
            changeSelectaleStates(inventorySelectables, true);

            for (int i = 0; i < inventorySelectables.Count; i++) //We find first interactable location and move selection there
            {
                if (inventorySelectables[i].interactable == true)
                {
                    inventorySelectables[i].Select();
                    break;
                }
            }

            //Deactivate locations
            changeSelectaleStates(locationSelectables, false);

            markerImg.enabled = false;
            currentlySelectedItem = EItemSelected.None;
        }

        private void changeSelectaleStates(List<Selectable> _listToChange, bool _shouldMakeThemActive)
        {
            for(int i = 0; i < _listToChange.Count; i++)
            {
                FE_MapPuzzleLocation _location = _listToChange[i].GetComponent<FE_MapPuzzleLocation>();
                if(_location == null)
                {
                    Debug.Log("Selectable " + _listToChange[i].gameObject + " doesn't have FE_MapPuzzleLocation on its' gameobject!");
                    _listToChange[i].enabled = false;
                    continue;
                }

                if(_shouldMakeThemActive == false)
                {
                    _listToChange[i].interactable = false;
                }
                else if(_location.WasUsedAlready == false)
                {
                    _listToChange[i].interactable = true;
                }
            }
        }

        private void repositionSelector(Image _selectorImg, Selectable _targetSelectable)
        {
            Rect _selectedRect = _targetSelectable.GetComponent<RectTransform>().rect;

            _selectorImg.enabled = true;
            _selectorImg.rectTransform.sizeDelta = new Vector2(_selectedRect.width + 20f, _selectedRect.height + 20f);
            _selectorImg.transform.position = _targetSelectable.transform.position;
        }

        private void resetPuzzle()
        {
            correctlyPlacedAmount = 0;
            currentlySelectedItem = EItemSelected.None;
            changeSelectaleStates(locationSelectables, false);
            changeSelectaleStates(inventorySelectables, true);

            foreach(FE_MapPuzzleLocation _locationScript in GetComponentsInChildren<FE_MapPuzzleLocation>())
            {
                _locationScript.VisualizeInteractibility(true);
                _locationScript.WasUsedAlready = false;
            }

            markerImg.enabled = false;
            selectorImg.enabled = false;

            OnCancelInteraction();
        }
    }
}
