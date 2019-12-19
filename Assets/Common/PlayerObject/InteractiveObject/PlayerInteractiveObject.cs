using AnimatorPlayable;
using Damage;
using Health;
using Input;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using LevelManagement;
using PlayerActions;
using PlayerLowHealth;
using PlayerLowHealth_Interfaces;
using PlayerObject_Interfaces;
using ProjectileDeflection;
using ProjectileDeflection_Interface;
using UnityEngine;
using UnityEngine.AI;
using Weapon;

namespace PlayerObject
{
    public class PlayerInteractiveObject : CoreInteractiveObject, IPlayerInteractiveObject
    {
        private PlayerInteractiveObjectDefinition PlayerInteractiveObjectDefinition;

        #region Systems

        private PlayerObjectAnimationStateManager PlayerObjectAnimationStateManager;
        private WeaponHandlingSystem WeaponHandlingSystem;
        private FiringTargetPositionSystem FiringTargetPositionSystem;
        private HealthSystem HealthSystem;
        private StunningDamageDealerReceiverSystem StunningDamageDealerReceiverSystem;
        private LowHealthPlayerSystem lowHealthPlayerSystem;

        public LowHealthPlayerSystem LowHealthPlayerSystem => this.lowHealthPlayerSystem;

        private ProjectileDeflectionSystem projectileDeflectionSystem;
        public ProjectileDeflectionSystem ProjectileDeflectionSystem => this.projectileDeflectionSystem;

        private PlayerVisualEffectSystem PlayerVisualEffectSystem;

        #endregion

        #region External Dependencies

        private GameInputManager GameInputManager;
        private PlayerActionEntryPoint PlayerActionEntryPoint = PlayerActionEntryPoint.Get();
        [VE_Nested] private BlockingCutscenePlayerManager BlockingCutscenePlayer = BlockingCutscenePlayerManager.Get();
        private LevelTransitionManager LevelTransitionManager = LevelTransitionManager.Get();

        #endregion

        [VE_Ignore] private PlayerBodyPhysicsEnvironment PlayerBodyPhysicsEnvironment;
        [VE_Nested] private ObjectMovementSpeedSystem ObjectMovementSpeedSystem;
        [VE_Ignore] private PlayerMoveManager playerMoveManager;
        public PlayerMoveManager PlayerMoveManager => this.playerMoveManager;
        [VE_Ignore] private PlayerSelectionWheelManager PlayerSelectionWheelManager;

        public PlayerInteractiveObject(IInteractiveGameObject interactiveGameObject, PlayerInteractiveObjectDefinition PlayerInteractiveObjectDefinition)
        {
            this.PlayerInteractiveObjectDefinition = PlayerInteractiveObjectDefinition;
            base.BaseInit(interactiveGameObject, false);
            this.WeaponHandlingSystem = new WeaponHandlingSystem(this, new WeaponHandlingSystemInitializationData(this, PlayerInteractiveObjectDefinition.WeaponHandlingSystemDefinition.WeaponHandlingFirePointOriginLocalDefinition, PlayerInteractiveObjectDefinition.WeaponHandlingSystemDefinition.WeaponDefinition));
            this.FiringTargetPositionSystem = new FiringTargetPositionSystem(PlayerInteractiveObjectDefinition.FiringTargetPositionSystemDefinition);
            this.HealthSystem = new HealthSystem(PlayerInteractiveObjectDefinition.HealthSystemDefinition, OnHealthValueChangedAction: this.OnHealthValueChanged);
            this.StunningDamageDealerReceiverSystem = new StunningDamageDealerReceiverSystem(PlayerInteractiveObjectDefinition.StunningDamageDealerReceiverSystemDefinition, this.HealthSystem);
            this.lowHealthPlayerSystem = new LowHealthPlayerSystem(this, this.HealthSystem, PlayerInteractiveObjectDefinition.LowHealthPlayerSystemDefinition);
            this.projectileDeflectionSystem = new ProjectileDeflectionSystem(this, PlayerInteractiveObjectDefinition.projectileDeflectionActorDefinition);
            this.PlayerVisualEffectSystem = new PlayerVisualEffectSystem(this, PlayerInteractiveObjectDefinition.PlayerVisualEffectSystemDefinition);

            /// To display the associated HealthSystem value to UI.
            HealthUIManager.Get().InitEvents(this.HealthSystem);
            OnProjectileDeflectionAttemptEvent.Get().RegisterOnProjectileDeflectionAttemptEventListener(this.OnProjectileDeflectionAttempt);
            
            PlayerStartTargettingEvent.Get().RegisterOnPlayerStartTargettingEvent(this.OnPlayerStartTargetting);
            PlayerStoppedTargettingEvent.Get().RegisterOnPlayerStoppedTargettingEvent(this.OnPlayerStoppedTargetting);
            
            PlayerLowHealthStartedEvent.Get().RegisterPlayerLowHealthStartedEvent(this.OnLowHealthStarted);
            PlayerLowHealthEndedEvent.Get().RegisterPlayerLowHealthEndedEvent(this.OnLowHealthEnded);
            
            PlayerInteractiveObjectCreatedEvent.Get().OnPlayerInteractiveObjectCreated(this);
        }

        public override void Init()
        {
            this.InteractiveGameObject.CreateLogicCollider(this.PlayerInteractiveObjectDefinition.InteractiveObjectLogicCollider);

            /// Agent creation is used to allow player movement without input.
            this.InteractiveGameObject.CreateAgent(this.PlayerInteractiveObjectDefinition.AIAgentDefinition);
            /// It is disabled by default.
            this.InteractiveGameObject.Agent.enabled = false;

            interactiveObjectTag = new InteractiveObjectTag {IsPlayer = true, IsTakingDamage = true};

            PlayerInteractiveObjectInitializerData = PlayerConfigurationGameObject.Get().PlayerGlobalConfiguration.PlayerInteractiveObjectInitializerData;

            #region External Dependencies

            this.GameInputManager = GameInputManager.Get();

            #endregion

            this.ObjectMovementSpeedSystem = new ObjectMovementSpeedSystem(this, PlayerInteractiveObjectInitializerData.TransformMoveManagerComponent, AIMovementSpeedAttenuationFactor.RUN,
                ObjectSpeedCalculationType.MANUAL);

            var cameraPivotPoint = GameObject.FindGameObjectWithTag(TagConstants.CAMERA_PIVOT_POINT_TAG);
            this.playerMoveManager = new PlayerMoveManager(this, this.ObjectMovementSpeedSystem,
                new PlayerRigidBodyMoveManager(this, PlayerInteractiveObjectInitializerData.TransformMoveManagerComponent, this.ObjectMovementSpeedSystem, cameraPivotPoint.transform),
                new PlayerAgentMoveManager(this, PlayerInteractiveObjectInitializerData.TransformMoveManagerComponent, this.ObjectMovementSpeedSystem, this.OnDestinationReached));

            PlayerBodyPhysicsEnvironment = new PlayerBodyPhysicsEnvironment(this.InteractiveGameObject.PhysicsRigidbody, this.InteractiveGameObject.PhysicsCollider, PlayerInteractiveObjectInitializerData.MinimumDistanceToStick);
            PlayerSelectionWheelManager = new PlayerSelectionWheelManager(this, this.GameInputManager,
                PlayerActionEntryPoint.Get());

            //Getting persisted position
            PlayerPositionPersistenceManager.Get().Init(this);
            this.InteractiveGameObject.InteractiveGameObjectParent.transform.position = PlayerPositionPersistenceManager.Get().PlayerPositionBeforeLevelLoad.GetPosition();
            this.InteractiveGameObject.InteractiveGameObjectParent.transform.rotation = PlayerPositionPersistenceManager.Get().PlayerPositionBeforeLevelLoad.GetQuaternion();

            this.PlayerObjectAnimationStateManager = new PlayerObjectAnimationStateManager(this.AnimationController, this.PlayerInteractiveObjectDefinition.BaseLocomotionAnimationDefinition);
        }

        public PlayerInteractiveObjectInitializerData PlayerInteractiveObjectInitializerData { get; private set; }

        public override void FixedTick(float d)
        {
            base.FixedTick(d);
            if (this.lowHealthPlayerSystem.IsHealthConsideredLow())
            {
                this.projectileDeflectionSystem.FixedTick(d);
            }

            PlayerActionTriggering();

            playerMoveManager.FixedTick(d);
            PlayerBodyPhysicsEnvironment.FixedTick(d);
        }

        public override void Tick(float d)
        {
            this.StunningDamageDealerReceiverSystem.Tick(d);
            if (this.lowHealthPlayerSystem.IsHealthConsideredLow())
            {
                this.projectileDeflectionSystem.Tick(d);
            }

            PlayerActionTriggering();
            UpdatePlayerMovement(d);
        }

        public override void TickTimeFrozen(float d)
        {
            if (this.lowHealthPlayerSystem.IsHealthConsideredLow())
            {
                this.projectileDeflectionSystem.TickTimeFrozen(d);
            }
        }

        public override void AfterTicks(float d)
        {
            this.ObjectMovementSpeedSystem.AfterTicks();
            this.playerMoveManager.AfterTicks();

            this.PlayerObjectAnimationStateManager.SetUnscaledObjectLocalDirection(this.ObjectMovementSpeedSystem.GetLocalSpeedDirectionAttenuated());
            base.UpdateAniamtions(d);
        }

        public override void LateTick(float d)
        {
            base.LateTick(d);
            this.PlayerVisualEffectSystem.LateTick(d);
            this.projectileDeflectionSystem.LateTick(d);
            this.playerMoveManager.LateTick(d);
        }

        /// <summary>
        /// Starts a new <see cref="PlayerAction"/> if input condition and player inherent conditions are met.
        /// </summary>
        private void PlayerActionTriggering()
        {
            if (!this.PlayerActionEntryPoint.IsActionExecuting() && !BlockingCutscenePlayer.Playing &&
                !this.PlayerActionEntryPoint.IsSelectionWheelEnabled() && !this.StunningDamageDealerReceiverSystem.IsStunned.GetValue())
            {
                if (this.GameInputManager.CurrentInput.FiringActionDown())
                {
                    this.PlayerActionEntryPoint.ExecuteAction(this.PlayerInteractiveObjectDefinition.FiringPlayerActionInherentData.BuildPlayerAction(this));
                }
            }
        }

        /// <summary>
        /// /!\ The update of player movements must be applied after every logic (that may apply <see cref="PlayerMovementConstraint"/>).
        /// </summary>
        private void UpdatePlayerMovement(float d)
        {
            if (BlockingCutscenePlayer.Playing || this.PlayerActionEntryPoint.IsSelectionWheelEnabled() || (this.PlayerActionEntryPoint.IsActionExecuting() && !this.PlayerActionEntryPoint.DoesCurrentActionAllowsMovement()))
            {
                playerMoveManager.ResetSpeed();
            }
            else
            {
                playerMoveManager.Tick(d);
            }
        }

        public override void Destroy()
        {
            this.WeaponHandlingSystem.Destroy();
            this.projectileDeflectionSystem.Destroy();
            PlayerInteractiveObjectDestroyedEvent.Get().OnPlayerInteractiveObjectDestroyed();
            PlayerStartTargettingEvent.Get().UnRegisterOnPlayerStartTargettingEvent(this.OnPlayerStartTargetting);
            PlayerStoppedTargettingEvent.Get().UnRegisterOnPlayerStoppedTargettingEvent(this.OnPlayerStoppedTargetting);
            
            PlayerLowHealthStartedEvent.Get().UnRegisterPlayerLowHealthStartedEvent(this.OnLowHealthStarted);
            PlayerLowHealthEndedEvent.Get().UnRegisterPlayerLowHealthEndedEvent(this.OnLowHealthEnded);
            
            OnProjectileDeflectionAttemptEvent.Get().UnRegisterOnProjectileDeflectionAttemptEvent(this.OnProjectileDeflectionAttempt);
            base.Destroy();
        }

        #region Agents events

        public override NavMeshPathStatus SetDestination(IAgentMovementCalculationStrategy IAgentMovementCalculationStrategy)
        {
            return this.playerMoveManager.SetDestination(IAgentMovementCalculationStrategy);
        }

        /// <summary>
        /// This event is called from the <see cref="PlayerMoveManager"/> when it is directed by <see cref="PlayerAgentMoveManager"/>.
        /// </summary>
        private void OnDestinationReached()
        {
            PlayerInteractiveObjectDestinationReachedEvent.Get().OnPlayerInteractiveObjectDestinationReached();
        }

        public override void SetAISpeedAttenuationFactor(AIMovementSpeedAttenuationFactor aiMovementSpeedAttenuationFactor)
        {
            this.ObjectMovementSpeedSystem.SetSpeedAttenuationFactor(aiMovementSpeedAttenuationFactor);
        }

        public override void ConstrainSpeed(IObjectSpeedAttenuationConstraint objectSpeedAttenuationConstraint)
        {
            this.ObjectMovementSpeedSystem.ConstrainSpeed(objectSpeedAttenuationConstraint);
        }

        public override void RemoveSpeedConstraints()
        {
            this.ObjectMovementSpeedSystem.RemoveSpeedConstraints();
        }

        public override AIMovementSpeedAttenuationFactor GetCurrentSpeedAttenuationFactor()
        {
            return this.ObjectMovementSpeedSystem.GetSpeedAttenuationFactor();
        }

        #endregion

        #region Health Events

        public override void DealDamage(float Damage, CoreInteractiveObject DamageDealerInteractiveObject)
        {
            this.StunningDamageDealerReceiverSystem.DealDamage(Damage);
        }

        private void OnHealthValueChanged(float oldValue, float newValue)
        {
            if (newValue <= 0)
            {
                this.LevelTransitionManager.RestartCurrentLevel();
            }
        }

        private void OnLowHealthStarted()
        {
            this.PlayerObjectAnimationStateManager.OnLowHealthStarted(this.PlayerInteractiveObjectDefinition.LowHealthPlayerSystemDefinition.OnLowHealthLocomotionAnimation);
            this.projectileDeflectionSystem.OnLowHealthStarted();
            this.PlayerVisualEffectSystem.OnLowHealthStarted();
        }

        private void OnLowHealthEnded()
        {
            this.PlayerObjectAnimationStateManager.OnLowHealthEnded();
            this.projectileDeflectionSystem.OnLowHealthEnded();
            this.PlayerVisualEffectSystem.OnLowHealthEnded();
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

        public override Vector3 GetWeaponWorldFirePoint()
        {
            return this.WeaponHandlingSystem.GetWorldWeaponFirePoint();
        }

        public override float GetFiredProjectileMaxRange()
        {
            return this.WeaponHandlingSystem.GetFiredProjectileMaxRange();
        }

        public override Vector3 GetFiringTargetLocalPosition()
        {
            return this.FiringTargetPositionSystem.GetFiringTargetLocalPosition();
        }

        #endregion

        #region Player Targetting Events

        public void OnPlayerStartTargetting(A_AnimationPlayableDefinition StartTargettingPoseAnimation)
        {
            this.PlayerObjectAnimationStateManager.StartTargetting(StartTargettingPoseAnimation);
        }

        public void OnPlayerStoppedTargetting()
        {
            this.PlayerObjectAnimationStateManager.EndTargetting();
        }

        public void SetConstraintForThisFrame(PlayerMovementConstraint PlayerMovementConstraint)
        {
            this.playerMoveManager.SetConstraintForThisFrame(PlayerMovementConstraint);
        }

        #endregion

        #region Deflection Events

        private void OnProjectileDeflectionAttempt()
        {
            this.PlayerObjectAnimationStateManager.OnProjectileDeflectionAttempt(this.PlayerInteractiveObjectDefinition.projectileDeflectionActorDefinition.ProjectileDeflectMovementAnimation);
        }

        #endregion
    }

    #region Player Action Managers

    internal class PlayerSelectionWheelManager
    {
        private PlayerInteractiveObject PlayerInteractiveObjectRef;
        private GameInputManager GameInputManager;

        #region External Dependencies

        private PlayerActionEntryPoint PlayerActionEntryPoint;

        #endregion

        public PlayerSelectionWheelManager(PlayerInteractiveObject PlayerInteractiveObject, GameInputManager gameInputManager,
            PlayerActionEntryPoint playerActionEntryPoint)
        {
            GameInputManager = gameInputManager;
            this.PlayerActionEntryPoint = playerActionEntryPoint;
            this.PlayerInteractiveObjectRef = PlayerInteractiveObject;
        }

        public bool AwakeOrSleepWheel()
        {
            if (!this.PlayerActionEntryPoint.IsSelectionWheelEnabled())
            {
                if (GameInputManager.CurrentInput.ActionButtonD())
                {
                    PlayerActionEntryPoint.AwakePlayerActionSelectionWheel(this.PlayerInteractiveObjectRef.InteractiveGameObject.InteractiveGameObjectParent.transform);
                    return true;
                }
            }
            else if (GameInputManager.CurrentInput.CancelButtonD())
            {
                this.PlayerActionEntryPoint.SleepPlayerActionSelectionWheel(false);
                return true;
            }

            return false;
        }

        public void TriggerActionOnInput()
        {
            if (GameInputManager.CurrentInput.ActionButtonD())
            {
                var selectedAction = this.PlayerActionEntryPoint.GetCurrentlySelectedPlayerAction();
                if (selectedAction.CanBeExecuted())
                {
                    this.PlayerActionEntryPoint.ExecuteAction(selectedAction);
                }
            }
        }
    }

    #endregion
}