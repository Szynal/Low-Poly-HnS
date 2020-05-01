using System;
using System.Collections.Generic;
using UnityEngine;

namespace LowPolyHnS.Core
{
    public class KeysData
    {
        private const string STORE_KEYFMT = "gamedata:{0:D2}:key-references";

        // CLASSES: -------------------------------------------------------------------------------

        [Serializable]
        private class Data
        {
            public List<string> keys = new List<string>();

            public Data()
            {
            }

            public Data(HashSet<string> hashset)
            {
                keys = new List<string>(hashset);
            }
        }

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public void Update(int profile, List<string> keys)
        {
            Data data = GetCurrentKeys(profile);
            HashSet<string> hash = new HashSet<string>(data.keys);

            foreach (string key in keys)
                if (!hash.Contains(key))
                    hash.Add(key);

            DatabaseGeneral.Load().GetDataProvider().SetString(
                GetKey(profile),
                JsonUtility.ToJson(new Data(hash))
            );
        }

        public void Delete(int profile)
        {
            Data data = GetCurrentKeys(profile);
            IDataProvider provider = DatabaseGeneral.Load().GetDataProvider();

            foreach (string key in data.keys) provider.DeleteKey(key);
            provider.DeleteKey(GetKey(profile));
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private Data GetCurrentKeys(int profile)
        {
            IDataProvider provider = DatabaseGeneral.Load().GetDataProvider();
            if (provider == null) return new Data();

            string strKeys = provider.GetString(GetKey(profile));
            return string.IsNullOrEmpty(strKeys)
                ? new Data()
                : JsonUtility.FromJson<Data>(strKeys);
        }

        private string GetKey(int profile)
        {
            return string.Format(STORE_KEYFMT, profile);
        }
    }
}