using AIObjects;
using Damage;
using Health;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using InteractiveObjectAction;
using SightVisualFeedback;
using SoldierAnimation;
using SoliderAIBehavior;
using UnityEngine;
using UnityEngine.AI;
using Weapon;

namespace TrainingLevel
{
    public partial class SoliderEnemy : CoreInteractiveObject, IEM_InteractiveObjectActionPlayerSystem_Retriever, IEM_WeaponHandlingSystem_Retriever
    {
        [VE_Nested] private HealthSystem HealthSystem;
        [VE_Nested] private StunningDamageDealerReceiverSystem _stunningDamageDealerReceiverSystem;
        public WeaponHandlingSystem WeaponHandlingSystem { get; private set; }
        private FiringTargetPositionSystem FiringTargetPositionSystem;
        private AIMoveToDestinationSystem AIMoveToDestinationSystem;
        [VE_Nested] private ObjectMovementSpeedSystem ObjectMovementSpeedSystem;
        private SoliderEnemyAnimationStateManager SoliderEnemyAnimationStateManager;
        private SightObjectSystem SightObjectSystem;
        [VE_Nested] private SoldierStateBehavior _soldierStateBehavior;
        public InteractiveObjectActionPlayerSystem InteractiveObjectActionPlayerSystem { get; private set; }
        private SightVisualFeedbackSystem SightVisualFeedbackSystem;
        private SightVisualFeedbackStateBehavior SightVisualFeedbackStateBehavior;

        public SoliderEnemy(IInteractiveGameObject parent, SoliderEnemyDefinition SoliderEnemyDefinition)
        {
            var mainCamera = Camera.main;

            parent.CreateLogicCollider(SoliderEnemyDefinition.InteractiveObjectBoxLogicColliderDefinition);
            parent.CreateAgent(SoliderEnemyDefinition.AIAgentDefinition);
            this.interactiveObjectTag = new InteractiveObjectTag() {IsTakingDamage = true};
            BaseInit(parent);
            this.InteractiveObjectActionPlayerSystem = new InteractiveObjectActionPlayerSystem(this);
            this.HealthSystem = new HealthSystem(this, SoliderEnemyDefinition.HealthSystemDefinition, this.OnHealthChanged);
            this._stunningDamageDealerReceiverSystem = new StunningDamageDealerReceiverSystem(SoliderEnemyDefinition.stunningDamageDealerReceiverSystemDefinition, this.HealthSystem, this.OnStunningDamageDealingStarted, this.OnStunningDamageDealingEnded);
            this.WeaponHandlingSystem = new WeaponHandlingSystem(this,
                new WeaponHandlingSystemInitializationData(this, SoliderEnemyDefinition.WeaponHandlingSystemDefinition.WeaponHandlingFirePointOriginLocalDefinition,
                    SoliderEnemyDefinition.WeaponHandlingSystemDefinition.WeaponDefinition));
            this.FiringTargetPositionSystem = new FiringTargetPositionSystem(SoliderEnemyDefinition.FiringTargetPositionSystemDefinition);
            this.ObjectMovementSpeedSystem = new ObjectMovementSpeedSystem(this, SoliderEnemyDefinition.AITransformMoveManagerComponentV3, new UnConstrainedObjectSpeedAttenuationValueSystem(AIMovementSpeedAttenuationFactor.RUN), ObjectSpeedCalculationType.AGENT);
            this.AIMoveToDestinationSystem = new AIMoveToDestinationSystem(this, SoliderEnemyDefinition.AITransformMoveManagerComponentV3, this.OnAIDestinationReached);
            this.SoliderEnemyAnimationStateManager = new SoliderEnemyAnimationStateManager(this.AnimationController, SoliderEnemyDefinition.LocomotionAnimation, SoliderEnemyDefinition.SoldierAnimationSystemDefinition);
            this._soldierStateBehavior = new SoldierStateBehavior();

            this.SightObjectSystem = new SightObjectSystem(this, SoliderEnemyDefinition.SightObjectSystemDefinition, tag => tag.IsPlayer,
                this._soldierStateBehavior.OnInteractiveObjectJustOnSight, null, this._soldierStateBehavior.OnInteractiveObjectJustOutOfSight);

            this.SightVisualFeedbackSystem = new SightVisualFeedbackSystem(SoliderEnemyDefinition.SightVisualFeedbackSystemDefinition, this, mainCamera);
            this.SightVisualFeedbackStateBehavior = new SightVisualFeedbackStateBehavior(this.SightVisualFeedbackSystem);

            this._soldierStateBehavior.Init(this, SoliderEnemyDefinition.SoldierAIBehaviorDefinition,
                new SoldierAIBehaviorExternalCallbacksV2()
                {
                    SetAIAgentDestinationAction = this.SetDestination,
                    SetAIAgentDestinationAction_NoReturn = (IAgentMovementCalculationStrategy => this.SetDestination(IAgentMovementCalculationStrategy)),
                    SetAIAgentSpeedAttenuationAction = this.SetAISpeedAttenuationFactor,
                    ClearAIAgentPathAction = this.AIMoveToDestinationSystem.ClearPath,
                    AskToFireAFiredprojectile_WithWorldDirection_Action = this.FireProjectileAction_Start,
                    GetWeaponFirePointOriginLocalDefinitionAction = () => SoliderEnemyDefinition.WeaponHandlingSystemDefinition.WeaponHandlingFirePointOriginLocalDefinition,
                    OnShootingAtPlayerStartAction = this.OnShootingAtPlayerStart,
                    OnShootingAtPlayerEndAction = this.OnShootingAtPlayerEnd,
                    GetIWeaponHandlingSystem_DataRetrievalAction = this.WeaponHandlingSystem,
                    OnMoveTowardsPlayerStartedAction = (CoreInteractiveObject MovingTowardsObject) => this.SightVisualFeedbackStateBehavior.SetSightVisualFeedbackState(SightVisualFeedbackState.DANGER),
                    OnMoveTowardsPlayerEndedAction = () => this.SightVisualFeedbackStateBehavior.SetSightVisualFeedbackState(SightVisualFeedbackState.NONE),
                    OnMoveAroundPlayerStartedAction = (Vector3 LockedWorldPosition) => this.SightVisualFeedbackStateBehavior.SetSightVisualFeedbackState(SightVisualFeedbackState.WARNING),
                    OnMoveAroundPlayerEndedAction = () => this.SightVisualFeedbackStateBehavior.SetSightVisualFeedbackState(SightVisualFeedbackState.NONE),
                    OnMoveToLastSeenPlayerPositionStartedAction = (Vector3 LockedWorldPosition) => this.SightVisualFeedbackStateBehavior.SetSightVisualFeedbackState(SightVisualFeedbackState.WARNING),
                    OnMoveToLastSeenPlayerPositionEndedAction = () => this.SightVisualFeedbackStateBehavior.SetSightVisualFeedbackState(SightVisualFeedbackState.NONE)
                });
        }

        public override void Tick(float d)
        {
            this.InteractiveObjectActionPlayerSystem.Tick(d);
            this._stunningDamageDealerReceiverSystem.Tick(d);
            if (!this._stunningDamageDealerReceiverSystem.IsStunned.GetValue())
            {
                this._soldierStateBehavior.Tick(d);
            }
        }

        public override void AfterTicks(float d)
        {
            this.InteractiveObjectActionPlayerSystem.AfterTicks(d);
            this.ObjectMovementSpeedSystem.AfterTicks(d);

            if (!this._stunningDamageDealerReceiverSystem.IsStunned.GetValue())
            {
                this.AIMoveToDestinationSystem.AfterTicks(d);
            }

            if (!this._stunningDamageDealerReceiverSystem.IsStunned.GetValue())
            {
                this.SoliderEnemyAnimationStateManager.SetUnscaledObjectLocalDirection(this.ObjectMovementSpeedSystem.GetLocalSpeedDirection_Attenuated());
            }

            SightVisualFeedbackSystem.AfterTicks(d);

            base.AfterTicks(d);
            this.SoliderEnemyAnimationStateManager.Tick(d);
            base.UpdateAniamtions(d);
        }

        public override void TickTimeFrozen(float d)
        {
            this.SightVisualFeedbackSystem.TickTimeFrozen(d);
            base.TickTimeFrozen(d);
        }

        public override void Destroy()
        {
            this.SightObjectSystem.OnDestroy();
            this._soldierStateBehavior.OnDestroy();
            this.WeaponHandlingSystem.Destroy();
            SightVisualFeedbackSystem.Destroy();
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

        private void OnShootingAtPlayerStart(CoreInteractiveObject TargettedInteractiveObject)
        {
            this.SoliderEnemyAnimationStateManager.StartTargetting();
            this.SightVisualFeedbackSystem.Show(SightVisualFeedbackColorType.DANGER);
        }

        private void OnShootingAtPlayerEnd()
        {
            this.SoliderEnemyAnimationStateManager.StopTargetting();
            this.SightVisualFeedbackSystem.Hide();
        }

        #endregion

        public override void Init()
        {
        }
    }

    public partial class SoliderEnemy : IEM_ProjectileFireActionInput_Retriever
    {
        private Vector3 LastTargetWorldDirection;

        private void FireProjectileAction_Start(Vector3 WorldDirection)
        {
            this.LastTargetWorldDirection = WorldDirection;
            this.InteractiveObjectActionPlayerSystem.ExecuteActionV2(this.WeaponHandlingSystem.GetCurrentWeaponProjectileFireActionDefinition());
        }

        public bool ProjectileFireActionEnabled()
        {
            return true;
        }

        public Vector3 GetCurrentTargetDirection()
        {
            return this.LastTargetWorldDirection;
        }

        public CoreInteractiveObject GetCurrentlyTargettedInteractiveObject()
        {
            return null;
        }
    }
}