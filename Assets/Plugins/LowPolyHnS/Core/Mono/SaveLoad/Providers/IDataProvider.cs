using System;
using UnityEngine;

namespace LowPolyHnS.Core
{
    [Serializable]
    public abstract class IDataProvider : ScriptableObject
    {
        public string title = "";
        public string description = "";

        // ABSTRACT METHODS: ----------------------------------------------------------------------

        public abstract string GetString(string key, string defaultValue = "");
        public abstract void SetString(string key, string value);

        public abstract void DeleteAll();
        public abstract void DeleteKey(string key);
        public abstract bool HasKey(string key);

        // VIRTUAL METHODS: -----------------------------------------------------------------------

        public virtual float GetFloat(string key, float defaultValue = 0.0f)
        {
            float result = defaultValue;
            float.TryParse(GetString(key), out result);

            return result;
        }

        public virtual int GetInt(string key, int defaultValue = 0)
        {
            int result = defaultValue;
            int.TryParse(GetString(key), out result);

            return result;
        }

        public virtual void SetFloat(string key, float value)
        {
            SetString(key, value.ToString());
        }

        public virtual void SetInt(string key, int value)
        {
            SetString(key, value.ToString());
        }
    }
}