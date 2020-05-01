using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FE_StateChangeOnEnable : MonoBehaviour
{
    [SerializeField] private float delayBeforeActivating = 0f;

    [Header("State changer properties")]
    public List<FE_ActionContainer> OnEnableStateChanges = new List<FE_ActionContainer>();

    private void OnEnable()
    {
        if (delayBeforeActivating <= 0f)
        {
            foreach (FE_ActionContainer _container in OnEnableStateChanges)
            {
                FE_StateChanger.HandleObjectChange(_container);
            }
        }
        else
        {
            StartCoroutine(waitBeforeActivation());
        }
    }

    private IEnumerator waitBeforeActivation()
    {
        yield return new WaitForSeconds(delayBeforeActivating);

        foreach (FE_ActionContainer _container in OnEnableStateChanges)
        {
            FE_StateChanger.HandleObjectChange(_container);
        }
    }
}