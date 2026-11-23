#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VK.Utilities.Serialization
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver, IEnumerable<KeyValuePair<TKey, TValue>> where TKey : notnull 
    {
        [Serializable]
        public struct Entry
        {
            public TKey key;
            public TValue value;

            public Entry(TKey key, TValue value)
            {
                this.key = key;
                this.value = value;
            }
        }

        [SerializeField]
        private List<Entry> _entries = new();

        [NonSerialized]
        private Dictionary<TKey, TValue> _dictionary = new();


        public TValue this[TKey key]
        {
            get => _dictionary[key];
            set => _dictionary[key] = value;
        }

#region DictionaryFunction
        public bool IsEmpty => _dictionary.Count == 0;

        public int Count => _dictionary.Count;
        
        public ICollection<TKey> Keys => _dictionary.Keys;

        public ICollection<TValue> Values => _dictionary.Values;

        public void Add(TKey key, TValue value)
        {
            _dictionary.Add(key, value);
        }

        public bool TryAdd(TKey key, TValue value)
        {
            if (_dictionary.ContainsKey(key))
                return false;

            _dictionary.Add(key, value);
            return true;
        }

        public bool Remove(TKey key)
        {
            return _dictionary.Remove(key);
        }

        public bool Remove(TKey key, out TValue value)
        {
            if (_dictionary.TryGetValue(key, out value!))
            {
                _dictionary.Remove(key);
                return true;
            }

            value = default!;
            return false;
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public bool ContainsValue(TValue value)
        {
            return _dictionary.ContainsValue(value);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(key, out value!);
        }

        public TValue? GetValueOrDefault(TKey key)
        {
            return _dictionary.GetValueOrDefault(key);
        }

        public TValue GetValueOrDefault(TKey key, TValue defaultValue)
        {
            return _dictionary.GetValueOrDefault(key, defaultValue);
        }

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> factory)
        {
            if (_dictionary.TryGetValue(key, out TValue value))
                return value;

            value = factory(key);
            _dictionary.Add(key, value);

            return value;
        }

        public Dictionary<TKey, TValue>.Enumerator GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator<KeyValuePair<TKey, TValue>>
            IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }
#endregion

#region ISerializationCallbackReceiver

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            _dictionary = new Dictionary<TKey, TValue>();

            foreach (Entry entry in _entries)
            {
                if (_dictionary.ContainsKey(entry.key))
                {
                    Debug.LogWarning(
                        $"[SerializableDictionary] 중복된 Key가 발견되었습니다. " +
                        $"Key: {entry.key}, Type: {typeof(TKey).Name}");
                    continue;
                }
                _dictionary.Add(entry.key, entry.value);
            }
        }
#endregion
    }
}