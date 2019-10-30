using OdinSerializer;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ConfigurationEditor
{
    public interface IConfigurationSerialization
    {
#if UNITY_EDITOR
        void ClearEntry(Enum key);
        ScriptableObject GetEntry(Enum key);
        void GetEntryTry(Enum key, out ScriptableObject scriptableObject);
        void SetEntry(Enum key, ScriptableObject value);
        List<Enum> GetKeys();
#endif
    }

    [System.Serializable]
    public abstract class ConfigurationSerialization<K, V> : SerializedScriptableObject, IConfigurationSerialization where K : Enum where V : ScriptableObject
    {
        public Dictionary<K, V> ConfigurationInherentData = new Dictionary<K, V>() { };

#if UNITY_EDITOR
        public void SetEntry(K key, V value)
        {
            this.ClearEntry(key);
            ConfigurationInherentData.Add(key, value);
            EditorUtility.SetDirty(this);
        }

        public void SetEntry(Enum key, ScriptableObject value)
        {
            var castedKey = (K)key;
            var castedValue = (V)value;
            this.SetEntry(castedKey, castedValue);
        }

        public ScriptableObject GetEntry(Enum key)
        {
            var castedKey = (K)key; 
            return this.ConfigurationInherentData[castedKey];
        }

        public void GetEntryTry(Enum key, out ScriptableObject scriptableObject)
        {
            var castedKey = (K)key;
            this.ConfigurationInherentData.TryGetValue(castedKey, out V valueObj);
            scriptableObject = (ScriptableObject)valueObj;
        }

        private void ClearEntry(K key)
        {
            if (ConfigurationInherentData.ContainsKey(key))
            {
                ConfigurationInherentData.Remove(key);
            }
            EditorUtility.SetDirty(this);
        }

        public void ClearEntry(Enum key)
        {
            var catedKey = (K)key;
            this.ClearEntry(catedKey);
        }

        public List<Enum> GetKeys()
        {
            return this.ConfigurationInherentData.Keys.ToList().ConvertAll(e => (Enum)e);
        }

       

#endif
    }
}
