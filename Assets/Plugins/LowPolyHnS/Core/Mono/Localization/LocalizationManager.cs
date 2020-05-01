using System;
using LowPolyHnS.Core;
using UnityEngine;
using UnityEngine.Events;

namespace LowPolyHnS.Localization
{
    [AddComponentMenu("LowPolyHnS/Managers/LocalizationManager", 100)]
    public class LocalizationManager : Singleton<LocalizationManager>, IGameSave
    {
        // PROPERTIES: -------------------------------------------------------------------------------------------------

        private DatabaseLocalization databaseLocalization;
        private SystemLanguage currentLanguage;

        public UnityAction onChangeLanguage;

        // INITIALIZER: ------------------------------------------------------------------------------------------------

        protected override void OnCreate()
        {
            databaseLocalization = IDatabase.LoadDatabaseCopy<DatabaseLocalization>();
            currentLanguage = databaseLocalization.GetMainLanguage();

            SaveLoadManager.Instance.Initialize(this);
        }

        public SystemLanguage GetCurrentLanguage()
        {
            return currentLanguage;
        }

        public void ChangeLanguage(SystemLanguage language)
        {
            int languagesCount = databaseLocalization.languages.Count;
            for (int i = 0; i < languagesCount; ++i)
            {
                if (databaseLocalization.languages[i].language == language)
                {
                    currentLanguage = language;
                    if (onChangeLanguage != null) onChangeLanguage.Invoke();

                    return;
                }
            }

            Debug.LogWarningFormat("Language {0} not available", language);
        }

        // INTERFACE ISAVELOAD: ----------------------------------------------------------------------------------------

        public string GetUniqueName()
        {
            return "localization";
        }

        public Type GetSaveDataType()
        {
            return typeof(SystemLanguage);
        }

        public object GetSaveData()
        {
            return currentLanguage;
        }

        public void ResetData()
        {
            currentLanguage = databaseLocalization.GetMainLanguage();
        }

        public void OnLoad(object generic)
        {
            SystemLanguage language = (SystemLanguage) generic;
            currentLanguage = language;
        }
    }
}