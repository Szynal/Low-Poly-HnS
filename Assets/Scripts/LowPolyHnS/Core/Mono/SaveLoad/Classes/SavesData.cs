using System;
using System.Collections.Generic;
using UnityEngine;

namespace LowPolyHnS.Core
{
    public class SavesData
    {
        private const string STORE_KEYFMT = "gamedata:profiles";

        // CLASSES: -------------------------------------------------------------------------------

        [Serializable]
        public class Profile
        {
            public string date;
        }

        [Serializable]
        public class Profiles : SerializableDictionaryBase<int, Profile>
        {
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public Profiles profiles = new Profiles();

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public SavesData(SaveLoadManager manager)
        {
            string data = DatabaseGeneral
                .Load()
                .GetDataProvider()
                .GetString(STORE_KEYFMT, string.Empty);

            if (!string.IsNullOrEmpty(data))
            {
                profiles = JsonUtility.FromJson<Profiles>(data);
            }

            manager.onSave += OnSave;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnSave(int profile)
        {
            if (!profiles.ContainsKey(profile))
            {
                profiles.Add(profile, new Profile());
            }

            profiles[profile].date = DateTime.Now.ToString();
            DatabaseGeneral
                .Load()
                .GetDataProvider()
                .SetString(STORE_KEYFMT, JsonUtility.ToJson(profiles));
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public int GetLastSave()
        {
            int profile = -1;
            DateTime maxDate = DateTime.MinValue;

            foreach (KeyValuePair<int, Profile> item in profiles)
            {
                DateTime itemDate = DateTime.Parse(item.Value.date);
                if (DateTime.Compare(itemDate, maxDate) > 0)
                {
                    profile = item.Key;
                    maxDate = itemDate;
                }
            }

            return profile;
        }

        public int GetSavesCount()
        {
            return profiles.Count;
        }

        public Profile GetProfileInfo(int profile)
        {
            if (profiles.ContainsKey(profile))
            {
                return profiles[profile];
            }

            return null;
        }
    }
}