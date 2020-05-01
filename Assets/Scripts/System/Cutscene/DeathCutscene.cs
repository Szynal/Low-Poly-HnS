using LowPolyHnS;
using UnityEngine;

public class DeathCutscene : CutsceneController
{
    protected override void endCutscene()
    {
        GameManager.Instance.OnCutsceneEnd(0f, true);

        if (hasItsOwnScene)
        {
            SceneLoader.Instance.CloseCutscene(SceneName);
        }
        else
        {
            if (oneShotOnly)
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