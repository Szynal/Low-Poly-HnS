using LowPolyHnS;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FE_UIController : MonoBehaviour
{
    public static FE_UIController Instance;

    [Header("Player UI references")] [SerializeField]
    private Image playerHealthImage = null;

    [SerializeField] private Image gameOverBackgroundImage = null;

    [Header("References to menus")] [SerializeField]
    private SubMenuController pauseMenu = null;

    [SerializeField] private SubMenuController saveMenu = null;
    [SerializeField] private SubMenuController loadMenu = null;
    [SerializeField] private SubMenuController gameOverMenu = null;
    [Header("Other")] [SerializeField] private GameObject controllerInputMap = null;
    [SerializeField] private GameObject keyboardInputMap = null;

    [SerializeField] public EventSystem MenuEventSystem = null;

    private FE_PlayerInputController inputController;
    private bool dangerNearby;
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

    private void Start()
    {
        inputController = FE_PlayerInputController.Instance;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Home))
        {
            Time.timeScale = controllerInputMap.activeInHierarchy ? 1f : 0f;
            controllerInputMap.SetActive(!controllerInputMap.activeInHierarchy);
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            Time.timeScale = keyboardInputMap.activeInHierarchy ? 1f : 0f;
            keyboardInputMap.SetActive(!keyboardInputMap.activeInHierarchy);
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    #endregion

    public void ShowHealth(bool _show, bool _isDanger = false)
    {
        if (_show)
        {
            playerHealthImage.enabled = _show;
        }

        dangerNearby = _isDanger;
    }

    public void SetHealthColor(int _newHealth)
    {
        if (_newHealth > 50)
        {
            playerHealthImage.color = Color.green;
        }
        else if (_newHealth <= 50 && _newHealth > 20)
        {
            playerHealthImage.color = new Color(250f, 155f, 0f);
        }
        else
        {
            playerHealthImage.color = Color.red;
        }
    }

    public void HandleCutscene(bool _isPlaying)
    {
    }


    #region Menus

    public void ShowSaveScreen()
    {
        if (saveMenu == null || GameManager.Instance.IsInCutscene)
        {
            return;
        }

        handleEnteringMenu();
        saveMenu.Show();
    }

    public void ShowLoadScreen()
    {
        if (loadMenu == null || GameManager.Instance.IsInCutscene)
        {
            return;
        }

        handleEnteringMenu();
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
            handleEnteringMenu();
            pauseMenu.Show();
        }
    }

    private void handleEnteringMenu()
    {
        FE_PlayerInputController.Instance.AllowInput = false;
        FE_PlayerInputController.Instance.AllowInventoryInput = false;
        Time.timeScale = 0f;
    }

    public void OnExitedMenu()
    {
        FE_PlayerInputController.Instance.AllowInput = true;
        FE_PlayerInputController.Instance.AllowInventoryInput = true;
        Time.timeScale = 1f;
    }

    public void ShowGameOverScreen()
    {
        gameOverBackgroundImage.enabled = true;
        gameOverShown = true;
        gameOverMenu.Show();
    }

    #endregion
}