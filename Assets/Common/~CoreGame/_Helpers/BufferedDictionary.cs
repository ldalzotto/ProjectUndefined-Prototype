using System.Collections.Generic;

namespace CoreGame
{
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