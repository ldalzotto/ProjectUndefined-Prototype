using System;
using InteractiveObjects;
using UnityEngine;

namespace SoliderAIBehavior
{
    /// <summary>
    /// Orient the <see cref="SoliderEnemy"/> and trigger its <see cref="SoliderEnemy.AskToFireAFiredProjectile()"/> event.
    /// <see cref="TrackAndKillPlayerStateEnum.SHOOTING_AT_PLAYER"/>
    /// </summary>
    class ShootingAtPlayerStateManager : SoldierStateManager
    {
        private TrackAndKillPlayerBehavior TrackAndKillAIbehaviorRef;
        private PlayerObjectStateDataSystem PlayerObjectStateDataSystem;
        private CoreInteractiveObject AssociatedInteractiveObject;

        /// <summary>
        /// Clear the path of <see cref="AIObjects.AIMoveToDestinationSystem"/>
        /// </summary>
        private Action ClearPathAction;

        private Action<Vector3> AskToFireAFiredProjectileAction_WithTargetPosition;

        public ShootingAtPlayerStateManager(TrackAndKillPlayerBehavior trackAndKillAIbehaviorRef, PlayerObjectStateDataSystem playerObjectStateDataSystem,
            CoreInteractiveObject associatedInteractiveObject, Action clearPathAction, Action<Vector3> askToFireAFiredProjectileAction_WithTargetPosition)
        {
            this.TrackAndKillAIbehaviorRef = trackAndKillAIbehaviorRef;
            PlayerObjectStateDataSystem = playerObjectStateDataSystem;
            AssociatedInteractiveObject = associatedInteractiveObject;
            ClearPathAction = clearPathAction;
            AskToFireAFiredProjectileAction_WithTargetPosition = askToFireAFiredProjectileAction_WithTargetPosition;
        }

        /// <summary>
        /// Wen ensure that the <see cref="SoliderEnemy"/> is not moving.
        /// </summary>
        public override void OnStateEnter()
        {
            this.ClearPathAction.Invoke();
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
            this.AskToFireAFiredProjectileAction_WithTargetPosition.Invoke(PlayerObject.InteractiveGameObject.GetLocalToWorld().MultiplyPoint(PlayerObject.GetFiringTargetLocalPosition()));
        }
    }
}