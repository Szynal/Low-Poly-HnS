using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FE_DangerZone : MonoBehaviour
{


    private FE_PlayerHealth player = null;

    private void OnTriggerEnter(Collider _other)
    {
        if(_other.tag == "Player")
        {
            player = _other.GetComponent<FE_PlayerHealth>();
            if (player != null)
            {
                player.AddDanger(this);
            }
        }
    }

    private void OnTriggerExit(Collider _other)
    {
        if (_other.tag == "Player")
        {
            if (player != null)
            {
                player.RemoveDanger(this);
                player = null;
            }

        }
    }

    private void OnDestroy()
    {
        if(player != null)
        {
            player.RemoveDanger(this);
        }
    }
}
