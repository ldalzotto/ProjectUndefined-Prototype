using System;
using OdinSerializer;
using RangeObjects;

namespace Health
{
    [Serializable]
    public class RecoveringHealthEmitterSystemDefinition : SerializedScriptableObject
    {
        [Inline(CreateAtSameLevelIfAbsent = true)]
        public RangeObjectInitialization RecveringHealthTriggerDefinition;
        public float RecoveredHealth;
    }
}