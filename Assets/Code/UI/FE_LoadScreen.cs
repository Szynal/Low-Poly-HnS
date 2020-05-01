using System.Collections.Generic;
using LowPolyHnS;
using UnityEngine;
using UnityEngine.UI;

public class FE_LoadScreen : MonoBehaviour
{
    [SerializeField] private Button[] saveSlots = new Button[10];
    [SerializeField] private Button exitButton = null;

    private MainMenuController mainMenuController;

    private void Awake()
    {
        mainMenuController = GetComponentInParent<MainMenuController>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("UICancel"))
        {
            OnExit();
        }
    }

    public void HandleShowing(bool _shouldShow)
    {
        if (_shouldShow)
        {
            prepareSlots();
            exitButton.Select();
        }

        gameObject.SetActive(_shouldShow);
    }

    public void OnExit()
    {
        HandleShowing(false);

        if (mainMenuController != null
        ) //if we've found main menu controller, then we're in the main menu and we don't need to call UI controller
        {
            mainMenuController.OnMenuExited();
        }
        else
        {
            FE_UIController.Instance?.OnExitedMenu();
        }
    }

    public void OnSlotSelected(Button _clickedButton)
    {
        //We need to find out which slot we selected
        int _selectedIndex = 0;
        while (_selectedIndex < 10)
        {
            if (_clickedButton == saveSlots[_selectedIndex])
            {
                break;
            }

            _selectedIndex++;
        }

        //Now to load the game from there
        SceneLoader.Instance?.LoadFromSave(GameManager.Instance.CurrentSave.Saves[_selectedIndex]);
        OnExit();
    }

    private void prepareSlots()
    {
        List<Save> _savedGames = GameManager.Instance.CurrentSave.Saves;

        for (int i = 0; i < 10; i++)
        {
            if (saveSlots[i] == null)
            {
                continue;
            }

            Text _slotTitle = saveSlots[i].GetComponentInChildren<Text>(true);
            if (_savedGames[i].MainSceneID != -1)
            {
                _slotTitle.text = $"SAVED SLOT {i + 1} : {_savedGames[i].Date}";
                saveSlots[i].interactable = true;
            }
            else
            {
                _slotTitle.text = "EMPTY SLOT";
                saveSlots[i].interactable = false;
            }
        }
    }
}