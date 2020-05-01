// Author: dpienkowska

using System.Collections.Generic;
using LowPolyHnS;
using UnityEngine;
using UnityEngine.UI;

public class SaveMenuController : SubMenuController
{
    // Code copied from FE_SaveScreen, very similar structure to LoadMenuController

    [Header("SaveMenu Properties")] [SerializeField]
    private Button[] saveSlots = new Button[10];

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

        //Now to either overwrite the save, or create a new one
        Save _usedSave = new Save();
        _usedSave.SaveScenes();
        GameManager.Instance.CurrentSave.Saves[_selectedIndex] = _usedSave;

        SaveLoadSystem.SaveGame(GameManager.Instance.CurrentSave, "save.sav");

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
            }
            else
            {
                _slotTitle.text = "EMPTY SLOT";
            }
        }
    }
}