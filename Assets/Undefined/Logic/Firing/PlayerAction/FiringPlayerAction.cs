using AnimatorPlayable;
using Input;
using InteractiveObject_Animation;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using PlayerActions;
using PlayerObject_Interfaces;
using Targetting;
using UnityEngine;
using UnityEngine.Profiling;

namespace Firing
{
    public class FiringPlayerAction : PlayerAction
    {
        private FiringPlayerActionInherentData FiringPlayerActionInherentData;

        private PlayerObjectOrientationSystem PlayerObjectOrientationSystem;
        private FiringProjectileTriggerSystem FiringProjectileTriggerSystem;
        private ExitActionSystem ExitActionSystem;
        private PlayerAnimationSystem PlayerAnimationSystem;
        private PlayerSpeedSystem PlayerSpeedSystem;
        private FiringRangeFeedbackSystem _firingRangeFeedbackSystem;

        public FiringPlayerAction(FiringPlayerActionInherentData FiringPlayerActionInherentData, IPlayerInteractiveObject PlayerInteractiveObject) : base(FiringPlayerActionInherentData.CorePlayerActionDefinition)
        {
            var PlayerCoreInteractiveObject = PlayerInteractiveObject as CoreInteractiveObject;
            var targettableInteractiveObjectSelectionManager = TargettableInteractiveObjectSelectionManager.Get();
            var gameInputManager = GameInputManager.Get();
            this.FiringPlayerActionInherentData = FiringPlayerActionInherentData;
            this.PlayerObjectOrientationSystem = new PlayerObjectOrientationSystem(this.FiringPlayerActionInherentData, PlayerInteractiveObject, TargetCursorManager.Get(), targettableInteractiveObjectSelectionManager);
            this.FiringProjectileTriggerSystem = new FiringProjectileTriggerSystem(gameInputManager, PlayerCoreInteractiveObject, targettableInteractiveObjectSelectionManager);
            this.ExitActionSystem = new ExitActionSystem(gameInputManager);
            this.PlayerAnimationSystem = new PlayerAnimationSystem(PlayerCoreInteractiveObject, FiringPlayerActionInherentData.FiringPoseAnimationV2.GetAnimationInput());
            this.PlayerSpeedSystem = new PlayerSpeedSystem(PlayerCoreInteractiveObject);
            this._firingRangeFeedbackSystem = new FiringRangeFeedbackSystem(PlayerCoreInteractiveObject, targettableInteractiveObjectSelectionManager);
        }

        public override void FirstExecution()
        {
            base.FirstExecution();
        }

        public override bool FinishedCondition()
        {
            return this.ExitActionSystem.ActionFinished || base.FinishedCondition();
        }

        public override void Tick(float d)
        {
            Profiler.BeginSample("FiringPlayerAction");
            this.ExitActionSystem.Tick(d);
            if (!this.ExitActionSystem.ActionFinished)
            {
                this.PlayerObjectOrientationSystem.Tick(d);
                this.FiringProjectileTriggerSystem.Tick(d);
                this._firingRangeFeedbackSystem.Tick(d);
            }

            Profiler.EndSample();
        }

        public override void Dispose()
        {
            this.PlayerObjectOrientationSystem.Dispose();
            this.PlayerAnimationSystem.Dispose();
            this.PlayerSpeedSystem.Dispose();
            this._firingRangeFeedbackSystem.Dispose();
        }

        public override void LateTick(float d)
        {
        }

        public override void GUITick()
        {
        }

        public override void GizmoTick()
        {
        }
    }

    struct PlayerObjectOrientationSystem
    {
        private IPlayerInteractiveObject PlayerInteractiveObjectRef;
        private GameObject HorizontalPlaneGameObject;
        private TargetCursorManager _targetCursorManagerRef;
        private TargettableInteractiveObjectSelectionManager TargettableInteractiveObjectSelectionManager;

        public PlayerObjectOrientationSystem(FiringPlayerActionInherentData firingPlayerActionInherentDataRef, IPlayerInteractiveObject PlayerInteractiveObjectRef, TargetCursorManager targetCursorManagerRef,
            TargettableInteractiveObjectSelectionManager TargettableInteractiveObjectSelectionManager)
        {
            this.HorizontalPlaneGameObject = GameObject.Instantiate(firingPlayerActionInherentDataRef.FiringHorizontalPlanePrefab);
            this.HorizontalPlaneGameObject.layer = LayerMask.NameToLayer(LayerConstants.FIRING_ACTION_HORIZONTAL_LAYER);
            this.PlayerInteractiveObjectRef = PlayerInteractiveObjectRef;
            this._targetCursorManagerRef = targetCursorManagerRef;
            this.TargettableInteractiveObjectSelectionManager = TargettableInteractiveObjectSelectionManager;
        }

        public void Tick(float d)
        {
            if (this.TargettableInteractiveObjectSelectionManager.IsCurrentlyTargetting())
            {
                var playerTransform = this.PlayerInteractiveObjectRef.InteractiveGameObject.InteractiveGameObjectParent.transform;
                var rotationAngle = Quaternion.LookRotation((this.TargettableInteractiveObjectSelectionManager.GetCurrentlyTargettedInteractiveObject().InteractiveGameObject.GetTransform().WorldPosition - playerTransform.position).normalized,
                    playerTransform.up).eulerAngles;
                playerTransform.eulerAngles = new Vector3(playerTransform.eulerAngles.x, rotationAngle.y, playerTransform.eulerAngles.z);
            }
            else
            {
                this.HorizontalPlaneGameObject.transform.position = this.PlayerInteractiveObjectRef.InteractiveGameObject.GetTransform().WorldPosition;
                var projectionRay = Camera.main.ScreenPointToRay(this._targetCursorManagerRef.GetTargetCursorScreenPosition());
                if (Physics.Raycast(projectionRay, out RaycastHit hit, Mathf.Infinity, 1 << LayerMask.NameToLayer(LayerConstants.FIRING_ACTION_HORIZONTAL_LAYER)))
                {
                    var projectedPosition = hit.point;
                    var playerTransform = this.PlayerInteractiveObjectRef.InteractiveGameObject.InteractiveGameObjectParent.transform;
                    var lookDirection = (projectedPosition - playerTransform.position).normalized;

                    var rotationAngle = Vector3.SignedAngle(Vector3.forward, lookDirection, playerTransform.up);
                    playerTransform.eulerAngles = new Vector3(playerTransform.eulerAngles.x, rotationAngle, playerTransform.eulerAngles.z);
                }
            }
        }

        public void Dispose()
        {
            if (this.HorizontalPlaneGameObject != null)
            {
                GameObject.Destroy(this.HorizontalPlaneGameObject);
            }
        }
    }

    struct FiringProjectileTriggerSystem
    {
        private GameInputManager GameInputManager;
        private CoreInteractiveObject PlayerInteractiveObject;
        private TargettableInteractiveObjectSelectionManager TargettableInteractiveObjectSelectionManager;

        public FiringProjectileTriggerSystem(GameInputManager gameInputManager, CoreInteractiveObject PlayerInteractiveObject, TargettableInteractiveObjectSelectionManager TargettableInteractiveObjectSelectionManager)
        {
            GameInputManager = gameInputManager;
            this.PlayerInteractiveObject = PlayerInteractiveObject;
            this.TargettableInteractiveObjectSelectionManager = TargettableInteractiveObjectSelectionManager;
        }

        public void Tick(float d)
        {
            if (this.GameInputManager.CurrentInput.FiringProjectileDH())
            {
                /// If the Player is currently targettin an object via it's cursor
                if (this.TargettableInteractiveObjectSelectionManager.IsCurrentlyTargetting())
                {
                    var CurrentlyTargettedInteractiveObject = this.TargettableInteractiveObjectSelectionManager.GetCurrentlyTargettedInteractiveObject();
                    /// Projectile direction heads towards the FiringTargetLocation
                    /// var firingTargetWorldLocation = CurrentlyTargettedInteractiveObject.InteractiveGameObject.GetLocalToWorld().MultiplyPoint(CurrentlyTargettedInteractiveObject.GetFiringTargetLocalPosition());
                    this.PlayerInteractiveObject.AskToFireAFiredProjectile_ToTarget(CurrentlyTargettedInteractiveObject);
                }
                else
                {
                    this.PlayerInteractiveObject.AskToFireAFiredProjectile_Forward();
                }
            }
        }
    }

    struct ExitActionSystem
    {
        public bool ActionFinished { get; private set; }
        private GameInputManager GameInputManager;

        public ExitActionSystem(GameInputManager gameInputManager)
        {
            this.ActionFinished = false;
            GameInputManager = gameInputManager;
        }

        public void Tick(float d)
        {
            this.ActionFinished = this.GameInputManager.CurrentInput.FiringActionReleased();
        }
    }

    struct PlayerAnimationSystem
    {
        private CoreInteractiveObject PlayerCoreInteractiveObject;

        public PlayerAnimationSystem(CoreInteractiveObject PlayerCoreInteractiveObject, IAnimationInput FiringPoseAnimation)
        {
            this.PlayerCoreInteractiveObject = PlayerCoreInteractiveObject;
            PlayerCoreInteractiveObject.AnimationController.PlayLocomotionAnimationOverride(FiringPoseAnimation, AnimationLayerID.LocomotionLayer_1);
        }

        public void Dispose()
        {
            PlayerCoreInteractiveObject.AnimationController.DestroyAnimationLayer(AnimationLayerID.LocomotionLayer_1);
        }
    }

    /// <summary>
    /// Because the <see cref="PlayerCoreInteractiveObject"/> can move while <see cref="FiringPlayerAction"/> is running,
    /// this sysem changes fix the maximum speed of <see cref="PlayerCoreInteractiveObject"/>.
    /// /!\ Max speed value is resetted when <see cref="FiringPlayerAction"/> is exited (<see cref="Dispose"/>).
    /// </summary>
    struct PlayerSpeedSystem
    {
        private CoreInteractiveObject PlayerCoreInteractiveObject;
        private AIMovementSpeedAttenuationFactor InitialAIMovementSpeedAttenuationFactor;

        public PlayerSpeedSystem(CoreInteractiveObject playerCoreInteractiveObject)
        {
            PlayerCoreInteractiveObject = playerCoreInteractiveObject;
            this.InitialAIMovementSpeedAttenuationFactor = this.PlayerCoreInteractiveObject.GetCurrentSpeedAttenuationFactor();
            this.PlayerCoreInteractiveObject.SetAISpeedAttenuationFactor(AIMovementSpeedAttenuationFactor.WALK);
        }

        public void Dispose()
        {
            this.PlayerCoreInteractiveObject.SetAISpeedAttenuationFactor(this.InitialAIMovementSpeedAttenuationFactor);
        }
    }
}