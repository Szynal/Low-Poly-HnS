using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LowPolyHnS.UI
{
    public class LoadPanel : MonoBehaviour
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

        public void HandleShowing(bool shouldShow)
        {
            if (shouldShow)
            {
                PrepareSlots();
                exitButton.Select();
            }

            gameObject.SetActive(shouldShow);
        }

        public void OnExit()
        {
            HandleShowing(false);

            if (mainMenuController != null)
            {
                mainMenuController.OnMenuExited();
            }
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

            SceneLoader.Instance?.LoadFromSave(GameManager.Instance.CurrentSave.Saves[selectedIndex]);
            OnExit();
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

                Text slotTitle = saveSlots[i].GetComponentInChildren<Text>(true);
                if (savedGames[i].MainSceneID != -1)
                {
                    slotTitle.text = $"SAVED SLOT {i + 1} : {savedGames[i].Date}";
                    saveSlots[i].interactable = true;
                }
                else
                {
                    slotTitle.text = "EMPTY SLOT";
                    saveSlots[i].interactable = false;
                }
            }
        }
    }
}