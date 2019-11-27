using AIObjects;
using Damage;
using Health;
using InteractiveObject_Animation;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using SoldierAnimation;
using SoliderAIBehavior;
using UnityEngine;
using UnityEngine.AI;
using Weapon;

namespace TrainingLevel
{
    public class SoliderEnemy : CoreInteractiveObject
    {
        [VE_Nested] private HealthSystem HealthSystem;
        [VE_Nested] private StunningDamageDealerReceiverSystem _stunningDamageDealerReceiverSystem;
        private WeaponHandlingSystem WeaponHandlingSystem;
        private FiringTargetPositionSystem FiringTargetPositionSystem;
        private AIMoveToDestinationSystem AIMoveToDestinationSystem;
        private BaseObjectAnimatorPlayableSystem BaseObjectAnimatorPlayableSystem;
        private SightObjectSystem SightObjectSystem;
        [VE_Nested] private SoldierAIBehavior SoldierAIBehavior;
        private SoldierAnimationSystem SoldierAnimationSystem;

        public SoliderEnemy(IInteractiveGameObject parent, SoliderEnemyDefinition SoliderEnemyDefinition)
        {
            parent.CreateLogicCollider(SoliderEnemyDefinition.InteractiveObjectBoxLogicColliderDefinition);
            parent.CreateAgent(SoliderEnemyDefinition.AIAgentDefinition);
            this.interactiveObjectTag = new InteractiveObjectTag() {IsTakingDamage = true};
            BaseInit(parent);
            this.HealthSystem = new HealthSystem(SoliderEnemyDefinition.HealthSystemDefinition, this.OnHealthChanged);
            this._stunningDamageDealerReceiverSystem = new StunningDamageDealerReceiverSystem(SoliderEnemyDefinition.stunningDamageDealerReceiverSystemDefinition, this.HealthSystem, this.OnStunningDamageDealingStarted, this.OnStunningDamageDealingEnded);
            this.WeaponHandlingSystem = new WeaponHandlingSystem(this, new WeaponHandlingSystemInitializationData(this, SoliderEnemyDefinition.WeaponHandlingSystemDefinition.WeaponHandlingFirePointOriginLocalDefinition,
                SoliderEnemyDefinition.WeaponHandlingSystemDefinition.WeaponDefinition));
            this.FiringTargetPositionSystem = new FiringTargetPositionSystem(SoliderEnemyDefinition.FiringTargetPositionSystemDefinition);
            this.AIMoveToDestinationSystem = new AIMoveToDestinationSystem(this, SoliderEnemyDefinition.AITransformMoveManagerComponentV3, this.OnAIDestinationReached,
                (unscaledSpeed) => this.BaseObjectAnimatorPlayableSystem.SetUnscaledObjectLocalDirection(unscaledSpeed));
            this.BaseObjectAnimatorPlayableSystem = new BaseObjectAnimatorPlayableSystem(this.AnimationController, SoliderEnemyDefinition.LocomotionAnimation);
            this.SoldierAIBehavior = new SoldierAIBehavior(this, SoliderEnemyDefinition.SoldierAIBehaviorDefinition,
                new SoldierAIBehaviorExternalCallbacks(
                    this.SetDestination,
                    this.AIMoveToDestinationSystem.ClearPath,
                    this.AskToFireAFiredProjectile_ToTarget,
                    () => SoliderEnemyDefinition.WeaponHandlingSystemDefinition.WeaponHandlingFirePointOriginLocalDefinition,
                    this.OnShootingAtPlayerStart,
                    this.OnShootingAtPlayerEnd
                ));
            this.SightObjectSystem = new SightObjectSystem(this, SoliderEnemyDefinition.SightObjectSystemDefinition, tag => tag.IsPlayer,
                this.SoldierAIBehavior.OnInteractiveObjectJustOnSight, null, this.SoldierAIBehavior.OnInteractiveObjectJustOutOfSight);
            this.SoldierAnimationSystem = new SoldierAnimationSystem(SoliderEnemyDefinition.SoldierAnimationSystemDefinition, this.AnimationController);
        }

        public override void Tick(float d)
        {
            this._stunningDamageDealerReceiverSystem.Tick(d);
            if (!this._stunningDamageDealerReceiverSystem.IsStunned.GetValue())
            {
                this.SoldierAIBehavior.Tick(d);
                this.AIMoveToDestinationSystem.Tick(d);
            }

            base.Tick(d);
        }

        public override void AfterTicks(float d)
        {
            if (!this._stunningDamageDealerReceiverSystem.IsStunned.GetValue())
            {
                this.AIMoveToDestinationSystem.AfterTicks();
            }

            base.AfterTicks(d);
        }

        public override void Destroy()
        {
            this.SightObjectSystem.OnDestroy();
            this.SoldierAIBehavior.OnDestroy();
            base.Destroy();
        }

        private void OnStunningDamageDealingStarted()
        {
            foreach (var renderer in this.InteractiveGameObject.Renderers)
            {
                renderer.material.SetColor("_BaseColor", Color.red);
                this.AnimatorPlayable.Stop();
                this.AIMoveToDestinationSystem.StopAgent();
            }
        }

        private void OnStunningDamageDealingEnded()
        {
            foreach (var renderer in this.InteractiveGameObject.Renderers)
            {
                renderer.material.SetColor("_BaseColor", Color.white);
                this.AnimatorPlayable.Play();
                this.AIMoveToDestinationSystem.EnableAgent();
            }
        }

        public override void OnHealthChanged(float oldVlaue, float newValue)
        {
            if (newValue <= 0f)
            {
                this.isAskingToBeDestroyed = true;
            }
        }

        public override void DealDamage(float Damage, CoreInteractiveObject DamageDealerInteractiveObject)
        {
            this._stunningDamageDealerReceiverSystem.DealDamage(Damage);
            this.SoldierAIBehavior.DamageDealt(DamageDealerInteractiveObject);
        }

        public override NavMeshPathStatus SetDestination(IAgentMovementCalculationStrategy IAgentMovementCalculationStrategy)
        {
            return this.AIMoveToDestinationSystem.SetDestination(IAgentMovementCalculationStrategy);
        }

        public override void SetAISpeedAttenuationFactor(AIMovementSpeedAttenuationFactor aiMovementSpeedAttenuationFactor)
        {
            this.AIMoveToDestinationSystem.SetSpeedAttenuationFactor(aiMovementSpeedAttenuationFactor);
        }

        private void OnAIDestinationReached()
        {
            this.SoldierAIBehavior.OnDestinationReached();
        }

        #region Projectile Events

        public override void AskToFireAFiredProjectile_Forward()
        {
            this.WeaponHandlingSystem.AskToFireAFiredProjectile_Forward();
        }

        public override void AskToFireAFiredProjectile_ToTarget(CoreInteractiveObject Target)
        {
            this.WeaponHandlingSystem.AskToFireAFiredProjectile_ToTarget(Target);
        }

        public override Vector3 GetWeaponWorldFirePoint()
        {
            return this.WeaponHandlingSystem.GetWorldWeaponFirePoint();
        }

        public override Vector3 GetFiringTargetLocalPosition()
        {
            return this.FiringTargetPositionSystem.GetFiringTargetLocalPosition();
        }

        #endregion

        
        #region internal Firing Events

        private void OnShootingAtPlayerStart()
        {
            this.SoldierAnimationSystem.OnShootingAtPlayerStart();
        }

        private void OnShootingAtPlayerEnd()
        {
            this.SoldierAnimationSystem.OnShootingAtPlayerEnd();
        }

        #endregion
        
        public override void Init()
        {
        }
    }
}