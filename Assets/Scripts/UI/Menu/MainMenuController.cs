using LowPolyHnS.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LowPolyHnS
{
    public class MainMenuController : SubMenuController
    {
        [Header("MainMenu Properties")] [SerializeField]
        private LoadPanel loadPanelPanel = null;

        public void StartNewGame()
        {
            SceneLoader.Instance.LoadLevel(SceneManager.GetSceneByName("Lvl_01_starter").buildIndex, "Lvl_01_starter",
                LoadSceneMode.Single, true);
        }

        public void OpenLoadScreen()
        {
            if (loadPanelPanel == null)
            {
                return;
            }

            loadPanelPanel.HandleShowing(true);
        }

        public void OnMenuExited()
        {
            GetComponentInChildren<Button>().Select();
        }

        public void ExitGame()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
        }
    }
}