using System;
using AIObjects;
using OdinSerializer;
using UnityEngine.Serialization;

namespace SoliderAIBehavior
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

        /// <summary>
        /// The distance from which the AI will stop while on state <see cref="SoldierAIStateEnum.SHOOTING_AT_PLAYER"/> and moving towards player.
        /// </summary>
        [FormerlySerializedAs("DistanceToStopWhenMovingWhileShootingAtPlayer")]
        [WireCircleWorld(R = 0f, G = 1f, B = 1f, UseTransform = true, RadiusFieldName = nameof(SoldierAIBehaviorDefinition.MinDistanceFromPlayerToStopWhenMovingWhileShootingAtPlayer))]
        public float MinDistanceFromPlayerToStopWhenMovingWhileShootingAtPlayer = 10;

        /// <summary>
        /// The maximum distance crossed when the <see cref="SoldierAIBehavior"/> enters in state <see cref="MoveTowardsInterestDirectionStateManager"/>.
        /// </summary>
        [WireCircleWorld(R = 0f, G = 1f, B = 1f, UseTransform = true, RadiusFieldName = nameof(MoveTowardsInterestDirectionDistance))]
        public float MoveTowardsInterestDirectionDistance;
    }
}