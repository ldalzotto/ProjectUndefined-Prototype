using System.Collections.Generic;
using System.Linq;

namespace CoreGame
{
    public class MultiValueDictionary<K, V> : Dictionary<K, List<V>>
    {
        private List<V> TrackedValues = new List<V>();

        public void MultiValueAdd(K key, V value)
        {
            if (!this.ContainsKey(key))
            {
                this[key] = new List<V>();
            }
            this[key].Add(value);
            this.UpdateTrackedValues();
        }

        public bool MultiValueRemove(K key)
        {
            if (this.ContainsKey(key))
            {
                if (this.Remove(key))
                {
                    this.UpdateTrackedValues();
                    return true;
                }
            }
            return false;
        }

        public bool MultiValueRemove(K key, V value)
        {
            if (this.ContainsKey(key))
            {
                if (this[key].Remove(value))
                {
                    if (this[key].Count == 0) { this.Remove(key); }
                    this.UpdateTrackedValues();
                    return true;
                }
            }
            return false;
        }

        private void UpdateTrackedValues()
        {
            this.TrackedValues = this.Values.SelectMany(v => v).ToList();
        }

        public List<V> MultiValueGetValues()
        {
            return this.TrackedValues;
        }
    }

}
