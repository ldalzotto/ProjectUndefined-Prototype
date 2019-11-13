using System;
using AIObjects;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace SoliderAIBehavior
{
    /// <summary>
    /// This state moves the PlayerObject to <see cref="PlayerObjectStateDataSystem.LastPlayerSeenPosition"/>.
    /// When the Player is in sight, it automacially switchs state to <see cref="SoldierAIStateEnum.MOVE_TOWARDS_PLAYER"/>.
    /// </summary>
    public class MoveToLastSeenPlayerPositionStateManager : SoldierStateManager
    {
        private SoldierAIBehavior SoldierAIBehaviorRef;
        private PlayerObjectStateDataSystem PlayerObjectStateDataSystem;
        private Func<IAgentMovementCalculationStrategy, AIMovementSpeedDefinition, NavMeshPathStatus> SetDestinationAction;

        public MoveToLastSeenPlayerPositionStateManager(SoldierAIBehavior soldierAiBehaviorRef, PlayerObjectStateDataSystem playerObjectStateDataSystem, Func<IAgentMovementCalculationStrategy, AIMovementSpeedDefinition, NavMeshPathStatus> destinationAction)
        {
            SoldierAIBehaviorRef = soldierAiBehaviorRef;
            PlayerObjectStateDataSystem = playerObjectStateDataSystem;
            SetDestinationAction = destinationAction;
        }

        public override void OnStateEnter()
        {
            if (this.SetDestinationAction.Invoke(new ForwardAgentMovementCalculationStrategy(new AIDestination() {WorldPosition = this.PlayerObjectStateDataSystem.LastPlayerSeenPosition}),
                    AIMovementSpeedDefinition.RUN) == NavMeshPathStatus.PathInvalid)
            {
                Debug.Log(MyLog.Format("MoveToLastSeenPlayerPositionStateManager to PATROLLING"));
                this.SoldierAIBehaviorRef.SetState(SoldierAIStateEnum.PATROLLING);
            }
        }
        
        /// <summary>
        /// When the Player is in sight, the <see cref="SoliderEnemy"/> rushes the Payer
        /// </summary>
        public override void Tick(float d)
        {
            if (this.PlayerObjectStateDataSystem.IsPlayerInSight)
            {
                Debug.Log(MyLog.Format("MoveToLastSeenPlayerPositionStateManager to MOVE_TOWARDS_PLAYER"));
                this.SoldierAIBehaviorRef.SetState(SoldierAIStateEnum.MOVE_TOWARDS_PLAYER);
            }
        }

        /// <summary>
        /// If this event is triggered, this means that nothing has happened
        /// </summary>
        public override void OnDestinationReached()
        {
            this.SoldierAIBehaviorRef.SetState(SoldierAIStateEnum.PATROLLING);
        }
    }
}