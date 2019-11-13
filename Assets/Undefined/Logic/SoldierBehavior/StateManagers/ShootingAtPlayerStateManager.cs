using System;
using InteractiveObjects;
using UnityEngine;

namespace SoliderAIBehavior
{
    /// <summary>
    /// Orient the <see cref="SoliderEnemy"/> and trigger its <see cref="SoliderEnemy.AskToFireAFiredProjectile()"/> event.
    /// <see cref="SoldierAIStateEnum.SHOOTING_AT_PLAYER"/>
    /// </summary>
    class ShootingAtPlayerStateManager : SoldierStateManager
    {
        private SoldierAIBehavior SoldierAIBehaviorRef;
        private PlayerObjectStateDataSystem PlayerObjectStateDataSystem;
        private CoreInteractiveObject AssociatedInteractiveObject;

        /// <summary>
        /// Clear the path of <see cref="AIObjects.AIMoveToDestinationSystem"/>
        /// </summary>
        private Action ClearPathAction;

        private Action<Vector3> AskToFireAFiredProjectileAction;

        public ShootingAtPlayerStateManager(SoldierAIBehavior SoldierAIBehaviorRef, PlayerObjectStateDataSystem playerObjectStateDataSystem,
            CoreInteractiveObject associatedInteractiveObject, Action clearPathAction, Action<Vector3> askToFireAFiredProjectileAction)
        {
            this.SoldierAIBehaviorRef = SoldierAIBehaviorRef;
            PlayerObjectStateDataSystem = playerObjectStateDataSystem;
            AssociatedInteractiveObject = associatedInteractiveObject;
            ClearPathAction = clearPathAction;
            AskToFireAFiredProjectileAction = askToFireAFiredProjectileAction;
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
                    this.SoldierAIBehaviorRef.SetState(SoldierAIStateEnum.GO_ROUND_PLAYER);
                }
                else
                {
                    Debug.Log(MyLog.Format("ShootingAtPlayerStateManager to MOVE_TOWARDS_PLAYER"));
                    this.SoldierAIBehaviorRef.SetState(SoldierAIStateEnum.MOVE_TO_LAST_SEEN_PLAYER_POSITION);
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
            var WorldTargetDirection = ((PlayerObject.InteractiveGameObject.GetTransform().WorldPosition + PlayerObject.GetFiringTargetLocalPosition())
                                        - (this.AssociatedInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition + this.AssociatedInteractiveObject.GetFiringTargetLocalPosition())).normalized;
            this.AskToFireAFiredProjectileAction.Invoke(WorldTargetDirection);
        }

    }
}