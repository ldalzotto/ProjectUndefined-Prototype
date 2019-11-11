using System;
using AIObjects;
using OdinSerializer;

namespace TrainingLevel
{
    [Serializable]
    [SceneHandleDraw]
    public class SoldierAIBehaviorDefinition : SerializedScriptableObject
    {
        [Inline(createAtSameLevelIfAbsent: true)]
        public AIPatrolSystemDefinition AIPatrolSystemDefinition;

        [WireCircleWorld(R = 0f, G = 1f, B = 1f, UseTransform = true, RadiusFieldName = nameof(SoldierAIBehaviorDefinition.MaxDistancePlayerCatchUp))]
        public float MaxDistancePlayerCatchUp;
    }
}