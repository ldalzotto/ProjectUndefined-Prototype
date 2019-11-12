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

        /// <summary>
        /// The maximum distance between the <see cref="SoliderEnemy"/> and the <see cref="PlayerObject.PlayerInteractiveObject"/> to switch
        /// the <see cref="SoldierAIBehavior"/> state from <see cref="SoldierAIStateEnum.MOVE_TOWARDS_PLAYER"/> too <see cref="SoldierAIStateEnum.SHOOTING_AT_PLAYER"/>. <br/>
        /// This is to avoid starting shooting at the exact point where the player enters on range. That may cause some frenetic
        /// behavior switching.
        /// </summary>
        [WireCircleWorld(R = 0f, G = 1f, B = 1f, UseTransform = true, RadiusFieldName = nameof(SoldierAIBehaviorDefinition.MaxDistancePlayerCatchUp))]
        public float MaxDistancePlayerCatchUp;
    }
}