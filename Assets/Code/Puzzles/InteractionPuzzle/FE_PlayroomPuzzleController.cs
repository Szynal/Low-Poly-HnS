using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FearEffect.Puzzle
{
    public class FE_PlayroomPuzzleController : FE_InteractionPuzzle
    {
        [SerializeField] GameObject objectToRotate = null;
        [SerializeField] float rotationSpeed = 72f;

        private int position = 1;
        public int stageID = 11;

        [Header("Changes between stages")]
        [SerializeField] List<FE_ActionContainer> afterFirstStageChange = new List<FE_ActionContainer>();
        [SerializeField] List<FE_ActionContainer> afterSecondStageChange = new List<FE_ActionContainer>();
        [SerializeField] List<FE_ActionContainer> afterThirdStageChange = new List<FE_ActionContainer>();

        protected override void handleLeftRightInput(float _value)
        {
            base.handleLeftRightInput(_value);

            StartCoroutine(rotateTheCircle(-_value));
        }

        protected override void onAcceptInput()
        {
            base.onAcceptInput();

            checkPosition();
        }

        protected override void onCancelInput()
        {
            positionCheckFailure();
        }

        private IEnumerator rotateTheCircle(float _dir)
        {
            allowInput = false;
            float _dirNormalized = (float)Mathf.RoundToInt(_dir); //We do that so we get either -1f or 1f, nothing in between
            float _rotatedAmount = 0f;

            position += (int)-_dir;

            if (position > 5)
            {
                position = 1;
            }
            else if (position < 1)
            {
                position = 5;
            }

            while (_rotatedAmount < 72f)
            {
                float _rot = rotationSpeed * _dirNormalized * Time.deltaTime;

                if (_rotatedAmount + Mathf.Abs(_rot) > 72f)
                {
                    if (_rot > 0f)
                    {
                        _rot = 72f - _rotatedAmount;
                    }
                    else
                    {
                        _rot = _rotatedAmount - 72f;
                    }
                }

                objectToRotate.transform.Rotate(0f, 0f, _rot);
                _rotatedAmount += Mathf.Abs(_rot);

                yield return null;
            }

            StartCoroutine(restoreInputAvailability(0.3f));
        }

        private IEnumerator restoreInputAvailability(float _time)
        {
            yield return new WaitForSeconds(_time);

            allowInput = true;
        }

        private void checkPosition()
        {
            allowInput = false;
            StartCoroutine(restoreInputAvailability(0.2f));

            switch (position)
            {
                case 5:
                    positionCheckFailure();
                    break;

                case 1:
                    if (stageID == 12 || stageID == 21 || stageID == 41 || stageID == 42)
                    {
                        positionCheckSuccess();
                    }
                    else
                    {
                        positionCheckFailure();
                    }
                    break;

                case 2:
                    if (stageID == 11 || stageID == 43)
                    {
                        positionCheckSuccess();
                    }
                    else
                    {
                        positionCheckFailure();
                    }
                    break;

                case 3:
                    if (stageID == 22 || stageID == 31 || stageID == 44)
                    {
                        positionCheckSuccess();
                    }
                    else
                    {
                        positionCheckFailure();
                    }
                    break;

                case 4:
                    if (stageID == 32)
                    {
                        positionCheckSuccess();
                    }
                    else
                    {
                        positionCheckFailure();
                    }
                    break;
            }
        }

        private void positionCheckSuccess()
        {
            Debug.Log("Correct!");

            switch (stageID)
            {
                case 11:
                    stageID = 12;
                    break;

                case 12:
                    stageID = 21;
                    resetPuzzleBetweenStages(afterFirstStageChange);
                    break;

                case 21:
                    stageID = 22;
                    break;

                case 22:
                    stageID = 31;
                    resetPuzzleBetweenStages(afterSecondStageChange);
                    break;

                case 31:
                    stageID = 32;
                    break;

                case 32:
                    stageID = 41;
                    resetPuzzleBetweenStages(afterThirdStageChange);
                    break;

                case 41:
                    stageID = 42;
                    break;

                case 42:
                    stageID = 43;
                    break;

                case 43:
                    stageID = 44;
                    break;

                case 44:
                    stageID = 51;
                    OnPuzzleFinished(true);
                    break;

                default:
                    Debug.Log("The puzzle is done already");
                    break;
            }
        }

        private void positionCheckFailure()
        {
            if(stageID == 12)
            {
                stageID = 11;
            }
            else if(stageID == 22)
            {
                stageID = 21;
            }
            else if(stageID == 32)
            {
                stageID = 31;
            }
            else if(stageID > 41 && stageID < 50)
            {
                stageID = 41;
            }

            Debug.Log("Reseting to last pos...");
            resetPuzzleBetweenStages(null);
        }

        private void resetPuzzleBetweenStages(List<FE_ActionContainer> _changesToMake)
        {
            if (_changesToMake != null)
            {
                foreach (FE_ActionContainer _container in _changesToMake)
                {
                    FE_StateChanger.HandleObjectChange(_container);
                }
            }

            StopAllCoroutines();

            OnCancelInteraction();
            allowInput = true;
            position = 1;
            objectToRotate.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }
}
