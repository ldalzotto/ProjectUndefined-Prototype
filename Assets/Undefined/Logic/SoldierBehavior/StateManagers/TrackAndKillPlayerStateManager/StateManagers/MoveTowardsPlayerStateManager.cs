using System;
using AIObjects;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace SoliderAIBehavior
{
    public struct MoveTowardsPlayerStateManagerExternalCallbacks
    {
        public Func<IAgentMovementCalculationStrategy, AIMovementSpeedAttenuationFactor, NavMeshPathStatus> SetAIAgentDestinationAction;

        public MoveTowardsPlayerStateManagerExternalCallbacks(Func<IAgentMovementCalculationStrategy, AIMovementSpeedAttenuationFactor, NavMeshPathStatus> aiAgentDestinationAction)
        {
            SetAIAgentDestinationAction = aiAgentDestinationAction;
        }
    }

    /// <summary>
    /// Moves the <see cref="SoliderEnemy"/> to the position of the <see cref="InteractiveObjectTag.IsPlayer"/> object.
    /// (<see cref="TrackAndKillPlayerStateEnum.MOVE_TOWARDS_PLAYER"/>)
    /// </summary>
    class MoveTowardsPlayerStateManager : SoldierStateManager
    {
        private TrackAndKillPlayerStateBehavior TrackAndKillAIbehaviorRef;
        private SoldierAIBehaviorDefinition SoldierAIBehaviorDefinition;
        private CoreInteractiveObject AssociatedInteractiveObject;
        private PlayerObjectStateDataSystem PlayerObjectStateDataSystem;
        private WeaponFiringAreaSystem WeaponFiringAreaSystem;
        private MoveTowardsPlayerStateManagerExternalCallbacks MoveTowardsPlayerStateManagerExternalCallbacks;

        public MoveTowardsPlayerStateManager(TrackAndKillPlayerStateBehavior trackAndKillAIbehaviorRef, SoldierAIBehaviorDefinition SoldierAIBehaviorDefinition,
            CoreInteractiveObject AssociatedInteractiveObject, PlayerObjectStateDataSystem playerObjectStateDataSystem, WeaponFiringAreaSystem WeaponFiringAreaSystem,
            MoveTowardsPlayerStateManagerExternalCallbacks MoveTowardsPlayerStateManagerExternalCallbacks)
        {
            TrackAndKillAIbehaviorRef = trackAndKillAIbehaviorRef;
            this.SoldierAIBehaviorDefinition = SoldierAIBehaviorDefinition;
            this.AssociatedInteractiveObject = AssociatedInteractiveObject;
            PlayerObjectStateDataSystem = playerObjectStateDataSystem;
            this.WeaponFiringAreaSystem = WeaponFiringAreaSystem;
            this.MoveTowardsPlayerStateManagerExternalCallbacks = MoveTowardsPlayerStateManagerExternalCallbacks;
        }

        public override void Tick(float d)
        {
            this.MoveTowardsPlayerStateManagerExternalCallbacks.SetAIAgentDestinationAction.Invoke(new ForwardAgentMovementCalculationStrategy(new AIDestination() {WorldPosition = this.PlayerObjectStateDataSystem.PlayerObject().InteractiveGameObject.GetTransform().WorldPosition}),
                AIMovementSpeedAttenuationFactor.RUN);
            this.SwitchToShootingAtPlayer();
        }

        private void SwitchToShootingAtPlayer()
        {
            if (
                SoldierAIBehaviorUtil.PlayerInSightButNoObstaclesBetween(this.PlayerObjectStateDataSystem, this.WeaponFiringAreaSystem) &&
                Vector3.Distance(this.PlayerObjectStateDataSystem.PlayerObject().InteractiveGameObject.GetTransform().WorldPosition, this.AssociatedInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition)
                <= this.SoldierAIBehaviorDefinition.MaxDistancePlayerCatchUp)
            {
                Debug.Log(MyLog.Format("MoveTowardsPlayerStateManager to GO_ROUND_PLAYER"));
                this.TrackAndKillAIbehaviorRef.SetState(TrackAndKillPlayerStateEnum.SHOOTING_AT_PLAYER);
            }
        }

        /// <summary>
        /// When the PlayerObject has just been out of sight, the <see cref="SoldierAIBehavior"/> state moves to <see cref="TrackAndKillPlayerStateEnum.GO_ROUND_PLAYER"/>
        /// if the PlayerObject is behind an <see cref="InteractiveObjectTag.IsObstacle"/> object.
        /// </summary>
        public override void OnPlayerObjectJustOutOfSight(CoreInteractiveObject NotInSightInteractiveObject)
        {
            if (this.WeaponFiringAreaSystem.AreObstaclesInside())
            {
                Debug.Log(MyLog.Format("MoveTowardsPlayerStateManager to GO_ROUND_PLAYER"));
                this.TrackAndKillAIbehaviorRef.SetState(TrackAndKillPlayerStateEnum.GO_ROUND_PLAYER);
            }
            else
            {
                Debug.Log(MyLog.Format("MoveTowardsPlayerStateManager to MOVE_TO_LAST_SEEN_PLAYER_POSITION"));
                this.TrackAndKillAIbehaviorRef.SetState(TrackAndKillPlayerStateEnum.MOVE_TO_LAST_SEEN_PLAYER_POSITION);
            }
        }
    }
}