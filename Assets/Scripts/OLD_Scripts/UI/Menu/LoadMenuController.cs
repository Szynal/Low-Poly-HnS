using System.Collections.Generic;
using LowPolyHnS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadMenuController : SubMenuController
{
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
        PrepareSlots();
    }

    public void OnSlotSelected(Button clickedButton)
    {
        int selectedIndex = 0;
        while (selectedIndex < 10)
        {
            if (clickedButton == saveSlots[selectedIndex])
            {
                break;
            }

            selectedIndex++;
        }

        if (SceneLoader.Instance)
        {
            SceneLoader.Instance.LoadFromSave(GameManager.Instance.CurrentSave.Saves[selectedIndex]);
        }

        ExitCompletely();
    }

    private void PrepareSlots()
    {
        List<Save> savedGames = GameManager.Instance.CurrentSave.Saves;

        for (int i = 0; i < 10; i++)
        {
            if (saveSlots[i] == null)
            {
                continue;
            }

            TextMeshProUGUI slotTitle = saveSlots[i].GetComponentInChildren<TextMeshProUGUI>(true);

            if (savedGames[i].MainSceneID != -1)
            {
                slotTitle.SetText($"Save {i + 1} : {savedGames[i].Date}");
                saveSlots[i].interactable = true;
            }
            else
            {
                slotTitle.SetText("Empty slot");
                saveSlots[i].interactable = false;
            }
        }
    }
}