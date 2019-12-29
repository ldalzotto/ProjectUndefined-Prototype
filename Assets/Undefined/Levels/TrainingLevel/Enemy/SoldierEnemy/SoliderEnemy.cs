using AIObjects;
using Damage;
using Health;
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
        [VE_Nested] private ObjectMovementSpeedSystem ObjectMovementSpeedSystem;
        private SoliderEnemyAnimationStateManager SoliderEnemyAnimationStateManager;
        private SightObjectSystem SightObjectSystem;
        [VE_Nested] private SoldierStateBehavior _soldierStateBehavior;

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
            this.ObjectMovementSpeedSystem = new ObjectMovementSpeedSystem(this, SoliderEnemyDefinition.AITransformMoveManagerComponentV3, new UnConstrainedObjectSpeedAttenuationValueSystem(AIMovementSpeedAttenuationFactor.RUN), ObjectSpeedCalculationType.AGENT);
            this.AIMoveToDestinationSystem = new AIMoveToDestinationSystem(this, SoliderEnemyDefinition.AITransformMoveManagerComponentV3, this.ObjectMovementSpeedSystem.GetSpeedAttenuationFactor, this.OnAIDestinationReached);
            this.SoliderEnemyAnimationStateManager = new SoliderEnemyAnimationStateManager(this.AnimationController, SoliderEnemyDefinition.LocomotionAnimation, SoliderEnemyDefinition.SoldierAnimationSystemDefinition);
            this._soldierStateBehavior = new SoldierStateBehavior();

            this.SightObjectSystem = new SightObjectSystem(this, SoliderEnemyDefinition.SightObjectSystemDefinition, tag => tag.IsPlayer,
                this._soldierStateBehavior.OnInteractiveObjectJustOnSight, null, this._soldierStateBehavior.OnInteractiveObjectJustOutOfSight);

            this._soldierStateBehavior.Init(this, SoliderEnemyDefinition.SoldierAIBehaviorDefinition,
                new SoldierAIBehaviorExternalCallbacksV2(
                    this.SetDestination,
                    (IAgentMovementCalculationStrategy => this.SetDestination(IAgentMovementCalculationStrategy)),
                    this.SetAISpeedAttenuationFactor,
                    this.AIMoveToDestinationSystem.ClearPath,
                    this.AskToFireAFiredProjectile_ToTarget,
                    this.AskToFireAFiredProjectile_ToDirection,
                    () => SoliderEnemyDefinition.WeaponHandlingSystemDefinition.WeaponHandlingFirePointOriginLocalDefinition,
                    this.OnShootingAtPlayerStart,
                    this.OnShootingAtPlayerEnd,
                    this.WeaponHandlingSystem
                ));
        }

        public override void Tick(float d)
        {
            this._stunningDamageDealerReceiverSystem.Tick(d);
            if (!this._stunningDamageDealerReceiverSystem.IsStunned.GetValue())
            {
                this._soldierStateBehavior.Tick(d);
                this.AIMoveToDestinationSystem.Tick(d);
            }
        }

        public override void AfterTicks(float d)
        {
            this.ObjectMovementSpeedSystem.AfterTicks();
            if (!this._stunningDamageDealerReceiverSystem.IsStunned.GetValue())
            {
                this.SoliderEnemyAnimationStateManager.SetUnscaledObjectLocalDirection(this.ObjectMovementSpeedSystem.GetLocalSpeedDirectionAttenuated());
            }

            base.AfterTicks(d);
            this.SoliderEnemyAnimationStateManager.Tick(d);
            base.UpdateAniamtions(d);
        }

        public override void Destroy()
        {
            this.SightObjectSystem.OnDestroy();
            this._soldierStateBehavior.OnDestroy();
            this.WeaponHandlingSystem.Destroy();
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

        private void OnHealthChanged(float oldVlaue, float newValue)
        {
            if (newValue <= 0f)
            {
                this.isAskingToBeDestroyed = true;
            }
        }

        public override void DealDamage(float Damage, CoreInteractiveObject DamageDealerInteractiveObject)
        {
            this._stunningDamageDealerReceiverSystem.DealDamage(Damage);
            this._soldierStateBehavior.DamageDealt(DamageDealerInteractiveObject);
        }

        #region Agent related events

        private NavMeshPathStatus SetDestination(IAgentMovementCalculationStrategy IAgentMovementCalculationStrategy)
        {
            return this.AIMoveToDestinationSystem.SetDestination(IAgentMovementCalculationStrategy);
        }

        private void SetAISpeedAttenuationFactor(AIMovementSpeedAttenuationFactor aiMovementSpeedAttenuationFactor)
        {
            this.ObjectMovementSpeedSystem.SetSpeedAttenuationFactor(aiMovementSpeedAttenuationFactor);
        }

        private void OnAIDestinationReached()
        {
            this._soldierStateBehavior.OnDestinationReached();
        }

        #endregion

        #region Projectile Events

        public override void AskToFireAFiredProjectile_Forward()
        {
            this.WeaponHandlingSystem.AskToFireAFiredProjectile_Forward();
        }

        public override void AskToFireAFiredProjectile_ToTarget(CoreInteractiveObject Target)
        {
            this.WeaponHandlingSystem.AskToFireAFiredProjectile_ToTarget(Target);
        }

        public override void AskToFireAFiredProjectile_ToDirection(Vector3 WorldDirection)
        {
            this.WeaponHandlingSystem.AskToFireAFiredProjectile_ToDirection(WorldDirection);
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
            this.SoliderEnemyAnimationStateManager.StartTargetting();
        }

        private void OnShootingAtPlayerEnd()
        {
            this.SoliderEnemyAnimationStateManager.StopTargetting();
        }

        #endregion

        public override void Init()
        {
        }
    }
}