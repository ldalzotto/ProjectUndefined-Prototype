using Input;
using InteractiveObject_Animation;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using PlayerActions;
using PlayerObject_Interfaces;
using UnityEngine;
using Weapon;

namespace PlayerObject
{
    public class PlayerInteractiveObject : CoreInteractiveObject, IPlayerInteractiveObject
    {
        private PlayerInteractiveObjectInitializer PlayerInteractiveObjectInitializer;

        #region Systems

        [VE_Nested] private BaseObjectAnimatorPlayableSystem baseObjectAnimatorPlayableSystem;
        private WeaponHandlingSystem WeaponHandlingSystem;

        #endregion

        #region External Dependencies

        private GameInputManager GameInputManager;

        #endregion

        [VE_Ignore] private PlayerBodyPhysicsEnvironment PlayerBodyPhysicsEnvironment;
        [VE_Ignore] private PlayerInputMoveManager PlayerInputMoveManager;
        [VE_Ignore] private PlayerSelectionWheelManager PlayerSelectionWheelManager;

        public PlayerInteractiveObject(IInteractiveGameObject interactiveGameObject, PlayerInteractiveObjectInitializer PlayerInteractiveObjectInitializer)
        {
            this.PlayerInteractiveObjectInitializer = PlayerInteractiveObjectInitializer;
            base.BaseInit(interactiveGameObject, false);
            this.WeaponHandlingSystem = new WeaponHandlingSystem(new WeaponHandlingSystemInitializationData(this, PlayerInteractiveObjectInitializer.WeaponHandlingSystemDefinition.WeaponFirePointOriginLocal, PlayerInteractiveObjectInitializer.WeaponHandlingSystemDefinition.WeaponInitializerPrefab));
        }

        public override void Init()
        {
            this.InteractiveGameObject.CreateLogicCollider(this.PlayerInteractiveObjectInitializer.InteractiveObjectLogicCollider);
            interactiveObjectTag = new InteractiveObjectTag {IsPlayer = true};

            PlayerInteractiveObjectInitializerData = PlayerConfigurationGameObject.Get().PlayerGlobalConfiguration.PlayerInteractiveObjectInitializerData;

            #region External Dependencies

            this.GameInputManager = GameInputManager.Get();

            #endregion

            var cameraPivotPoint = GameObject.FindGameObjectWithTag(TagConstants.CAMERA_PIVOT_POINT_TAG);

            PlayerInputMoveManager = new PlayerInputMoveManager(PlayerInteractiveObjectInitializerData.SpeedMultiplicationFactor, cameraPivotPoint.transform, this.GameInputManager, this.InteractiveGameObject.PhysicsRigidbody);
            PlayerBodyPhysicsEnvironment = new PlayerBodyPhysicsEnvironment(this.InteractiveGameObject.PhysicsRigidbody, this.InteractiveGameObject.PhysicsCollider, PlayerInteractiveObjectInitializerData.MinimumDistanceToStick);
            PlayerSelectionWheelManager = new PlayerSelectionWheelManager(this, this.GameInputManager,
                PlayerActionEntryPoint.Get());

            //Getting persisted position
            PlayerPositionPersistenceManager.Get().Init(this);
            this.InteractiveGameObject.InteractiveGameObjectParent.transform.position = PlayerPositionPersistenceManager.Get().PlayerPositionBeforeLevelLoad.GetPosition();
            this.InteractiveGameObject.InteractiveGameObjectParent.transform.rotation = PlayerPositionPersistenceManager.Get().PlayerPositionBeforeLevelLoad.GetQuaternion();

            this.baseObjectAnimatorPlayableSystem = new BaseObjectAnimatorPlayableSystem(this.AnimatorPlayable, this.PlayerInteractiveObjectInitializer.LocomotionAnimation);
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
                            this.PlayerActionEntryPoint.ExecuteAction(this.PlayerInteractiveObjectInitializer.FiringPlayerActionInherentData.BuildPlayerAction(this));
                        }
                        else
                        {
                            PlayerInputMoveManager.Tick(d);
                        }
                    }
                    else
                    {
                        PlayerInputMoveManager.ResetSpeed();
                        PlayerSelectionWheelManager.TriggerActionOnInput();
                    }
                }
            }
            else
            {
                PlayerInputMoveManager.ResetSpeed();
            }

            this.baseObjectAnimatorPlayableSystem.SetUnscaledObjectSpeed(GetNormalizedSpeed());
        }

        public override void FixedTick(float d)
        {
            base.FixedTick(d);
            PlayerInputMoveManager.FixedTick(d);
            PlayerBodyPhysicsEnvironment.FixedTick(d);
        }

        public override void Destroy()
        {
            base.Destroy();
        }

        #region External Dependencies

        private PlayerActionEntryPoint PlayerActionEntryPoint = PlayerActionEntryPoint.Get();
        [VE_Nested] private BlockingCutscenePlayerManager BlockingCutscenePlayer = BlockingCutscenePlayerManager.Get();

        #endregion

        #region Logical Conditions

        public float GetNormalizedSpeed()
        {
            return PlayerInputMoveManager.PlayerSpeedMagnitude;
        }

        #endregion


        #region Projectile Events

        public override void AskToFireAFiredProjectile()
        {
            this.WeaponHandlingSystem.AskToFireAFiredProjectile();
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