using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FearEffect.Puzzle
{
    public class FE_MapPuzzleLocation : MonoBehaviour
    {
        [SerializeField] GameObject alreadyUsedVisualizationObject = null;
        public FE_MapPuzzleController.EItemSelected ItemToRespondTo = FE_MapPuzzleController.EItemSelected.None;
        public bool WasUsedAlready = false;

        public void VisualizeInteractibility(bool _isInteractable)
        {
            alreadyUsedVisualizationObject?.SetActive(!_isInteractable);
        }
    }
}
