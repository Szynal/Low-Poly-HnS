using LowPolyHnS;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FE_UIController : MonoBehaviour
{
    public static FE_UIController Instance;

    [Header("Panels")] [SerializeField] private FE_InteractionPanel interactionPanel = null;
    [SerializeField] private FE_InventoryPanel inventoryPanel = null;
    [SerializeField] private FE_CombatStatusScreen combatInfo = null;
    [SerializeField] private FE_PickupPanel pickupPanel = null;

    [Header("Player UI references")] [SerializeField]
    private Image playerHealthImage = null;

    [SerializeField] private Text interactionText = null;
    [SerializeField] private Image gameOverBackgroundImage = null;

    [Header("Boss healthbar")] [SerializeField]
    private GameObject bossHealthBarObject = null;

    [SerializeField] private Image bossHealthFillImage = null;

    [Header("References to menus")] [SerializeField]
    private SubMenuController pauseMenu = null;

    [SerializeField] private SubMenuController saveMenu = null;
    [SerializeField] private SubMenuController loadMenu = null;
    [SerializeField] private SubMenuController gameOverMenu = null;
    [Header("Other")] [SerializeField] private GameObject controllerInputMap = null;
    [SerializeField] private GameObject keyboardInputMap = null;

    [SerializeField] public EventSystem MenuEventSystem = null;


    private FE_PlayerInventoryInteraction interactionController;
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

        if (inventoryPanel == null || combatInfo == null || interactionText == null || interactionPanel == null ||
            pickupPanel == null || gameOverBackgroundImage == null || playerHealthImage == null)
        {
        }


        playerHealthImage.enabled = false;
    }

    private void Start()
    {
        interactionController = FE_PlayerInventoryInteraction.Instance;
        if (interactionController == null)
        {
        }

        inputController = FE_PlayerInputController.Instance;
        if (inputController == null)
        {
        }
        else
        {
            inputController.OnInventoryOpened += OpenInventory;
        }
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

    public void ChangePanelVisibility(bool _visibilityState, MonoBehaviour _callerScript)
    {
        if (_callerScript == this || _callerScript == interactionController)
        {
            if (_visibilityState && interactionPanel.GetVisibility() == false)
            {
                interactionPanel.Appear();
            }
            else if (_visibilityState == false && interactionPanel.GetVisibility() &&
                     interactionController.HasInteraction() == false)
            {
                interactionPanel.Disappear();
            }

            handlePanelDisplay();
        }
    }

    private void handlePanelDisplay()
    {
        if (interactionController.HasInteraction())
        {
            if (dangerNearby == false)
            {
                ShowHealth(false, dangerNearby);
            }

            FE_InteractableObject _foundInteraction = interactionController.GetInteraction();

            if (_foundInteraction != null && (_foundInteraction is FE_Pickup || _foundInteraction is FE_AmmoPickup))
            {
                pickupPanel.gameObject.SetActive(true);
            }
            else
            {
                pickupPanel.gameObject.SetActive(false);
            }

            ChangeInteractionText();
        }
        else
        {
            pickupPanel.gameObject.SetActive(false);

            if (dangerNearby == false)
            {
                ShowHealth(false);
            }
        }
    }

    public void ChangeInteractionText()
    {
        FE_InteractableObject _foundInteraction = interactionController.GetInteraction();

        if (_foundInteraction != null)
        {
            if (_foundInteraction is FE_Pickup || _foundInteraction is FE_AmmoPickup)
            {
                pickupPanel.UpdateDisplay(_foundInteraction);
            }

            interactionText.color = _foundInteraction.GetStringColor();
            interactionText.text = _foundInteraction.GetInteractionString();
        }
    }

    public void ShowTargetingIndicator(bool _hasTarget, bool _critical = false)
    {
        combatInfo.ShowTargetingStatus(_hasTarget, _critical);
    }

    public void ShowHealth(bool _show, bool _isDanger = false)
    {
        if (_show || inventoryPanel.gameObject.activeSelf != true)
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
        interactionPanel.gameObject.SetActive(!_isPlaying);
        combatInfo.gameObject.SetActive(!_isPlaying);
        pickupPanel.gameObject.SetActive(!_isPlaying);

        inventoryPanel.gameObject.SetActive(false);

        if (_isPlaying == false && interactionController != null)
        {
            interactionController.RefreshInteractions();
        }
    }

    #region Inventory

    public void OpenInventory()
    {
        if (inventoryPanel.gameObject.activeSelf)
        {
            CloseInventory();
            return;
        }

        FE_PlayerInventoryInteraction.Instance.InputController.ChangeInputMode(EInputMode.InventoryOnly);
        Time.timeScale = 0f;

        combatInfo.gameObject.SetActive(false);
        ChangePanelVisibility(false, this);

        inventoryPanel.gameObject.SetActive(true);
    }

    public void CloseInventory()
    {
        Time.timeScale = 1f; //is it always 1?

        inventoryPanel.gameObject.SetActive(false);
        combatInfo.gameObject.SetActive(true);

        FE_PlayerInventoryInteraction.Instance.InputController.ChangeInputMode(EInputMode.Full);
    }

    #endregion

    #region BossHealth

    public void ChangeBossHealthVisibility(bool _newVal)
    {
        bossHealthBarObject.SetActive(_newVal);
    }

    public void UpdateBossHealthBar(float _healthMax, float _healthCurrent)
    {
        bossHealthFillImage.fillAmount = _healthCurrent / _healthMax;
    }

    #endregion

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
        if (inventoryPanel.gameObject.activeSelf)
        {
            CloseInventory();
        }

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