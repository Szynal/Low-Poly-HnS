using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FearEffect.Puzzle
{
    public class FE_InteractionPuzzle : MonoBehaviour
    {
        public FE_PuzzleStarter StarterScript;
        protected bool allowInput = true;

        protected virtual void Update()
        {
            if(allowInput == true)
            {
                if (FE_CrossInput.MenuUpDown() != 0f)
                {
                    handleUpDownInput(FE_CrossInput.MenuUpDown() > 0f ? 1f : -1f);
                }
                else if(FE_CrossInput.MenuLeftRight() != 0f)
                {
                    handleLeftRightInput(FE_CrossInput.MenuLeftRight() > 0f ? 1f : -1f);
                }
                else if (FE_CrossInput.UseItem() == true)
                {
                    onAcceptInput();
                }
                else if (FE_CrossInput.MenuCancel() == true)
                {
                    onCancelInput();
                }
            }
        }

        protected virtual void handleUpDownInput(float _value)
        {

        }

        protected virtual void handleLeftRightInput(float _value)
        {

        }

        protected virtual void onAcceptInput()
        {

        }

        protected virtual void onCancelInput()
        {
            OnCancelInteraction();
        }

        public virtual void StartPuzzle(FE_PuzzleStarter _startingScript)
        {
            StarterScript = _startingScript;
            gameObject.SetActive(true);
        }

        public virtual void OnPuzzleFinished(bool _result)
        {
            StarterScript.HandleLeavingPuzzle(true, _result);
            gameObject.SetActive(false);
        }

        public virtual void OnCancelInteraction()
        {
            StarterScript.HandleLeavingPuzzle(false, false);
            gameObject.SetActive(false);
        }
    }
}
