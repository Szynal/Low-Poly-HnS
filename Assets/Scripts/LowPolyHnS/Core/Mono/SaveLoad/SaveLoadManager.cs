using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("LowPolyHnS/Managers/SaveLoadManager", 100)]
    public class SaveLoadManager : Singleton<SaveLoadManager>
    {
        public enum Priority
        {
            High = 0,
            Normal = 50,
            Low = 100
        }

        [Serializable]
        public class Storage
        {
            public IGameSave target;
            public int priority;

            public Storage(IGameSave target, int priority = (int) Priority.Normal)
            {
                this.target = target;
                this.priority = priority;
            }
        }

        public class ProfileEvent : UnityEvent<int, int>
        {
        }

        // CONST & STATIC PROPERTIES: -------------------------------------------------------------

        private const string STORE_KEYFMT = "gamedata:{0:D2}:{1}";

        // PROPERTIES: ----------------------------------------------------------------------------

        public static bool IsLoading { get; private set; }
        public static bool IsProfileLoaded { get; private set; }

        public static int ActiveProfile { get; private set; }
        public static int LoadedProfile { get; private set; }

        public SavesData savesData { get; private set; }

        private ScenesData scenesData;
        private KeysData keysData;

        private List<Storage> storage;
        private Dictionary<string, object> data;

        public UnityAction<int> onSave;
        public UnityAction<int> onLoad;

        public ProfileEvent eventOnChangeProfile = new ProfileEvent();

        // INITIALIZE: ----------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeOnLoad()
        {
            Instance.WakeUp();
        }

        protected override void OnCreate()
        {
            savesData = new SavesData(this);
            scenesData = new ScenesData(SceneManager.GetActiveScene().name);
            keysData = new KeysData();

            SceneManager.sceneLoaded += OnLoadScene;
            SceneManager.sceneUnloaded += OnUnloadScene;
        }

        private void OnLoadScene(Scene scene, LoadSceneMode mode)
        {
            scenesData.Add(scene.name, mode);
        }

        private void OnUnloadScene(Scene scene)
        {
            scenesData.Remove(scene.name);
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void SetCurrentProfile(int profile)
        {
            int prevProfile = ActiveProfile;
            ActiveProfile = profile;

            if (prevProfile != profile && eventOnChangeProfile != null)
            {
                eventOnChangeProfile.Invoke(prevProfile, profile);
            }
        }

        public int GetCurrentProfile()
        {
            return ActiveProfile;
        }

        public void DeleteProfile(int profile)
        {
            keysData.Delete(profile);
            savesData.profiles.Remove(profile);

            if (ActiveProfile == profile)
            {
                data = new Dictionary<string, object>();
                storage = new List<Storage>();
            }
        }

        public void Initialize(IGameSave gameSave, int priority = (int) Priority.Normal,
            bool limitOnLoad = false)
        {
            if (storage == null) storage = new List<Storage>();
            if (data == null) data = new Dictionary<string, object>();

            string key = gameSave.GetUniqueName();
            int index = -1;

            for (int i = storage.Count - 1; i >= 0; --i)
            {
                if (storage[i] == null || storage[i].target == null)
                {
                    storage.RemoveAt(i);
                    continue;
                }

                if (storage[i].target.GetUniqueName() == key)
                {
                    index = i;
                    break;
                }
            }

            if (index >= 0)
            {
                storage[index].target = gameSave;
                storage[index].priority = priority;
            }
            else
            {
                storage.Add(new Storage(gameSave, priority));
            }

            if (!limitOnLoad)
            {
                if (data.ContainsKey(key))
                {
                    gameSave.OnLoad(data[key]);
                }
                else if (IsProfileLoaded)
                {
                    LoadItem(gameSave, ActiveProfile);
                }
            }
        }

        public void Save(int profile)
        {
            if (onSave != null) onSave.Invoke(profile);
            if (storage == null) storage = new List<Storage>();
            if (data == null) data = new Dictionary<string, object>();

            SetCurrentProfile(profile);

            if (DatabaseGeneral.Load().saveScenes)
            {
                object saveData = scenesData.GetSaveData();
                data[scenesData.GetUniqueName()] = saveData;
            }

            for (int i = storage.Count - 1; i >= 0; --i)
            {
                IGameSave item = storage[i].target;
                if (item == null)
                {
                    storage.RemoveAt(i);
                    continue;
                }

                object saveData = item.GetSaveData();
                if (saveData == null) continue;

                if (!saveData.GetType().IsSerializable)
                {
                    throw new NonSerializableException(saveData.GetType().ToString());
                }

                data[item.GetUniqueName()] = saveData;
            }

            List<string> keys = new List<string>();
            foreach (KeyValuePair<string, object> item in data)
            {
                string serializedSaveData = JsonUtility.ToJson(item.Value, false);
                string key = GetKeyName(profile, item.Key);

                keys.Add(key);
                DatabaseGeneral.Load().GetDataProvider().SetString(key, serializedSaveData);
            }

            keysData.Update(profile, keys);
        }

        public void Load(int profile, Action callback = null)
        {
            SetCurrentProfile(profile);
            IsLoading = true;
            LoadedProfile = profile;

            if (storage == null) storage = new List<Storage>();
            data = new Dictionary<string, object>();

            storage.Sort((a, b) =>
            {
                if (a.priority < b.priority) return 1;
                if (a.priority > b.priority) return -1;
                return 0;
            });

            StartCoroutine(CoroutineLoad(profile, callback));
        }

        private IEnumerator CoroutineLoad(int profile, Action callback)
        {
            string key = GetKeyName(profile, scenesData.GetUniqueName());
            string serializedData = DatabaseGeneral.Load().GetDataProvider().GetString(key);

            if (DatabaseGeneral.Load().saveScenes)
            {
                object genericData = JsonUtility.FromJson(
                    serializedData,
                    scenesData.GetSaveDataType()
                );

                yield return scenesData.OnLoad(genericData);
            }

            for (int i = storage.Count - 1; i >= 0; --i)
            {
                IGameSave item = storage[i].target;

                if (item == null)
                {
                    storage.RemoveAt(i);
                    continue;
                }

                item.ResetData();
                LoadItem(item, profile);
            }

            IsLoading = false;
            IsProfileLoaded = true;

            if (onLoad != null) onLoad.Invoke(profile);
            if (callback != null) callback.Invoke();
        }

        public void LoadLast(Action callback = null)
        {
            int profile = savesData.GetLastSave();
            if (profile >= 0) Load(profile, callback);
        }

        public int GetSavesCount()
        {
            return savesData.GetSavesCount();
        }

        public SavesData.Profile GetProfileInfo(int profile)
        {
            return savesData.GetProfileInfo(profile);
        }

        public void OnDestroyIGameSave(IGameSave gameSave)
        {
            if (isExiting) return;
            if (gameSave == null || data == null) return;
            if (IsLoading && (!IsProfileLoaded || ActiveProfile != LoadedProfile)) return;

            object saveData = gameSave.GetSaveData();
            data[gameSave.GetUniqueName()] = saveData;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void LoadItem(IGameSave gameSave, int profile)
        {
            if (gameSave == null) return;

            string key = GetKeyName(profile, gameSave.GetUniqueName());
            string serializedData = DatabaseGeneral.Load().GetDataProvider().GetString(key);

            if (!string.IsNullOrEmpty(serializedData))
            {
                Type type = gameSave.GetSaveDataType();
                object genericData = JsonUtility.FromJson(serializedData, type);

                gameSave.OnLoad(genericData);
            }
        }

        private string GetKeyName(int profile, string key)
        {
            return string.Format(STORE_KEYFMT, profile, key);
        }

        // EXCEPTIONS: ----------------------------------------------------------------------------

        [Serializable]
        private class NonSerializableException : Exception
        {
            private const string MESSAGE = "Unable to serialize: {0}. Add [System.Serializable]";

            public NonSerializableException(string key) : base(string.Format(MESSAGE, key))
            {
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    // INTERFACE ISAVEGAME: -----------------------------------------------------------------------
    ///////////////////////////////////////////////////////////////////////////////////////////////

    public interface IGameSave
    {
        string GetUniqueName();

        Type GetSaveDataType();
        object GetSaveData();

        void ResetData();
        void OnLoad(object generic);
    }
}