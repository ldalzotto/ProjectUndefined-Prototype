using UnityEngine;
using System.Collections;
using OdinSerializer;

namespace InteractiveObjects
{
    [System.Serializable]
    [SceneHandleDraw]
    public class DisarmSystemDefinition : SerializedScriptableObject
    {
        [WireCircle(R = 0f, G = 0f, B = 1f)]
        public float DisarmRange;
        public float DisarmTime;
    }
    
    public class AIDisarmObjectState
    {
        public BoolVariable IsDisarming { get; private set; }

        public AIDisarmObjectState(BoolVariable IsDisarming)
        {
            this.IsDisarming = IsDisarming;
        }
    }
}
