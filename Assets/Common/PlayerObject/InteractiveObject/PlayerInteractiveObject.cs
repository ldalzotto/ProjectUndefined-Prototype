using System;
using Damage;
using PlayerAim;
using Health;
using Input;
using InteractiveObjectAction;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using LevelManagement;
using PlayerDash;
using PlayerLowHealth;
using PlayerObject_Interfaces;
using ProjectileDeflection;
using Skill;
using UnityEngine;
using UnityEngine.AI;
using Weapon;

namespace PlayerObject
{
    public partial class PlayerInteractiveObject : CoreInteractiveObject, IPlayerInteractiveObject,
        IEM_PlayerLowHealthInteractiveObjectExposedMethods, IEM_InteractiveObjectActionPlayerSystem_Retriever, IEM_WeaponHandlingSystem_Retriever
    {
        private PlayerInteractiveObjectDefinition PlayerInteractiveObjectDefinition;

        #region Systems

        public InteractiveObjectActionPlayerSystem InteractiveObjectActionPlayerSystem { get; private set; }
        private PlayerObjectAnimationStateManager PlayerObjectAnimationStateManager;
        public WeaponHandlingSystem WeaponHandlingSystem { get; private set; }
        private FiringTargetPositionSystem FiringTargetPositionSystem;
        private HealthSystem HealthSystem;
        private StunningDamageDealerReceiverSystem StunningDamageDealerReceiverSystem;
        private LowHealthPlayerSystem lowHealthPlayerSystem;
        public PlayerObjectInteractiveObjectActionStateManager PlayerObjectInteractiveObjectActionStateManager;

        public LowHealthPlayerSystem LowHealthPlayerSystem => this.lowHealthPlayerSystem;

        private PlayerVisualEffectSystem PlayerVisualEffectSystem;

        #endregion

        private SkillSystem SkillSystem;

        #region External Dependencies

        private GameInputManager GameInputManager;
        [VE_Nested] private BlockingCutscenePlayerManager BlockingCutscenePlayer = BlockingCutscenePlayerManager.Get();
        private LevelTransitionManager LevelTransitionManager = LevelTransitionManager.Get();

        #endregion

        [VE_Ignore] private PlayerBodyPhysicsEnvironment PlayerBodyPhysicsEnvironment;
        [VE_Ignore] private PlayerSpeedAttenuationSystem PlayerSpeedAttenuationSystem;
        [VE_Nested] private ObjectMovementSpeedSystem ObjectMovementSpeedSystem;
        [VE_Ignore] private PlayerMoveManager playerMoveManager;
        public PlayerMoveManager PlayerMoveManager => this.playerMoveManager;

        public PlayerInteractiveObject(IInteractiveGameObject interactiveGameObject, PlayerInteractiveObjectDefinition PlayerInteractiveObjectDefinition)
        {
            this.PlayerInteractiveObjectDefinition = PlayerInteractiveObjectDefinition;
            base.BaseInit(interactiveGameObject, false);
            this.InteractiveObjectActionPlayerSystem = new InteractiveObjectActionPlayerSystem(this);
            this.WeaponHandlingSystem = new WeaponHandlingSystem(
                this,
                new WeaponHandlingSystemInitializationData(this, PlayerInteractiveObjectDefinition.WeaponHandlingSystemDefinition.WeaponHandlingFirePointOriginLocalDefinition, PlayerInteractiveObjectDefinition.WeaponHandlingSystemDefinition.WeaponDefinition));
            this.FiringTargetPositionSystem = new FiringTargetPositionSystem(PlayerInteractiveObjectDefinition.FiringTargetPositionSystemDefinition);
            this.HealthSystem = new HealthSystem(this, PlayerInteractiveObjectDefinition.HealthSystemDefinition, OnHealthValueChangedAction: this.OnHealthValueChanged);
            this.StunningDamageDealerReceiverSystem = new StunningDamageDealerReceiverSystem(PlayerInteractiveObjectDefinition.StunningDamageDealerReceiverSystemDefinition, this.HealthSystem);
            this.lowHealthPlayerSystem = new LowHealthPlayerSystem(this.HealthSystem, PlayerInteractiveObjectDefinition.LowHealthPlayerSystemDefinition);
            this.PlayerVisualEffectSystem = new PlayerVisualEffectSystem(this, PlayerInteractiveObjectDefinition.PlayerVisualEffectSystemDefinition);

            this.SkillSystem = new SkillSystem(this, this.InteractiveObjectActionPlayerSystem);
            this.SkillSystem.SetPlayerActionToMainWeaponSkill(this.WeaponHandlingSystem.GetCurrentWeaponProjectileFireActionDefinition());
            this.SkillSystem.SetPlayerActionToSubSkill(PlayerInteractiveObjectDefinition.DeflectingProjectileInteractiveObjectActionInherentData, 0);
            this.SkillSystem.SetPlayerActionToSubSkill(PlayerInteractiveObjectDefinition.PlayerDashTeleportationActionDefinition, 1);

            this.PlayerObjectInteractiveObjectActionStateManager =
            new PlayerObjectInteractiveObjectActionStateManager(this.GameInputManager, this.InteractiveObjectActionPlayerSystem,
                this.SkillSystem,
                PlayerInteractiveObjectDefinition.firingInteractiveObjectActionInherentData, PlayerInteractiveObjectDefinition.projectileDeflectionTrackingInteractiveObjectActionInherentData,
                PlayerInteractiveObjectDefinition.PlayerDashActionStateBehaviorInputDataSystemDefinition);

            /// To display the associated HealthSystem value to UI.
            HealthUIManager.Get().InitEvents(this.HealthSystem);

            this.FiringPartialDefinitionInitialize();

            this.lowHealthPlayerSystem.RegisterPlayerLowHealthStartedEvent(this.OnLowHealthStarted);
            this.lowHealthPlayerSystem.RegisterPlayerLowHealthEndedEvent(this.OnLowHealthEnded);

            PlayerInteractiveObjectCreatedEvent.Get().OnPlayerInteractiveObjectCreated(this);
        }

        public override void Init()
        {
            this.InteractiveGameObject.CreateLogicCollider(this.PlayerInteractiveObjectDefinition.InteractiveObjectLogicCollider);

            /// Agent creation is used to allow player movement without input.
            this.InteractiveGameObject.CreateAgent(this.PlayerInteractiveObjectDefinition.AIAgentDefinition);
            /// It is disabled by default.
            this.InteractiveGameObject.Agent.enabled = false;

            interactiveObjectTag = new InteractiveObjectTag { IsPlayer = true, IsTakingDamage = true };

            PlayerInteractiveObjectInitializerData = PlayerConfigurationGameObject.Get().PlayerGlobalConfiguration.PlayerInteractiveObjectInitializerData;

            #region External Dependencies

            this.GameInputManager = GameInputManager.Get();

            #endregion

            this.PlayerSpeedAttenuationSystem = new PlayerSpeedAttenuationSystem();
            this.ObjectMovementSpeedSystem = new ObjectMovementSpeedSystem(this, PlayerInteractiveObjectInitializerData.TransformMoveManagerComponent, this.PlayerSpeedAttenuationSystem,
                ObjectSpeedCalculationType.MANUAL);

            var cameraPivotPoint = GameObject.FindGameObjectWithTag(TagConstants.CAMERA_PIVOT_POINT_TAG);
            this.playerMoveManager = new PlayerMoveManager(this, this.ObjectMovementSpeedSystem,
                new PlayerRigidBodyMoveManager(this, PlayerInteractiveObjectInitializerData.TransformMoveManagerComponent, this.ObjectMovementSpeedSystem, cameraPivotPoint.transform),
                new PlayerAgentMoveManager(this, PlayerInteractiveObjectInitializerData.TransformMoveManagerComponent, this.OnDestinationReached));

            PlayerBodyPhysicsEnvironment = new PlayerBodyPhysicsEnvironment(this.InteractiveGameObject.PhysicsRigidbody, this.InteractiveGameObject.PhysicsCollider, PlayerInteractiveObjectInitializerData.MinimumDistanceToStick);

            //Getting persisted position
            PlayerPositionPersistenceManager.Get().Init(this);
            this.InteractiveGameObject.InteractiveGameObjectParent.transform.position = PlayerPositionPersistenceManager.Get().PlayerPositionBeforeLevelLoad.GetPosition();
            this.InteractiveGameObject.InteractiveGameObjectParent.transform.rotation = PlayerPositionPersistenceManager.Get().PlayerPositionBeforeLevelLoad.GetQuaternion();

            this.PlayerObjectAnimationStateManager = new PlayerObjectAnimationStateManager(this.AnimationController, this.PlayerInteractiveObjectDefinition.BaseLocomotionAnimationDefinition);
        }

        public PlayerInteractiveObjectInitializerData PlayerInteractiveObjectInitializerData { get; private set; }

        public override void FixedTick(float d)
        {
            this.InteractiveObjectActionPlayerSystem.FixedTick(d);

            base.FixedTick(d);

            playerMoveManager.FixedTick(d);
            PlayerBodyPhysicsEnvironment.FixedTick(d);
        }

        /// <summary>
        /// When the time is frozen we don't want to update player rigidbody position and rotation.
        /// </summary>
        public override void FixedTickTimeFrozen(float d)
        {
            this.InteractiveObjectActionPlayerSystem.FixedTickTimeFrozen(d);
        }

        public override void Tick(float d)
        {
            this.InteractiveObjectActionPlayerSystem.Tick(d);
            this.SkillSystem.Tick(d);

            this.StunningDamageDealerReceiverSystem.Tick(d);

            this.PlayerObjectInteractiveObjectActionStateManager.Tick(d);

            UpdatePlayerMovement(d);
        }

        public override void AfterTicks(float d)
        {
            this.InteractiveObjectActionPlayerSystem.AfterTicks(d);
            this.ObjectMovementSpeedSystem.AfterTicks(d);
            this.playerMoveManager.AfterTicks(d);

            this.PlayerObjectAnimationStateManager.SetUnscaledObjectLocalDirection(this.ObjectMovementSpeedSystem.GetLocalSpeedDirection_Attenuated());
            base.UpdateAniamtions(d);
        }

        public override void TickTimeFrozen(float d)
        {
            this.PlayerObjectInteractiveObjectActionStateManager.TickTimeFrozen(d);
            this.InteractiveObjectActionPlayerSystem.TickTimeFrozen(d);
            this.SkillSystem.TickTimeFrozen(d);
        }


        public override void LateTick(float d)
        {
            base.LateTick(d);
            this.InteractiveObjectActionPlayerSystem.LateTick(d);
            this.PlayerVisualEffectSystem.LateTick(d);
            this.playerMoveManager.LateTick(d);
        }

        /// <summary>
        /// /!\ The update of player movements must be applied after every logic (that may apply <see cref="PlayerMovementConstraint"/>).
        /// </summary>
        private void UpdatePlayerMovement(float d)
        {
            if (BlockingCutscenePlayer.Playing || (!this.InteractiveObjectActionPlayerSystem.DoesCurrentActionAllowsMovement()))
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
            PlayerInteractiveObjectDestroyedEvent.Get().OnPlayerInteractiveObjectDestroyed();

            this.FiringPartialDefinitionDestroy();

            this.lowHealthPlayerSystem.UnRegisterPlayerLowHealthStartedEvent(this.OnLowHealthStarted);
            this.lowHealthPlayerSystem.UnRegisterPlayerLowHealthEndedEvent(this.OnLowHealthEnded);

            this.SkillSystem.Destroy();

            base.Destroy();
        }

        #region Speed Data Retrieval

        public override Vector3 GetWorldSpeed_Scaled_Attenuated()
        {
            return this.ObjectMovementSpeedSystem.GetEffectiveVelocity_Scaled_Attenuated();
        }

        #endregion

        #region Agents events

#if UNITY_EDITOR
        public NavMeshPathStatus SetDestination(IAgentMovementCalculationStrategy IAgentMovementCalculationStrategy)
        {
            return this.playerMoveManager.SetDestination(IAgentMovementCalculationStrategy);
        }


        public void SetAISpeedAttenuationFactor(AIMovementSpeedAttenuationFactor aiMovementSpeedAttenuationFactor)
        {
            this.ObjectMovementSpeedSystem.SetSpeedAttenuationFactor(aiMovementSpeedAttenuationFactor);
        }
#endif

        /// <summary>
        /// This event is called from the <see cref="PlayerMoveManager"/> when it is directed by <see cref="PlayerAgentMoveManager"/>.
        /// </summary>
        private void OnDestinationReached()
        {
            PlayerInteractiveObjectDestinationReachedEvent.Get().OnPlayerInteractiveObjectDestinationReached();
        }

        #endregion

        #region Health Events

        public override void DealDamage(float Damage, CoreInteractiveObject DamageDealerInteractiveObject)
        {
            this.StunningDamageDealerReceiverSystem.DealDamage(Damage);
        }

        public override void OnRecoverHealth(float recoveredHealthAmount)
        {
            this.HealthSystem.ChangeHealth(recoveredHealthAmount);
        }

        private void OnHealthValueChanged(float oldValue, float newValue)
        {
            if (newValue <= 0)
            {
                this.LevelTransitionManager.RestartCurrentLevel();
            }
        }

        #endregion

        #region Low Health Events

        private void OnLowHealthStarted()
        {
            this.PlayerSpeedAttenuationSystem.OnLowHealthStarted();
            this.PlayerObjectAnimationStateManager.OnLowHealthStarted(this.PlayerInteractiveObjectDefinition.LowHealthPlayerSystemDefinition.OnLowHealthLocomotionAnimation);
            this.PlayerObjectInteractiveObjectActionStateManager.OnLowOnHealthStarted();
            this.PlayerVisualEffectSystem.OnLowHealthStarted();
        }

        private void OnLowHealthEnded()
        {
            this.PlayerSpeedAttenuationSystem.OnLowHealthEnded();
            this.PlayerObjectAnimationStateManager.OnLowHealthEnded();
            this.PlayerObjectInteractiveObjectActionStateManager.OnLowOnHealthEnded();
            this.PlayerVisualEffectSystem.OnLowHealthEnded();
        }


        public void RegisterPlayerLowHealthStartedEvent(Action action)
        {
            this.lowHealthPlayerSystem.RegisterPlayerLowHealthStartedEvent(action);
        }

        public void UnRegisterPlayerLowHealthStartedEvent(Action action)
        {
            this.lowHealthPlayerSystem.UnRegisterPlayerLowHealthStartedEvent(action);
        }

        public void RegisterPlayerLowHealthEndedEvent(Action action)
        {
            this.lowHealthPlayerSystem.RegisterPlayerLowHealthEndedEvent(action);
        }

        public void UnRegisterPlayerLowHealthEndedEvent(Action action)
        {
            this.lowHealthPlayerSystem.UnRegisterPlayerLowHealthEndedEvent(action);
        }

        #endregion

        #region Projectile Events

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

        public void SetConstraintForThisFrame(PlayerMovementConstraint PlayerMovementConstraint)
        {
            this.playerMoveManager.SetConstraintForThisFrame(PlayerMovementConstraint);
        }
    }

    public partial class PlayerInteractiveObject : IEM_IFiringAInteractiveObjectAction_EventsListener, IEM_ProjectileFireActionInput_Retriever, IEM_IPlayerAimingFiringRegisteringEventsExposedMethod
    {
        public bool ProjectileFireActionEnabled()
        {
            return this.PlayerObjectInteractiveObjectActionStateManager.IsAiming();
        }

        public Vector3 GetCurrentTargetDirection()
        {
            if (InteractiveObjectActionPlayerSystem.GetPlayingPlayerActionReference(PlayerAimingInteractiveObjectAction.PlayerAimingInteractiveObjectActionUniqueID) is PlayerAimingInteractiveObjectAction firingPlayerActionReference)
            {
                return firingPlayerActionReference.GetCurrentTargetDirection();
            }

            return default;
        }

#if UNITY_EDITOR
        public CoreInteractiveObject GetCurrentlyTargettedInteractiveObject()
        {
            if (InteractiveObjectActionPlayerSystem.GetPlayingPlayerActionReference(PlayerAimingInteractiveObjectAction.PlayerAimingInteractiveObjectActionUniqueID) is PlayerAimingInteractiveObjectAction firingPlayerActionReference)
            {
                return firingPlayerActionReference.GetCurrentlyTargettedInteractiveObject();
            }

            return null;
        }
#endif

        private void FiringPartialDefinitionInitialize()
        {
            this.RegisterOnPlayerStartTargettingEvent(this.OnPlayerAimingActionStarted);
            this.RegisterOnPlayerStoppedTargettingEvent(this.OnPlayerAimingActionEnded);
        }

        private void FiringPartialDefinitionDestroy()
        {
            this.UnRegisterOnPlayerStartTargettingEvent(this.OnPlayerAimingActionStarted);
            this.UnRegisterOnPlayerStoppedTargettingEvent(this.OnPlayerAimingActionEnded);
        }

        #region Player Targetting Events

        private void OnPlayerAimingActionStarted(PlayerAimingInteractiveObjectActionInherentData playerAimingPlayerActionInherentData)
        {
            this.PlayerSpeedAttenuationSystem.StartAiming();
            this.PlayerObjectAnimationStateManager.StartAiming(playerAimingPlayerActionInherentData.FiringPoseAnimationV2);
        }

        private void OnPlayerAimingActionEnded(PlayerAimingInteractiveObjectActionInherentData playerAimingPlayerActionInherentData)
        {
            this.PlayerSpeedAttenuationSystem.StopTargetting();
            this.PlayerObjectAnimationStateManager.EndTargetting();
            this.PlayerObjectInteractiveObjectActionStateManager.StopTargetting();
        }

        #endregion


        private event Action<PlayerAimingInteractiveObjectActionInherentData> FiringAInteractiveObjectActionStartedEvent;
        private event Action<PlayerAimingInteractiveObjectActionInherentData> FiringAInteractiveObjectActionEndedEvent;

        public void RegisterOnPlayerStartTargettingEvent(Action<PlayerAimingInteractiveObjectActionInherentData> action)
        {
            this.FiringAInteractiveObjectActionStartedEvent += action;
        }

        public void UnRegisterOnPlayerStartTargettingEvent(Action<PlayerAimingInteractiveObjectActionInherentData> action)
        {
            this.FiringAInteractiveObjectActionStartedEvent -= action;
        }

        public void RegisterOnPlayerStoppedTargettingEvent(Action<PlayerAimingInteractiveObjectActionInherentData> action)
        {
            this.FiringAInteractiveObjectActionEndedEvent += action;
        }

        public void UnRegisterOnPlayerStoppedTargettingEvent(Action<PlayerAimingInteractiveObjectActionInherentData> action)
        {
            this.FiringAInteractiveObjectActionEndedEvent -= action;
        }

        public void OnFiringInteractiveObjectActionStart(PlayerAimingInteractiveObjectActionInherentData playerAimingInteractiveObjectActionInherentData)
        {
            this.FiringAInteractiveObjectActionStartedEvent?.Invoke(playerAimingInteractiveObjectActionInherentData);
        }

        public void OnFiringInteractiveObjectActionEnd(PlayerAimingInteractiveObjectActionInherentData playerAimingInteractiveObjectActionInherentData)
        {
            this.FiringAInteractiveObjectActionEndedEvent?.Invoke(playerAimingInteractiveObjectActionInherentData);
        }
    }

    public partial class PlayerInteractiveObject : IEM_DeflectingProjectileAction_DataRetriever, IEM_DeflectingProjectileAction_WorkflowEventListener
    {
        #region Deflection Events

        public void OnDeflectingProjectileInteractiveObjectActionExecuted(DeflectingProjectileInteractiveObjectActionInherentData DeflectingProjectileInteractiveObjectActionInherentData)
        {
            this.PlayerObjectAnimationStateManager.OnProjectileDeflectionAttempt(DeflectingProjectileInteractiveObjectActionInherentData.ProjectileDeflectMovementAnimation);
        }

        public bool ProjectileDeflectionEnabled()
        {
            return this.PlayerObjectInteractiveObjectActionStateManager.IsTrackingNearDeflectableObjects();
        }

        #endregion

        public ProjectileDeflectionTrackingInteractiveObjectAction GetPlayingProjectileDeflectionSystem()
        {
            return this.InteractiveObjectActionPlayerSystem.GetPlayingPlayerActionReference(ProjectileDeflectionTrackingInteractiveObjectAction.ProjectileDeflectionSystemUniqueID) as ProjectileDeflectionTrackingInteractiveObjectAction;
        }
    }

    public partial class PlayerInteractiveObject : IEM_PlayerDashAction, IEM_DashTeleportationDirectionAction_DataRetriever
    {
        public bool TryingToExecuteDashTeleportationAction()
        {
            return this.PlayerObjectInteractiveObjectActionStateManager.TryingToExecuteDashTeleportationAction();
        }

        public Vector3? GetTargetWorldPosition()
        {
            return this.PlayerObjectInteractiveObjectActionStateManager.GetPlayerDash_TargetPointWorldPosition();
        }
    }

    public partial class PlayerInteractiveObject : IEM_SkillSystem_ExposedMethods
    {
        public InputID GetInputIdAssociatedToTheInteractiveObjectAction(string actionUniqueID)
        {
            return this.SkillSystem.GetInputIdAssociatedToTheInteractiveObjectAction(actionUniqueID);
        }
    }
}