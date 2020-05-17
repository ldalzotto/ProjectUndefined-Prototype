using OdinSerializer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreGame
{
    public interface IByEnumProperty
    {
#if UNITY_EDITOR
        void TryGetValue(Enum e, out object retrievedObject);
#endif
    }

    [System.Serializable]
    public abstract class ByEnumProperty<K, V> : SerializedScriptableObject, IByEnumProperty where K : Enum
    {
        public Dictionary<K, V> Values = new Dictionary<K, V>();

#if UNITY_EDITOR
        public void TryGetValue(Enum e, out object retrievedObject)
        {
            V value;
            this.Values.TryGetValue((K)e, out value);
            retrievedObject = value;
        }
#endif
    }
}