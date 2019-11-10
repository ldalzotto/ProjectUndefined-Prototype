using System;
using OdinSerializer;

namespace TrainingLevel
{
    [Serializable]
    public class SoldierAIBehaviorDefinition : SerializedScriptableObject
    {
        public float MaxDistancePlayerCatchUp;
    }
}