using System;
using AIObjects;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace SoliderAIBehavior
{
    public struct MoveToLastSeenPlayerPositionStateManagerExternalCallbacks
    {
        public Func<IAgentMovementCalculationStrategy, AIMovementSpeedAttenuationFactor, NavMeshPathStatus> SetDestinationAction;
        public Action AskedToExitTrackAndKillPlayerBehaviorAction;

        public MoveToLastSeenPlayerPositionStateManagerExternalCallbacks(Func<IAgentMovementCalculationStrategy, AIMovementSpeedAttenuationFactor, NavMeshPathStatus> destinationAction, Action askedToExitTrackAndKillPlayerBehaviorAction)
        {
            SetDestinationAction = destinationAction;
            AskedToExitTrackAndKillPlayerBehaviorAction = askedToExitTrackAndKillPlayerBehaviorAction;
        }
    }

    /// <summary>
    /// This state moves the PlayerObject to <see cref="PlayerObjectStateDataSystem.LastPlayerSeenPosition"/>.
    /// When the Player is in sight, it automacially switchs state to <see cref="TrackAndKillPlayerStateEnum.MOVE_TOWARDS_PLAYER"/>.
    /// </summary>
    public class MoveToLastSeenPlayerPositionStateManager : SoldierStateManager
    {
        private TrackAndKillPlayerBehavior TrackAndKillPlayerbehaviorRef;
        private PlayerObjectStateDataSystem PlayerObjectStateDataSystem;
        private MoveToLastSeenPlayerPositionStateManagerExternalCallbacks MoveToLastSeenPlayerPositionStateManagerExternalCallbacks;

        public MoveToLastSeenPlayerPositionStateManager(TrackAndKillPlayerBehavior trackAndKillPlayerbehaviorRef, PlayerObjectStateDataSystem playerObjectStateDataSystem,
            MoveToLastSeenPlayerPositionStateManagerExternalCallbacks MoveToLastSeenPlayerPositionStateManagerExternalCallbacks)
        {
            TrackAndKillPlayerbehaviorRef = trackAndKillPlayerbehaviorRef;
            PlayerObjectStateDataSystem = playerObjectStateDataSystem;
            this.MoveToLastSeenPlayerPositionStateManagerExternalCallbacks = MoveToLastSeenPlayerPositionStateManagerExternalCallbacks;
        }

        public override void OnStateEnter()
        {
            if (!this.PlayerObjectStateDataSystem.HasPlayerBeenSeenAtLeastOneTime
                 || MoveToLastSeenPlayerPosition() == NavMeshPathStatus.PathInvalid)
            {
                Debug.Log(MyLog.Format("Exit TrackAndKillPlayerBehavior"));
                this.MoveToLastSeenPlayerPositionStateManagerExternalCallbacks.AskedToExitTrackAndKillPlayerBehaviorAction.Invoke();
            }
        }

        /// <summary>
        /// The <see cref="SoliderEnemy"/> destination is set from data provided by <see cref="PlayerObjectStateDataSystem"/>.
        /// The <see cref="SoliderEnemy"/> destination also include the last seen Rotation. This behavior allows that when the Player has not been found, to look at it's escape destination.
        /// </summary>
        /// <returns></returns>
        private NavMeshPathStatus MoveToLastSeenPlayerPosition()
        {
            return this.MoveToLastSeenPlayerPositionStateManagerExternalCallbacks.SetDestinationAction.Invoke(new ForwardAgentMovementCalculationStrategy(new AIDestination(this.PlayerObjectStateDataSystem.LastPlayerSeenPosition.WorldPosition,
                    this.PlayerObjectStateDataSystem.LastPlayerSeenPosition.WorldRotation)),
                AIMovementSpeedAttenuationFactor.RUN);
        }

        /// <summary>
        /// When the Player is in sight, the <see cref="SoliderEnemy"/> rushes the Payer
        /// </summary>
        public override void Tick(float d)
        {
            if (this.PlayerObjectStateDataSystem.IsPlayerInSight.GetValue())
            {
                Debug.Log(MyLog.Format("MoveToLastSeenPlayerPositionStateManager to MOVE_TOWARDS_PLAYER"));
                this.TrackAndKillPlayerbehaviorRef.SetState(TrackAndKillPlayerStateEnum.MOVE_TOWARDS_PLAYER);
            }
        }

        /// <summary>
        /// If this event is triggered, this means that nothing has happened
        /// </summary>
        public override void OnDestinationReached()
        {
            Debug.Log(MyLog.Format("Exit TrackAndKillPlayerBehavior"));
            this.MoveToLastSeenPlayerPositionStateManagerExternalCallbacks.AskedToExitTrackAndKillPlayerBehaviorAction.Invoke();
        }
    }
}