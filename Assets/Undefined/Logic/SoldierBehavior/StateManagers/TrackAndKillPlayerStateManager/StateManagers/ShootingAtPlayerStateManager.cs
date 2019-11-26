using System;
using InteractiveObjects;
using UnityEngine;

namespace SoliderAIBehavior
{
    public struct ShootingAtPlayerStateManagerExternalCallbacks
    {
        public Action ClearAiAgentPathAction;
        public Action<Vector3> AskToFireAFiredProjectileAction_WithTargetPosition;
        public Action OnShootingAtPlayerStartAction;
        public Action OnShootingAtPlayerEndAction;

        public ShootingAtPlayerStateManagerExternalCallbacks(Action clearAiAgentPathAction, Action<Vector3> askToFireAFiredProjectileActionWithTargetPosition, 
            Action onShootingAtPlayerStartAction, Action onShootingAtPlayerEndAction)
        {
            ClearAiAgentPathAction = clearAiAgentPathAction;
            AskToFireAFiredProjectileAction_WithTargetPosition = askToFireAFiredProjectileActionWithTargetPosition;
            OnShootingAtPlayerStartAction = onShootingAtPlayerStartAction;
            OnShootingAtPlayerEndAction = onShootingAtPlayerEndAction;
        }
    }

    /// <summary>
    /// Orient the <see cref="SoliderEnemy"/> and trigger its <see cref="SoliderEnemy.AskToFireAFiredProjectile()"/> event.
    /// <see cref="TrackAndKillPlayerStateEnum.SHOOTING_AT_PLAYER"/>
    /// </summary>
    class ShootingAtPlayerStateManager : SoldierStateManager
    {
        private TrackAndKillPlayerBehavior TrackAndKillAIbehaviorRef;
        private PlayerObjectStateDataSystem PlayerObjectStateDataSystem;
        private CoreInteractiveObject AssociatedInteractiveObject;

        private ShootingAtPlayerStateManagerExternalCallbacks ShootingAtPlayerStateManagerExternalCallbacks;

        public ShootingAtPlayerStateManager(TrackAndKillPlayerBehavior trackAndKillAIbehaviorRef, PlayerObjectStateDataSystem playerObjectStateDataSystem,
            CoreInteractiveObject associatedInteractiveObject, ShootingAtPlayerStateManagerExternalCallbacks ShootingAtPlayerStateManagerExternalCallbacks)
        {
            this.TrackAndKillAIbehaviorRef = trackAndKillAIbehaviorRef;
            PlayerObjectStateDataSystem = playerObjectStateDataSystem;
            AssociatedInteractiveObject = associatedInteractiveObject;
            this.ShootingAtPlayerStateManagerExternalCallbacks = ShootingAtPlayerStateManagerExternalCallbacks;
        }

        /// <summary>
        /// Wen ensure that the <see cref="SoliderEnemy"/> is not moving.
        /// </summary>
        public override void OnStateEnter()
        {
            this.ShootingAtPlayerStateManagerExternalCallbacks.OnShootingAtPlayerStartAction.Invoke();
            this.ShootingAtPlayerStateManagerExternalCallbacks.ClearAiAgentPathAction.Invoke();
        }

        public override void OnStateExit()
        {
            this.ShootingAtPlayerStateManagerExternalCallbacks.OnShootingAtPlayerEndAction.Invoke();
        }

        public override void Tick(float d)
        {
            if (!this.PlayerObjectStateDataSystem.IsPlayerInSight)
            {
                if (SoldierAIBehaviorUtil.InteractiveObjectBeyondObstacle(this.PlayerObjectStateDataSystem.PlayerObject(), this.AssociatedInteractiveObject))
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
                var PlayerObject = this.PlayerObjectStateDataSystem.PlayerObject();
                OrientToTarget(PlayerObject);
                FireProjectile(PlayerObject);
            }
        }

        private void OrientToTarget(CoreInteractiveObject PlayerObject)
        {
            this.AssociatedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent.transform.rotation =
                Quaternion.LookRotation((PlayerObject.InteractiveGameObject.GetTransform().WorldPosition - this.AssociatedInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition).normalized, Vector3.up);
        }

        /// <summary>
        /// The firing projectile method can be called every frame without worrying about spwaning multiples projectiles. <br/>
        /// Effective projectile spawn is ensured by <see cref="Weapon.WeaponRecoilTimeManager"/>.
        /// </summary>
        private void FireProjectile(CoreInteractiveObject PlayerObject)
        {
            this.ShootingAtPlayerStateManagerExternalCallbacks.AskToFireAFiredProjectileAction_WithTargetPosition
                .Invoke(PlayerObject.InteractiveGameObject.GetLocalToWorld().MultiplyPoint(PlayerObject.GetFiringTargetLocalPosition()));
        }
    }
}