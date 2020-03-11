using LowPolyHnS;
using UnityEngine;

public class FE_DeathCutscene : FE_CutsceneController
{
    protected override void endCutscene()
    {
        GameManager.Instance.OnCutsceneEnd(0f, true);

        if (hasItsOwnScene == true)
        {
            SceneLoader.Instance.CloseCutscene(SceneName);
        }
        else
        {
            if (oneShotOnly == true)
            {
                Destroy(gameObject);
            }
            else
            {
                skipCanvas.SetActive(false);
                foreach (GameObject _go in objectsToActivateWithoutDirector)
                {
                    _go.SetActive(false);
                }
            }
        }

        FE_UIController.Instance.ShowGameOverScreen();
    }
}
