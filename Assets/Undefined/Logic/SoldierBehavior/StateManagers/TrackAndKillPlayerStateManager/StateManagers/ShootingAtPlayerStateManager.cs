using System;
using AIObjects;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using UnityEngine;

namespace SoliderAIBehavior
{
    /// <summary>
    /// Orient the <see cref="SoliderEnemy"/> and trigger its <see cref="SoliderEnemy.AskToFireAFiredProjectile()"/> event.
    /// <see cref="TrackAndKillPlayerStateEnum.SHOOTING_AT_PLAYER"/>
    /// </summary>
    class ShootingAtPlayerStateManager : SoldierStateManager
    {
        private TrackAndKillPlayerStateBehavior TrackAndKillAIbehaviorRef;
        private PlayerObjectStateDataSystem PlayerObjectStateDataSystem;
        private CoreInteractiveObject AssociatedInteractiveObject;
        private SoldierAIBehaviorDefinition SoldierAIBehaviorDefinition;

        private WeaponFiringAreaSystem WeaponFiringAreaSystem;

        #region Callbacks

        private ISetAIAgentDestinationActionCallback ISetAIAgentDestinationActionCallback;
        private IFiringProjectileCallback IFiringProjectileCallback;
        private IShootingAtPlayerWorkflowCallback IShootingAtPlayerWorkflowCallback;

        #endregion

        public ShootingAtPlayerStateManager(TrackAndKillPlayerStateBehavior trackAndKillAIbehaviorRef, PlayerObjectStateDataSystem playerObjectStateDataSystem,
            CoreInteractiveObject associatedInteractiveObject, SoldierAIBehaviorDefinition SoldierAIBehaviorDefinition, WeaponFiringAreaSystem WeaponFiringAreaSystem,
            ISetAIAgentDestinationActionCallback ISetAIAgentDestinationActionCallback, IFiringProjectileCallback IFiringProjectileCallback,
            IShootingAtPlayerWorkflowCallback IShootingAtPlayerWorkflowCallback)
        {
            this.TrackAndKillAIbehaviorRef = trackAndKillAIbehaviorRef;
            PlayerObjectStateDataSystem = playerObjectStateDataSystem;
            AssociatedInteractiveObject = associatedInteractiveObject;
            this.SoldierAIBehaviorDefinition = SoldierAIBehaviorDefinition;
            this.WeaponFiringAreaSystem = WeaponFiringAreaSystem;
            this.ISetAIAgentDestinationActionCallback = ISetAIAgentDestinationActionCallback;
            this.IFiringProjectileCallback = IFiringProjectileCallback;
            this.IShootingAtPlayerWorkflowCallback = IShootingAtPlayerWorkflowCallback;
        }

        /// <summary>
        /// Wen ensure that the <see cref="SoliderEnemy"/> is not moving.
        /// </summary>
        public override void OnStateEnter()
        {
            this.IShootingAtPlayerWorkflowCallback.OnShootingAtPlayerStartAction.Invoke();
            this.ISetAIAgentDestinationActionCallback.ClearAIAgentPathAction.Invoke();
        }

        public override void OnStateExit()
        {
            this.IShootingAtPlayerWorkflowCallback.OnShootingAtPlayerEndAction.Invoke();
        }

        public override void Tick(float d)
        {
            if (!this.PlayerObjectStateDataSystem.IsPlayerInSight.GetValue())
            {
                if (this.WeaponFiringAreaSystem.AreObstaclesInside())
                {
                    Debug.Log(MyLog.Format("ShootingAtPlayerStateManager to GO_ROUND_PLAYER"));
                    this.TrackAndKillAIbehaviorRef.SetState(TrackAndKillPlayerStateEnum.GO_ROUND_PLAYER);
                }
                else
                {
                    Debug.Log(MyLog.Format("ShootingAtPlayerStateManager to MOVE_TOWARDS_PLAYER"));
                    this.TrackAndKillAIbehaviorRef.SetState(TrackAndKillPlayerStateEnum.MOVE_TO_LAST_SEEN_PLAYER_POSITION);
                }
            }
            else
            {
                if (this.WeaponFiringAreaSystem.AreObstaclesInside())
                {
                    Debug.Log(MyLog.Format("ShootingAtPlayerStateManager to GO_ROUND_PLAYER"));
                    this.TrackAndKillAIbehaviorRef.SetState(TrackAndKillPlayerStateEnum.GO_ROUND_PLAYER);
                }
                else
                {
                    if (Vector3.Distance(this.PlayerObjectStateDataSystem.LastPlayerSeenPosition.WorldPosition, this.AssociatedInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition) <= this.SoldierAIBehaviorDefinition.MinDistanceFromPlayerToStopWhenMovingWhileShootingAtPlayer)
                    {
                        this.ISetAIAgentDestinationActionCallback.SetAIAgentSpeedAttenuationAction.Invoke(AIMovementSpeedAttenuationFactor.ZERO);
                        this.ISetAIAgentDestinationActionCallback.SetAIAgentDestinationAction.Invoke(
                            new LookingAtAgentMovementCalculationStrategy(new AIDestination(this.AssociatedInteractiveObject.InteractiveGameObject.Agent.transform.position, null), this.WeaponFiringAreaSystem.GetPredictedTransform()));
                    }
                    else
                    {
                        this.ISetAIAgentDestinationActionCallback.SetAIAgentSpeedAttenuationAction.Invoke(AIMovementSpeedAttenuationFactor.WALK);
                        this.ISetAIAgentDestinationActionCallback.SetAIAgentDestinationAction.Invoke(
                            new LookingAtAgentMovementCalculationStrategy(new AIDestination(this.PlayerObjectStateDataSystem.LastPlayerSeenPosition.WorldPosition, null), this.WeaponFiringAreaSystem.GetPredictedTransform()));
                    }

//                    OrientToTarget(this.PlayerObjectStateDataSystem.PlayerObject());
                    FireProjectile();
                }
            }
        }

        /// <summary>
        /// The firing projectile method can be called every frame without worrying about spwaning multiples projectiles. <br/>
        /// Effective projectile spawn is ensured by <see cref="Weapon.WeaponRecoilTimeManager"/>.
        /// </summary>
        private void FireProjectile()
        {
            var targettingDirection = this.WeaponFiringAreaSystem.GetWorldRayForwardDirection();
            this.IFiringProjectileCallback.AskToFireAFiredprojectile_WithWorldDirection_Action.Invoke(targettingDirection);
        }
    }
}