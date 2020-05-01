using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LowPolyHnS
{
    public class GameManager : MonoBehaviour
    {
        [Serializable]
        public struct CharactersInventory
        {
            public List<int> PlayerInvList;
            public int[] PlayerSpellList;
            public bool PlayerDisguised;

            public void Clear()
            {
                PlayerInvList.Clear();
                PlayerSpellList = new int[0];
                PlayerDisguised = false;
            }
        }

        [Serializable]
        public struct CheatFlags
        {
            public bool GodMode;
        }

        [Serializable]
        public class PlayerSettings
        {
            public bool UseCharacterBasedMovement = false;
        }

        [HideInInspector] public static GameManager Instance;

        public CharactersInventory SavedCharactersInventory;
        public bool IsInCutscene;
        public SaveFile CurrentSave;
        public CheatFlags CheatOptions;
        public PlayerSettings Settings = new PlayerSettings();

        [Header("References to other scripts")]
        public ScreenFadeManager ScreenFadeScript = null;

        private Queue<CutsceneController> scheduledCutscenes = new Queue<CutsceneController>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                GetComponent<SceneLoader>().AssignAsInstance();
                GetComponent<MultipleStateObjectManager>().AssignAsInstance();
                PrepareSaveFile();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void PrepareSaveFile()
        {
            CurrentSave = SaveLoadSystem.GetSavedFile("save.sav");

            if (CurrentSave == null)
            {
                SaveGame();
            }
            else
            {
                Settings = CurrentSave.Settings;
            }
        }

        public void OnCutsceneStart()
        {
            IsInCutscene = true;
            Time.timeScale = 0f;
        }

        public void OnCutsceneEnd(float timeBeforeEndOfFade = 0f, bool isDeathCutscene = false)
        {
            IsInCutscene = false;

            if (scheduledCutscenes.Count > 0 && isDeathCutscene == false)
            {
                StartCoroutine(WaitAndDequeue(timeBeforeEndOfFade));
            }
            else if (isDeathCutscene == false)
            {
                UIController.Instance.HandleCutscene(false);
                Time.timeScale = 1f;
            }
        }

        private IEnumerator WaitAndDequeue(float waitTime)
        {
            yield return new WaitForSecondsRealtime(waitTime);

            if (scheduledCutscenes.Count > 0)
            {
                scheduledCutscenes.Dequeue().PlayCutscene();
            }
        }

        public void ScheduleCutscene(CutsceneController cutsceneToQueue)
        {
            if (IsInCutscene)
            {
                scheduledCutscenes.Enqueue(cutsceneToQueue);
            }
        }

        public void SaveGame(bool createNew = false)
        {
            if (createNew || CurrentSave == null)
            {
                CurrentSave = new SaveFile();
            }

            CurrentSave.Settings = Settings;

            SaveLoadSystem.SaveGame(CurrentSave, "save.sav");
        }

        public void ToggleGodMode()
        {
            CheatOptions.GodMode = !CheatOptions.GodMode;
        }
    }
}