﻿using System.Collections.Generic;
using LowPolyHnS;
using UnityEngine;
using UnityEngine.UI;

public class FE_SaveScreen : MonoBehaviour
{
    [SerializeField] Button[] saveSlots = new Button[10];

    private void Update()
    {
        if(Input.GetButtonDown("UICancel") == true)
        {
            OnCancel();
        }
    }

    public void HandleShowing(bool _shouldShow)
    {
        if(_shouldShow == true)
        {
            prepareSlots();
            saveSlots[0].Select();
        }

        gameObject.SetActive(_shouldShow);
    }

    public void OnCancel()
    {
        HandleShowing(false);
        FE_UIController.Instance.OnExitedMenu();
    }

    public void OnSlotSelected(Button _clickedButton)
    {
        //We need to find out which slot we selected
        int _selectedIndex = 0;
        while(_selectedIndex < 10)
        {
            if(_clickedButton == saveSlots[_selectedIndex])
            {
                break;
            }

            _selectedIndex++;
        }

        //Now to either overwrite the save, or create a new one
        FE_Save _usedSave = new FE_Save();
        _usedSave.SaveScenes();
        GameManager.Instance.CurrentSave.Saves[_selectedIndex] = _usedSave;
        
        FE_SaveLoadSystem.SaveGame(GameManager.Instance.CurrentSave, "save.sav");

        OnCancel();
    }

    private void prepareSlots()
    {
        List<FE_Save> _savedGames = GameManager.Instance.CurrentSave.Saves;

        for(int i = 0; i < 10; i++)
        {
            if(saveSlots[i] == null)
            {
                continue;
            }

            Text _slotTitle = saveSlots[i].GetComponentInChildren<Text>(true);
            if(_savedGames[i].MainSceneID != -1)
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