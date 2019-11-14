using Input;
using InteractiveObject_Animation;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using PlayerActions;
using PlayerObject_Interfaces;
using UnityEngine;
using UnityEngine.AI;
using Weapon;

namespace PlayerObject
{
    public class PlayerInteractiveObject : CoreInteractiveObject, IPlayerInteractiveObject
    {
        private PlayerInteractiveObjectDefinition PlayerInteractiveObjectDefinition;

        #region Systems

        [VE_Nested] private BaseObjectAnimatorPlayableSystem baseObjectAnimatorPlayableSystem;
        private WeaponHandlingSystem WeaponHandlingSystem;
        private FiringTargetPositionSystem FiringTargetPositionSystem;

        #endregion

        #region External Dependencies

        private GameInputManager GameInputManager;
        private PlayerActionEntryPoint PlayerActionEntryPoint = PlayerActionEntryPoint.Get();
        [VE_Nested] private BlockingCutscenePlayerManager BlockingCutscenePlayer = BlockingCutscenePlayerManager.Get();

        #endregion

        [VE_Ignore] private PlayerBodyPhysicsEnvironment PlayerBodyPhysicsEnvironment;
        [VE_Ignore] private PlayerMoveManager playerMoveManager;
        public PlayerMoveManager PlayerMoveManager => this.playerMoveManager;
        [VE_Ignore] private PlayerSelectionWheelManager PlayerSelectionWheelManager;

        public PlayerInteractiveObject(IInteractiveGameObject interactiveGameObject, PlayerInteractiveObjectDefinition PlayerInteractiveObjectDefinition)
        {
            this.PlayerInteractiveObjectDefinition = PlayerInteractiveObjectDefinition;
            base.BaseInit(interactiveGameObject, false);
            this.WeaponHandlingSystem = new WeaponHandlingSystem(this, new WeaponHandlingSystemInitializationData(this, PlayerInteractiveObjectDefinition.WeaponHandlingSystemDefinition.WeaponHandlingFirePointOriginLocalDefinition, PlayerInteractiveObjectDefinition.WeaponHandlingSystemDefinition.WeaponDefinition));
            this.FiringTargetPositionSystem = new FiringTargetPositionSystem(PlayerInteractiveObjectDefinition.FiringTargetPositionSystemDefinition);
          
            PlayerInteractiveObjectCreatedEvent.Get().OnPlayerInteractiveObjectCreated(this);
        }

        public override void Init()
        {
            this.InteractiveGameObject.CreateLogicCollider(this.PlayerInteractiveObjectDefinition.InteractiveObjectLogicCollider);

            /// Agent creation is used to allow player movement without input.
            this.InteractiveGameObject.CreateAgent(this.PlayerInteractiveObjectDefinition.AIAgentDefinition);
            /// It is disabled by default.
            this.InteractiveGameObject.Agent.enabled = false;

            interactiveObjectTag = new InteractiveObjectTag {IsPlayer = true};

            PlayerInteractiveObjectInitializerData = PlayerConfigurationGameObject.Get().PlayerGlobalConfiguration.PlayerInteractiveObjectInitializerData;

            #region External Dependencies

            this.GameInputManager = GameInputManager.Get();

            #endregion

            var cameraPivotPoint = GameObject.FindGameObjectWithTag(TagConstants.CAMERA_PIVOT_POINT_TAG);

            this.playerMoveManager = new PlayerMoveManager(this,
                new PlayerRigidBodyMoveManager(PlayerInteractiveObjectInitializerData.TransformMoveManagerComponent.SpeedMultiplicationFactor, this.InteractiveGameObject.PhysicsRigidbody, cameraPivotPoint.transform),
                new PlayerAgentMoveManager(this, PlayerInteractiveObjectInitializerData.TransformMoveManagerComponent, this.OnDestinationReached));
            PlayerBodyPhysicsEnvironment = new PlayerBodyPhysicsEnvironment(this.InteractiveGameObject.PhysicsRigidbody, this.InteractiveGameObject.PhysicsCollider, PlayerInteractiveObjectInitializerData.MinimumDistanceToStick);
            PlayerSelectionWheelManager = new PlayerSelectionWheelManager(this, this.GameInputManager,
                PlayerActionEntryPoint.Get());

            //Getting persisted position
            PlayerPositionPersistenceManager.Get().Init(this);
            this.InteractiveGameObject.InteractiveGameObjectParent.transform.position = PlayerPositionPersistenceManager.Get().PlayerPositionBeforeLevelLoad.GetPosition();
            this.InteractiveGameObject.InteractiveGameObjectParent.transform.rotation = PlayerPositionPersistenceManager.Get().PlayerPositionBeforeLevelLoad.GetQuaternion();

            this.baseObjectAnimatorPlayableSystem = new BaseObjectAnimatorPlayableSystem(this.AnimatorPlayable, this.PlayerInteractiveObjectDefinition.LocomotionAnimation);
        }

        public PlayerInteractiveObjectInitializerData PlayerInteractiveObjectInitializerData { get; private set; }

        public override void Tick(float d)
        {
            base.Tick(d);
            if (!this.PlayerActionEntryPoint.IsActionExecuting() && !BlockingCutscenePlayer.Playing)
            {
                if (!PlayerSelectionWheelManager.AwakeOrSleepWheel())
                {
                    if (!this.PlayerActionEntryPoint.IsSelectionWheelEnabled())
                    {
                        if (this.GameInputManager.CurrentInput.FiringActionDown())
                        {
                            this.PlayerActionEntryPoint.ExecuteAction(this.PlayerInteractiveObjectDefinition.FiringPlayerActionInherentData.BuildPlayerAction(this));
                        }
                        else
                        {
                            playerMoveManager.Tick(d);
                        }
                    }
                    else
                    {
                        playerMoveManager.ResetSpeed();
                        PlayerSelectionWheelManager.TriggerActionOnInput();
                    }
                }
            }
            else
            {
                playerMoveManager.ResetSpeed();
            }

            this.baseObjectAnimatorPlayableSystem.SetUnscaledObjectLocalDirection(Vector3.forward * playerMoveManager.GetPlayerSpeedMagnitude());
        }

        public override void AfterTicks(float d)
        {
            this.playerMoveManager.AfterTicks();
        }

        public override void FixedTick(float d)
        {
            base.FixedTick(d);
            playerMoveManager.FixedTick(d);
            PlayerBodyPhysicsEnvironment.FixedTick(d);
        }

        public override void Destroy()
        {
            PlayerInteractiveObjectDestroyedEvent.Get().OnPlayerInteractiveObjectDestroyed();
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

        public override void SetAISpeedAttenuationFactor(AIMovementSpeedDefinition AIMovementSpeedDefinition)
        {
        }

        #endregion

        #region Projectile Events

        public override void AskToFireAFiredProjectile_Forward()
        {
            this.WeaponHandlingSystem.AskToFireAFiredProjectile_Forward();
        }

        public override void AskToFireAFiredProjectile_WithDirection(Vector3 WorldTargetDirection)
        {
            this.WeaponHandlingSystem.AskToFireAFiredProjectile_WithDirections(WorldTargetDirection);
        }
        
        public override void AskToFireAFiredProjectile_ToTargetPoint(Vector3 WorldDestination)
        {
            this.WeaponHandlingSystem.AskToFireAFiredProjectile_ToTargetPoint(WorldDestination);
        }

        public override Vector3 GetFiringTargetLocalPosition()
        {
            return this.FiringTargetPositionSystem.GetFiringTargetLocalPosition();
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