using LowPolyHnS;
using UnityEngine.SceneManagement;

public class PauseMenuController : SubMenuController
{
    public void ReturnToMainMenu()
    {
        if(SceneLoader.Instance == null)
        {
            return;
        }

        ExitCompletely();
        SceneLoader.Instance.LoadLevel(-1, "Mainmenu", LoadSceneMode.Single, true);
    }
}
