using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FE_SpecialDeathCutsceneStarter : MonoBehaviour
{
    public FE_CutsceneController CutsceneToStart = null;

    public void StartTargetCutscene()
    {
        if(CutsceneToStart == null)
        {
            return;
        }

        CutsceneToStart.PlayCutscene(FE_PlayerInventoryInteraction.Instance);
    }
}
