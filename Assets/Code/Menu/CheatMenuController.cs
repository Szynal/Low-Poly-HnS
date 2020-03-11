using UnityEngine.SceneManagement;

namespace LowPolyHnS
{
    public class CheatMenuController : SubMenuController
    {
        public void LoadLevel(int index)
        {
            switch (index)
            {
                case 1:
                    SceneLoader.Instance.LoadLevel(SceneManager.GetSceneByName("Lvl_01_starter").buildIndex,
                        "Lvl_01_starter",
                        LoadSceneMode.Single, true);
                    break;
                case 2:
                    SceneLoader.Instance.LoadLevel(SceneManager.GetSceneByName("Lvl_02_starter").buildIndex,
                        "Lvl_02_starter",
                        LoadSceneMode.Single, true);
                    break;
                case 3:
                    SceneLoader.Instance.LoadLevel(SceneManager.GetSceneByName("Lvl_03_starter").buildIndex,
                        "Lvl_03_starter",
                        LoadSceneMode.Single, true);
                    break;
                case 4:
                    SceneLoader.Instance.LoadLevel(SceneManager.GetSceneByName("Lvl_04_starter").buildIndex,
                        "Lvl_04_starter",
                        LoadSceneMode.Single, true);
                    break;
            }
        }
    }
}