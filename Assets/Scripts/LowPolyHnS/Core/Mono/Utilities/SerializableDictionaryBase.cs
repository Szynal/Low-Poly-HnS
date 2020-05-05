using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LowPolyHnS.Core
{
    [Serializable]
    public class SerializableDictionaryBase<TKey, TValue> :
        IDictionary<TKey, TValue>,
        ISerializationCallbackReceiver
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        protected Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

        [SerializeField] private TKey[] keys;
        [SerializeField] private TValue[] values;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public int Count => dictionary.Count;

        public void Add(TKey key, TValue value)
        {
            dictionary.Add(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            return dictionary.ContainsKey(key);
        }

        public ICollection<TKey> Keys => dictionary.Keys;

        public bool Remove(TKey key)
        {
            return dictionary.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values => dictionary.Values;

        public TValue this[TKey key]
        {
            get => dictionary[key];
            set => dictionary[key] = value;
        }

        public void Clear()
        {
            dictionary.Clear();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            (dictionary as ICollection<KeyValuePair<TKey, TValue>>).Add(item);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return (dictionary as ICollection<KeyValuePair<TKey, TValue>>).Contains(item);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            (dictionary as ICollection<KeyValuePair<TKey, TValue>>).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            return (dictionary as ICollection<KeyValuePair<TKey, TValue>>).Remove(item);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        public Dictionary<TKey, TValue>.Enumerator GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        // SERIALIZATION CALLBACKS: ---------------------------------------------------------------

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (keys != null && values != null)
            {
                dictionary.Clear();
                for (int i = 0; i < keys.Length; i++)
                {
                    if (i < values.Length) dictionary[keys[i]] = values[i];
                    else dictionary[keys[i]] = default;
                }
            }

            keys = null;
            values = null;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (dictionary.Count == 0)
            {
                keys = null;
                values = null;
            }
            else
            {
                int cnt = dictionary.Count;
                keys = new TKey[cnt];
                values = new TValue[cnt];
                int i = 0;
                var e = dictionary.GetEnumerator();
                while (e.MoveNext())
                {
                    keys[i] = e.Current.Key;
                    values[i] = e.Current.Value;
                    i++;
                }
            }
        }
    }
}