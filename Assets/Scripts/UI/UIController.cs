using LowPolyHnS;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController Instance;
    [SerializeField] private Image gameOverBackgroundImage = null;

    [Header("References to menus")] [SerializeField]
    private SubMenuController pauseMenu = null;

    [SerializeField] private SubMenuController saveMenu = null;
    [SerializeField] private SubMenuController loadMenu = null;
    [SerializeField] private SubMenuController gameOverMenu = null;

    private bool gameOverShown;

    #region Basic Unity Methods

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    #endregion

    public void HandleCutscene(bool isPlaying)
    {
    }

    #region Menus

    public void ShowSaveScreen()
    {
        if (saveMenu == null || GameManager.Instance.IsInCutscene)
        {
            return;
        }

        saveMenu.Show();
    }

    public void ShowLoadScreen()
    {
        if (loadMenu == null || GameManager.Instance.IsInCutscene)
        {
            return;
        }

        loadMenu.Show();
    }

    public void TogglePauseMenu()
    {
        if (GameManager.Instance.IsInCutscene || gameOverShown)
        {
            return;
        }

        if (pauseMenu.IsVisible())
        {
            pauseMenu.ExitCompletely();
        }
        else if (saveMenu.IsVisible())
        {
            saveMenu.ExitCompletely();
        }
        else if (loadMenu.IsVisible())
        {
            loadMenu.ExitCompletely();
        }
        else
        {
            pauseMenu.Show();
        }
    }

    public void ShowGameOverScreen()
    {
        gameOverBackgroundImage.enabled = true;
        gameOverShown = true;
        gameOverMenu.Show();
    }

    #endregion
}