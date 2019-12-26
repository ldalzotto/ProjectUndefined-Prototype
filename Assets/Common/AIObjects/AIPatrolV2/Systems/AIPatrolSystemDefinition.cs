using System;
using OdinSerializer;

namespace AIObjects
{
    [Serializable]
    public class AIPatrolSystemDefinition : SerializedScriptableObject
    {
        [Inline(CreateAtSameLevelIfAbsent = true)]
        public AIPatrolGraphV2 AIPatrolGraph;
    }
}