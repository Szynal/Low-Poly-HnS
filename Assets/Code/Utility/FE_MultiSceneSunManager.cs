using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FE_MultiSceneSunManager : MonoBehaviour
{
    public static FE_MultiSceneSunManager Instance;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
