using System.Collections.Generic;

namespace CoreGame
{
    /// <summary>
    /// A <see cref="BufferedDictionary"/> is a dictionary that holds a <see cref="Buffer"/> in the form of a Stack. The goal of this buffer is to push dictionary entry in it
    /// that will be consumed by <see cref="UpdateAndConsumeFromBuffer"/> and updates the effective dictionary values.
    /// This is useful if dictionary values are value type and we want to update internal state of the value state and updating the dictionary.
    /// </summary>
    public class BufferedDictionary<K, V> : Dictionary<K, V>
    {
        private Stack<KeyValuePair<K, V>> Buffer = new Stack<KeyValuePair<K, V>>();

        public void PushToBuffer(KeyValuePair<K, V> entry)
        {
            this.Buffer.Push(entry);
        }

        public void UpdateAndConsumeFromBuffer()
        {
            while (Buffer.Count > 0)
            {
                var bufferEntry = this.Buffer.Pop();
                this[bufferEntry.Key] = bufferEntry.Value;
            }
        }

        public void StartBuffer()
        {
            this.ClearBuffer();
        }

        private void ClearBuffer()
        {
            this.Buffer.Clear();
        }
    }
}