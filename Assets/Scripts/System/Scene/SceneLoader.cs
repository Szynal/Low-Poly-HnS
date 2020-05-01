using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LowPolyHnS.Core;
using LowPolyHnS.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LowPolyHnS
{
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance;

        public List<ISaveable> ActiveSaveables = new List<ISaveable>();
        public List<Scene> ActiveScenes = new List<Scene>();

        [Header("Settings for loading")] [SerializeField]
        private float fadeToBlackTime = 1f;

        [SerializeField] private Camera loadingCamera = null;
        [SerializeField] private LoadingCircle loadingIndicator = null;

        private List<SceneSave> statesOfScenes = new List<SceneSave>();
        private SceneSave lastSceneState;
        private PlayerLoadParams playerLoadParams;
        private GameObject playerObject;
        private bool loadedFromFile;
        private string distinctGameplaySceneName = "";
        private ScreenFadeManager fadeManager;
        private bool isInLoading;

        private AsyncOperation loadOperation;

        public void AssignAsInstance()
        {
            Instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        public void LoadLevel(int sceneID, string name, LoadSceneMode loadMode, bool clearOldScenes = false,
            PlayerLoadParams playerParams = null, string distinctGameplayName = "")
        {
            if (isInLoading == false)
            {
                if (fadeManager == null)
                {
                    fadeManager = GameManager.Instance.ScreenFadeScript;
                }

                fadeManager.FadeToBlack(true, fadeToBlackTime,
                    () => StartLoading(sceneID, name, loadMode, clearOldScenes, playerParams,
                        distinctGameplayName));
                return;
            }

            distinctGameplaySceneName = distinctGameplayName;

            //If we've got custom player parameters we want our player to have when they load in the new scene, we need to remember it here
            if (playerParams != null)
            {
                playerLoadParams = playerParams;
            }

            //If we won't be able to go back to older scenes, then we should clear them from memory
            if (clearOldScenes)
            {
                statesOfScenes.Clear();
                ActiveScenes.Clear();
                GameManager.Instance.SavedCharactersInventory.Clear();
            }
            else
            {
                Scene activeGameplay = GetActiveGameplay();

                if (ActiveScenes.Contains(activeGameplay))
                {
                    //Before we load a new scene, we need to save our progress on current scene
                    if (GetStateFromID(activeGameplay.buildIndex) != null)
                    {
                        statesOfScenes.Remove(GetStateFromID(activeGameplay.buildIndex));
                    }

                    SceneSave state = new SceneSave();
                    state.SaveScene(activeGameplay);
                    lastSceneState = state;
                    statesOfScenes.Add(state);

                    //Save enemy scene
                    for (int i = 0; i < SceneManager.sceneCount; i++)
                    {
                        if (SceneManager.GetSceneAt(i).name.ToLower().Contains("enemies"))
                        {
                            Scene foundScene = SceneManager.GetSceneAt(i);
                            if (GetStateFromID(foundScene.buildIndex) != null)
                            {
                                statesOfScenes.Remove(GetStateFromID(foundScene.buildIndex));
                            }

                            SceneSave enemyState = new SceneSave();
                            enemyState.SaveScene(foundScene);
                            statesOfScenes.Add(enemyState);
                        }
                    }
                }
            }


            loadOperation = SceneManager.LoadSceneAsync(name, loadMode);
        }

        private void StartLoading(int sceneID, string name, LoadSceneMode loadMode, bool clearOldScenes = false,
            PlayerLoadParams playerParams = null, string distinctGameplayName = "")
        {
            Time.timeScale = 0f;
            isInLoading = true;
            loadingIndicator.gameObject.SetActive(true);

            if (Camera.main != null && Camera.main != loadingCamera)
            {
                Camera.main.enabled = false;
            }

            loadingCamera.enabled = true;

            LoadLevel(sceneID, name, loadMode, clearOldScenes, playerParams, distinctGameplayName);
        }

        private void FinishLoading()
        {
            isInLoading = false;
            loadingCamera.enabled = false;
            loadingIndicator.Hide();
        }

        #region Handling Cutscenes

        public void OpenCutscene(string sceneName)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }

        public void CloseCutscene(string sceneName)
        {
            Debug.Log("Unloading cutscene: " + sceneName);
            SceneManager.UnloadSceneAsync(sceneName);
        }

        #endregion

        #region General scene load/unload

        private void OnSceneLoaded(Scene loadedScene, LoadSceneMode loadMode)
        {
            if (Instance != this) return;

            SceneInfo sceneInfo = getSceneInfo(loadedScene);
            if (sceneInfo == null) return;

            switch (sceneInfo.SceneType)
            {
                case ESceneType.Geometry:
                {
                    //We need to load our gameplay
                    //If we have a distinct gameplay, we load it first
                    if (distinctGameplaySceneName != "")
                    {
                        LoadLevel(-1, distinctGameplaySceneName, LoadSceneMode.Additive);
                        distinctGameplaySceneName = "";
                    }

                    //Setting up basic stuff
                    if (loadMode != LoadSceneMode.Additive)
                    {
                        SceneManager.SetActiveScene(loadedScene);
                    }

                    break;
                }
                case ESceneType.Gameplay:
                case ESceneType.Development:
                {
                    //We need to make sure the scene's cameras are up
                    if (sceneInfo.SceneType == ESceneType.Gameplay)
                    {
                        if (sceneInfo.CameraSceneName != "")
                        {
                            if (ActiveScenes.Contains(SceneManager.GetSceneByName(sceneInfo.CameraSceneName)) ==
                                false)
                            {
                                LoadLevel(-1, sceneInfo.CameraSceneName, LoadSceneMode.Additive);
                                ActiveScenes.Add(SceneManager.GetSceneByName(sceneInfo.CameraSceneName));
                            }
                        }
                    }

                    //Setting player character
                    if (playerObject == null)
                    {
                        playerObject = GameObject.FindGameObjectWithTag("Player");
                    }
                    else
                    {
                        GameObject newPlayer = getPlayerInScene(loadedScene);
                        if (newPlayer != null && newPlayer != playerObject)
                        {
                            Destroy(playerObject);
                            playerObject = newPlayer;
                        }
                    }

                    //We want to always move player to the new scene, so we can properly save his state
                    if (playerObject != null)
                    {
                        SceneManager.MoveGameObjectToScene(playerObject, loadedScene);
                    }

                    //In case we have a save from which we want to load things
                    if (GetStateFromID(loadedScene.buildIndex) != null)
                    {
                        List<ISaveable> listOfSaveables = Resources.FindObjectsOfTypeAll<MonoBehaviour>()
                            .Where(_mb => _mb != null && _mb.gameObject.scene.isLoaded && _mb is ISaveable)
                            .Cast<ISaveable>().ToList();

                        OnLoadedFromSave(GetStateFromID(loadedScene.buildIndex), listOfSaveables);
                    }

                    //If we have custom player parameters for loading this scene, we use them now
                    if (playerLoadParams != null)
                    {
                        LoadCustomPlayerParams();
                    }

                    if (isInLoading && sceneInfo.HasEnemyScene == false && sceneInfo.CameraSceneName == "")
                    {
                        fadeManager.UnfadeFromBlack(0f, fadeToBlackTime, FinishLoading, true);
                    }

                    break;
                }
                case ESceneType.Cutscene:
                {
                    CutsceneController cutsceneController = FindObjectOfType<CutsceneController>();
                    if (cutsceneController != null)
                    {
                        cutsceneController.SceneName = loadedScene.name;
                        cutsceneController.PlayCutscene();
                    }

                    break;
                }
                case ESceneType.Enemies:
                {
                    //In case we have a save from which we want to load things
                    if (GetStateFromID(loadedScene.buildIndex) != null)
                    {
                        List<ISaveable> saveablesOnScene = Resources.FindObjectsOfTypeAll<MonoBehaviour>()
                            .Where(monoBehaviour =>
                                monoBehaviour.gameObject.scene.isLoaded && monoBehaviour is ISaveable).Cast<ISaveable>()
                            .ToList();
                        //We create a list of saveables, so we can use it in loading the state

                        OnLoadedFromSave(GetStateFromID(loadedScene.buildIndex), saveablesOnScene, true);
                    }

                    if (isInLoading)
                    {
                        fadeManager.UnfadeFromBlack(0f, fadeToBlackTime, FinishLoading, true);
                    }

                    break;
                }
                case ESceneType.Camera when isInLoading:
                    fadeManager.UnfadeFromBlack(0f, fadeToBlackTime, FinishLoading, true);
                    break;
                case ESceneType.Menu:
                {
                    if (UIController.Instance != null)
                    {
                        Destroy(UIController.Instance.gameObject);
                    }

                    if (isInLoading)
                    {
                        fadeManager.UnfadeFromBlack(0f, fadeToBlackTime, FinishLoading, true);
                    }

                    break;
                }
                case ESceneType.Starter:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            ActiveScenes.Add(loadedScene);
        }

        private void OnSceneUnloaded(Scene unloadedScene)
        {
            if (ActiveScenes.Contains(unloadedScene))
            {
                ActiveScenes.Remove(unloadedScene);
            }
        }

        #endregion

        #region Loading From Save

        public void LoadFromSave(Save savedGame)
        {
            //Clear up stuff, if there is any
            GameManager.Instance.SavedCharactersInventory.Clear();
            ActiveScenes.Clear();
            statesOfScenes = new List<SceneSave>(savedGame.StatesOfScenes);
            lastSceneState = null;

            loadedFromFile = true;

            StartCoroutine(LoadFromSaveAsync(savedGame));
        }

        private IEnumerator LoadFromSaveAsync(Save savedGame)
        {
            loadOperation = SceneManager.LoadSceneAsync("LoadingScene", LoadSceneMode.Single);
            loadOperation.allowSceneActivation = true;

            do
            {
                yield return null;
            } while (loadOperation.isDone == false);

            loadOperation = null;

            LoadLevel(savedGame.MainSceneID, savedGame.MainSceneName, LoadSceneMode.Single);

            do
            {
                yield return null;
            } while (loadOperation == null || loadOperation.isDone == false);

            for (int i = 0; i < savedGame.OpenedScenes.Length; i++)
            {
                if (savedGame.OpenedScenes[i].Item1 != savedGame.MainSceneID)
                {
                    LoadLevel(savedGame.OpenedScenes[i].Item1, savedGame.OpenedScenes[i].Item2,
                        LoadSceneMode.Additive);

                    do
                    {
                        yield return null;
                    } while (loadOperation.isDone == false);
                }
            }
        }

        private void OnLoadedFromSave(SceneSave loadedFrom, List<ISaveable> listOfSaveables,
            bool enemyScene = false)
        {
            //First, we need to load MSO manager, so it can give values to MSOs. BUT only if we're loading the game from a file!
            if (loadedFromFile)
            {
                loadedFromFile = false;
            }

            foreach (ISaveable saveable in listOfSaveables)
            {
                MonoBehaviour monoBehaviour = (MonoBehaviour) saveable;

                if (monoBehaviour.gameObject.scene.buildIndex != loadedFrom.SceneID)
                {
                    continue;
                }

                switch (monoBehaviour)
                {
                    case PlayerLoadSaveController _:
                    {
                        if (playerLoadParams == null && loadedFrom.GetPlayerState() != null)
                        {
                            saveable.OnLoad(loadedFrom.GetPlayerState());
                        }

                        break;
                    }
                }
            }
        }

        private void LoadCustomPlayerParams()
        {
            ISaveable player = PlayerLoadSaveController.Instance;

            if (lastSceneState == null) return;

            PlayerSaveState playerState = new PlayerSaveState();
            if (playerLoadParams.OverwriteTransform)
            {
                playerState.Position_X = playerLoadParams.CustomLoadPos.x;
                playerState.Position_Y = playerLoadParams.CustomLoadPos.y;
                playerState.Position_Z = playerLoadParams.CustomLoadPos.z;
                playerState.Rotation_Y = playerLoadParams.CustomLoadRot.y;
            }
            else
            {
                Transform playerTrans = ((PlayerLoadSaveController) player).transform;
                playerState.Position_X = playerTrans.position.x;
                playerState.Position_Y = playerTrans.position.y;
                playerState.Position_Z = playerTrans.position.z;
                playerState.Rotation_Y = playerTrans.rotation.eulerAngles.y;
            }

            if (playerLoadParams.OverwriteInventory)
            {
                playerState.InventoryIDList_Player = lastSceneState.GetPlayerState().InventoryIDList_Player;

                playerState.AmmoTable_Player = lastSceneState.GetPlayerState().AmmoTable_Player;

                playerState.CurrentWeaponID_Player = lastSceneState.GetPlayerState().CurrentWeaponID_Player;
                playerState.CurrentWeaponAmmo_Player = lastSceneState.GetPlayerState().CurrentWeaponAmmo_Player;

                playerState.PlayerDisguised = lastSceneState.GetPlayerState().PlayerDisguised;
            }
            else
            {
                playerState.CurrentWeaponID_Player = -1;
            }

            if (playerLoadParams.OverwriteHealth)
            {
                playerState.Health = lastSceneState.GetPlayerState().Health;
            }

            playerLoadParams = null;
            player.OnLoad(playerState);
        }

        #endregion

        #region Helper Methods

        private SceneSave GetStateFromID(int id)
        {
            return statesOfScenes.FirstOrDefault(scene => scene.SceneID == id);
        }

        public List<SceneSave> GetSceneStates()
        {
            List<Scene> invalidScenes = new List<Scene>();
            List<SceneSave> returnList = new List<SceneSave>(statesOfScenes);

            foreach (Scene scene in ActiveScenes)
            {
                if (scene.IsValid())
                {
                    returnList.Remove(GetStateFromID(scene.buildIndex));

                    SceneSave state = new SceneSave();
                    state.SaveScene(scene);
                    returnList.Add(state);
                }
                else
                {
                    invalidScenes.Add(scene);
                }
            }

            foreach (Scene scene in invalidScenes)
            {
                ActiveScenes.Remove(scene);
            }

            return returnList;
        }

        private GameObject getPlayerInScene(Scene sceneToSearch)
        {
            return sceneToSearch.GetRootGameObjects().FirstOrDefault(o => o != null && o.tag == "Player");
        }

        private Scene GetActiveGameplay()
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (getSceneInfo(SceneManager.GetSceneAt(i)).SceneType == ESceneType.Gameplay)
                {
                    return SceneManager.GetSceneAt(i);
                }
            }

            return SceneManager.GetSceneByName(SceneManager.GetActiveScene().name + "_Gameplay");
        }

        private List<Scene> GetActiveGameplayScenes()
        {
            return (from scene in ActiveScenes
                let info = getSceneInfo(scene)
                where info != null && info.SceneType == ESceneType.Gameplay
                select scene).ToList();
        }

        private SceneInfo getSceneInfo(Scene sceneToSearch)
        {
            return (from go in sceneToSearch.GetRootGameObjects()
                where go.tag == "SceneInfo"
                select go.GetComponent<SceneInfo>()).FirstOrDefault();
        }

        private bool IsGameplayLoaded(Scene geometryScene)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).name == geometryScene.name + "_Gameplay")
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}