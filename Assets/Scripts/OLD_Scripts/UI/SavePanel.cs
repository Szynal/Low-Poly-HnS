using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LowPolyHnS.UI
{
    public class SavePanel : MonoBehaviour
    {
        [SerializeField] private Button[] saveSlots = new Button[10];

        private void Update()
        {
            if (Input.GetButtonDown("UICancel"))
            {
                OnCancel();
            }
        }

        public void HandleShowing(bool shouldShow)
        {
            if (shouldShow)
            {
                prepareSlots();
                saveSlots[0].Select();
            }

            gameObject.SetActive(shouldShow);
        }

        public void OnCancel()
        {
            HandleShowing(false);
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

            Save usedSave = new Save();
            usedSave.SaveScenes();
            GameManager.Instance.CurrentSave.Saves[selectedIndex] = usedSave;

            SaveLoadSystem.SaveGame(GameManager.Instance.CurrentSave, "save.sav");

            OnCancel();
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
                _slotTitle.text = _savedGames[i].MainSceneID != -1
                    ? $"SAVED SLOT {i + 1} : {_savedGames[i].Date}"
                    : "EMPTY SLOT";
            }
        }
    }
}