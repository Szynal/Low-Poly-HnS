﻿using System;
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
            public bool UseGravitatingCamera;
            public bool GodMode;
        }

        [Serializable]
        public class PlayerSettings
        {
            public bool UseCharacterBasedMovement = false;
        }

        [HideInInspector] public static GameManager Instance;

        public FE_ItemDatabase ItemDatabase;
        public CharactersInventory SavedCharactersInventory;
        public bool IsInCutscene;
        public FE_SaveFile CurrentSave;
        public CheatFlags CheatOptions;
        public PlayerSettings Settings = new PlayerSettings();

        [Header("References to other scripts")]
        public ScreenFadeManager ScreenFadeScript = null;

        private Queue<FE_CutsceneController> scheduledCutscenes = new Queue<FE_CutsceneController>();

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
            CurrentSave = FE_SaveLoadSystem.GetSavedFile("save.sav");

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
                FE_UIController.Instance.HandleCutscene(false);
                Time.timeScale = 1f;
            }
        }

        private IEnumerator WaitAndDequeue(float waitTime)
        {
            yield return new WaitForSecondsRealtime(waitTime);

            if (scheduledCutscenes.Count > 0)
            {
                scheduledCutscenes.Dequeue().PlayCutscene(FE_PlayerInventoryInteraction.Instance);
            }
        }

        public void ScheduleCutscene(FE_CutsceneController cutsceneToQueue)
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
                CurrentSave = new FE_SaveFile();
            }

            CurrentSave.Settings = Settings;

            FE_SaveLoadSystem.SaveGame(CurrentSave, "save.sav");
        }

        public void ToggleGodMode()
        {
            CheatOptions.GodMode = !CheatOptions.GodMode;
        }
    }
}