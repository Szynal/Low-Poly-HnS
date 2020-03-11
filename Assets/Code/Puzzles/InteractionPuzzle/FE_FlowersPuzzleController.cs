using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FearEffect.Puzzle
{

    public class FE_FlowersPuzzleController : FE_InteractionPuzzle
    {
        [SerializeField] Text selectionText = null;
        [SerializeField] Image selectionImage = null;
        [Space(5f)]
        [SerializeField] Transform leftVaseTransform = null;
        [SerializeField] Transform rightVaseTransform = null;
        [SerializeField] Transform leftFlowerTransform = null;
        [SerializeField] Transform rightFlowerTransform = null;
        [Space(5f)]
        [SerializeField] Image whiteVaseImage = null;
        [SerializeField] Image blackVaseImage = null;
        [Space(5f)]
        [SerializeField] Image irisImage = null;
        [SerializeField] Image sunflowerImage = null;
        [SerializeField] Image roseImage = null;
        [SerializeField] Image tulipImage = null;
        [SerializeField] Image lilyImage = null;
        [Space(5f)]
        [SerializeField] string whiteVaseString = "A Pristine White Vase";
        [SerializeField] string blackVaseString = "A Cracked Black Vase";
        [SerializeField] string irisString = "A Beautiful Iris";
        [SerializeField] string sunflowerString = "A fresh Sunflower";
        [SerializeField] string roseString = "A Delicate Rose";
        [SerializeField] string tulipString = "A plain Tulip";
        [SerializeField] string lilyString = "A wilted Calla Lily";
        [Space(5f)]
        [SerializeField] List<Image> flowerList = new List<Image>();

        private Image leftFlower = null;
        private Image rightFlower = null;

        private int stageID = 1;

        private void Awake()
        {
            whiteVaseImage.gameObject.SetActive(true);
            blackVaseImage.gameObject.SetActive(true);

            whiteVaseImage.transform.position = rightVaseTransform.position;
            blackVaseImage.transform.position = leftVaseTransform.position;

            selectionImage.transform.position = leftVaseTransform.transform.position;
            selectionText.text = blackVaseString;
        }

        protected override void handleUpDownInput(float _value)
        {
            base.handleUpDownInput(_value);

            allowInput = false;

            switch (stageID)
            {
                case 1:
                    switchVases();
                    break;

                case 2:
                    changeFlower(leftFlowerTransform.position, _value);
                    break;

                case 3:
                    changeFlower(rightFlowerTransform.position, _value);
                    break;
            }

            StartCoroutine(restoreInputAvailability(0.2f));
        }

        protected override void onAcceptInput()
        {
            base.onAcceptInput();

            allowInput = false;
            StartCoroutine(restoreInputAvailability(0.2f));
            changeStage();
        }

        protected override void onCancelInput()
        {
            base.onCancelInput();

            resetPuzzle();
        }

        private IEnumerator restoreInputAvailability(float _time)
        {
            yield return new WaitForSeconds(_time);

            allowInput = true;
        }

        private void switchVases()
        {
            if (blackVaseImage.transform.position == leftVaseTransform.position)
            {
                blackVaseImage.transform.position = rightVaseTransform.position;
                whiteVaseImage.transform.position = leftVaseTransform.position;

                selectionText.text = whiteVaseString;
            }
            else
            {
                blackVaseImage.transform.position = leftVaseTransform.position;
                whiteVaseImage.transform.position = rightVaseTransform.position;

                selectionText.text = blackVaseString;
            }
        }

        private void changeStage()
        {
            switch (stageID)
            {
                case 1:
                    selectionImage.transform.position = leftFlowerTransform.position;
                    leftFlower = irisImage;
                    irisImage.gameObject.SetActive(true);
                    irisImage.transform.position = leftFlowerTransform.position;
                    rightFlower = roseImage;
                    roseImage.gameObject.SetActive(true);
                    roseImage.transform.position = rightFlowerTransform.position;
                    selectionText.text = irisString;
                    stageID = 2;
                    break;
                case 2:
                    selectionImage.transform.position = rightFlowerTransform.position;
                    selectionText.text = getStringByFlower(rightFlower);
                    stageID = 3;
                    break;
                case 3:
                    if(checkAnswer() == true)
                    {
                        StopAllCoroutines();
                        OnPuzzleFinished(true);
                    }
                    else
                    {
                        resetPuzzle();
                    }
                   // OnPuzzleFinished(checkAnswer());
                    break;
            }
        }

        private bool checkAnswer()
        {
            if (whiteVaseImage.transform.position != leftVaseTransform.position)
            {
                Debug.Log("Failed. The vases were wrong.");
                return false;
            }
            if (leftFlower != sunflowerImage)
            {
                Debug.Log("Failed. Left flower was wrong: " + leftFlower.name);
                return false;
            }
            if (rightFlower != lilyImage)
            {
                Debug.Log("Failed. Right flower was wrong: " + rightFlower.name);
                return false;
            }

            Debug.Log("Done correctly!");
            return true;
        }

        private void changeFlower(Vector3 _position, float _dir)
        {
            int _newIndex = -1;
            bool _isOnLeftSide = true;

            if (_position == leftFlowerTransform.position)
            {
                _newIndex = getFlowerIndex(leftFlower);
                _isOnLeftSide = true;
            }
            else
            {
                _newIndex = getFlowerIndex(rightFlower);
                _isOnLeftSide = false;
            }

            if (_newIndex == -1)
            {
                Debug.LogError("There has been an error in the flower list.");
                return;
            }

            int _startIndex = _newIndex;

            while (flowerList[_newIndex] == leftFlower || flowerList[_newIndex] == rightFlower)
            {
                _newIndex += (int)_dir;

                if (_newIndex >= flowerList.Count)
                {
                    _newIndex = 0;
                }
                else if (_newIndex < 0)
                {
                    _newIndex = flowerList.Count - 1;
                }

                if (_newIndex == _startIndex)
                {
                    Debug.LogError("No available flowers found. Returning to avoid infinite loop.");
                    return;
                }
            }

            flowerList[_startIndex].gameObject.SetActive(false);
            flowerList[_newIndex].gameObject.SetActive(true);

            if (_isOnLeftSide == true)
            {
                flowerList[_newIndex].transform.position = leftFlowerTransform.position;
                leftFlower = flowerList[_newIndex];
            }
            else
            {
                flowerList[_newIndex].transform.position = rightFlowerTransform.position;
                rightFlower = flowerList[_newIndex];
            }

            selectionText.text = getStringByFlower(flowerList[_newIndex]);
        }

        private int getFlowerIndex(Image _input)
        {
            for (int i = 0; i < flowerList.Count; i++)
            {
                if (flowerList[i] == _input)
                {
                    return i;
                }
            }

            return -1;
        }

        private string getStringByFlower(Image _flower)
        {
            if (_flower == irisImage)
            {
                return irisString;
            }
            else if (_flower == sunflowerImage)
            {
                return sunflowerString;
            }
            else if (_flower == roseImage)
            {
                return roseString;
            }
            else if (_flower == tulipImage)
            {
                return tulipString;
            }
            else
            {
                return lilyString;
            }
        }

        private void resetPuzzle()
        {
            whiteVaseImage.gameObject.SetActive(true);
            blackVaseImage.gameObject.SetActive(true);

            whiteVaseImage.transform.position = rightVaseTransform.position;
            blackVaseImage.transform.position = leftVaseTransform.position;

            selectionImage.transform.position = leftVaseTransform.transform.position;
            selectionText.text = blackVaseString;

            leftFlower = irisImage;
            rightFlower = roseImage;
            irisImage.gameObject.SetActive(false);
            sunflowerImage.gameObject.SetActive(false);
            roseImage.gameObject.SetActive(false);
            tulipImage.gameObject.SetActive(false);
            lilyImage.gameObject.SetActive(false);
            irisImage.gameObject.SetActive(false);

            stageID = 1;
        }
    }
}
