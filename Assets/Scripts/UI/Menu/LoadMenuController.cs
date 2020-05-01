// Author: dpienkowska

using System.Collections.Generic;
using LowPolyHnS;
using UnityEngine;
using UnityEngine.UI;

public class LoadMenuController : SubMenuController
{
    // Code copied from FE_LoadScreen

    [Header("LoadMenu Properties")] [SerializeField]
    private Button[] saveSlots = new Button[10];

    private MainMenuController mainMenuController;

    private void Start()
    {
        mainMenuController = GetComponentInParent<MainMenuController>();
    }

    public override void Show()
    {
        base.Show();
        prepareSlots();
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
        if (SceneLoader.Instance)
        {
            SceneLoader.Instance.LoadFromSave(GameManager.Instance.CurrentSave.Saves[_selectedIndex]);
        }

        ExitCompletely();
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