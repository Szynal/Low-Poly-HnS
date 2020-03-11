using System.Collections;
using System.Collections.Generic;
using LowPolyHnS;
using UnityEngine;
using UnityEngine.UI;

public class FE_FinaleChooser : MonoBehaviour
{
    public enum EFinaleType
    {
        Hana,
        Glas,
        Both
    };

    public static FE_FinaleChooser Instance;

    [SerializeField] List<FE_ActionContainer> hanaStateChanges = new List<FE_ActionContainer>();
    [SerializeField] List<FE_ActionContainer> glasStateChanges = new List<FE_ActionContainer>();
    [SerializeField] List<FE_ActionContainer> bothStateChanges = new List<FE_ActionContainer>();
    [SerializeField] GameObject canvasObject = null;

    private void Start()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    public void StartChoosingFinale()
    {
        StartCoroutine(waitForCutsceneEnd());
    }

    private IEnumerator waitForCutsceneEnd()
    {
        while(GameManager.Instance.IsInCutscene == true)
        {
            yield return null;
        }

        canvasObject.SetActive(true);
        GetComponentInChildren<Selectable>().Select();
    }

    public void OnFinaleChosen(int _chosenType)
    {
        canvasObject.SetActive(false);

        if ((EFinaleType)_chosenType == EFinaleType.Hana)
        {
            Debug.Log("Chosen Hana");
            foreach (FE_ActionContainer _state in hanaStateChanges)
            {
                FE_StateChanger.HandleObjectChange(_state);
            }
        }
        else if ((EFinaleType)_chosenType == EFinaleType.Glas)
        {
            Debug.Log("Chosen Glas");
            foreach (FE_ActionContainer _state in glasStateChanges)
            {
                FE_StateChanger.HandleObjectChange(_state);
            }
        }
        else
        {
            Debug.Log("Chosen Both");
            foreach (FE_ActionContainer _state in bothStateChanges)
            {
                FE_StateChanger.HandleObjectChange(_state);
            }
        }
    }
}
