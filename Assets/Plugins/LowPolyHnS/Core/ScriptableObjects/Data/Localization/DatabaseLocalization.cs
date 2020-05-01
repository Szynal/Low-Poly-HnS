using System;
using System.Collections.Generic;
using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Localization
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    public class DatabaseLocalization : IDatabase
    {
        [Serializable]
        public class TranslationContent
        {
            public SystemLanguage language;
            public string text;
        }

        [Serializable]
        public class TranslationStrings
        {
            public int key;
            public string placeholder;
            public List<TranslationContent> content;

            public TranslationStrings(int key)
            {
                this.key = key;
                placeholder = "";
                content = new List<TranslationContent>();
            }
        }

        [Serializable]
        public class TranslationLanguage
        {
            public SystemLanguage language;
            public string updateDate;
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        private Dictionary<int, Dictionary<SystemLanguage, string>> _content;

        public List<TranslationLanguage> languages = new List<TranslationLanguage>();
        public List<TranslationStrings> translationList = new List<TranslationStrings>();

        // PUBLIC METHODS: ------------------------------------------------------------------------

        private void BuildContentDictionary()
        {
            _content = new Dictionary<int, Dictionary<SystemLanguage, string>>();

            int translationListCount = translationList.Count;
            for (int i = 0; i < translationListCount; ++i)
            {
                int key = translationList[i].key;
                _content.Add(key, new Dictionary<SystemLanguage, string>());

                int contentCount = translationList[i].content.Count;
                for (int j = 0; j < contentCount; ++j)
                {
                    SystemLanguage language = translationList[i].content[j].language;
                    string text = translationList[i].content[j].text;
                    _content[key].Add(language, text);
                }
            }
        }

        public string GetText(LocString locString, SystemLanguage language = SystemLanguage.Unknown)
        {
            if (_content == null) BuildContentDictionary();

            if (locString.translationID == 0) return locString.content;
            if (language == SystemLanguage.Unknown) language = GetMainLanguage();
            if (language == GetMainLanguage()) return locString.content;

            if (!_content.ContainsKey(locString.translationID))
            {
                Debug.LogWarningFormat("Can't find localization key {0}", locString.translationID);
                return locString.content;
            }

            if (!_content[locString.translationID].ContainsKey(language))
            {
                Debug.LogWarningFormat("Can't find localization language {0} for key {1}", language,
                    locString.translationID);
                return locString.content;
            }

            return _content[locString.translationID][language];
        }

        public SystemLanguage GetMainLanguage()
        {
            if (languages.Count > 0) return languages[0].language;
            return SystemLanguage.Unknown;
        }

        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public static DatabaseLocalization Load()
        {
            return LoadDatabase<DatabaseLocalization>();
        }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

#if UNITY_EDITOR

        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            Setup<DatabaseLocalization>();
        }

        public override int GetSidebarPriority()
        {
            return 2;
        }

#endif
    }
}