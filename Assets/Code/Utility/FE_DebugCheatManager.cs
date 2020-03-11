using UnityEngine;
using UnityEngine.SceneManagement;

public class FE_DebugCheatManager : MonoBehaviour
{
    void Start()
    {
        if(Debug.isDebugBuild != true)
        {
            Destroy(this);
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            for(int i = 0; i < FE_PlayerInventoryInteraction.Instance.Ammunition.Length - 1; i++)
            {
                FE_PlayerInventoryInteraction.Instance.Ammunition[i] = 999;
            }
        }
    }
}
