using AnimatorPlayable;
using Input;
using InteractiveObject_Animation;
using InteractiveObjects;
using PlayerActions;
using PlayerObject_Interfaces;
using Targetting;
using UnityEngine;

namespace Firing
{
    public class FiringPlayerAction : PlayerAction
    {
        private FiringPlayerActionInherentData FiringPlayerActionInherentData;

        private PlayerObjectOrientationSystem PlayerObjectOrientationSystem;
        private FiringProjectileTriggerSystem FiringProjectileTriggerSystem;
        private ExitActionSystem ExitActionSystem;
        private PlayerAnimationSystem PlayerAnimationSystem;

        public FiringPlayerAction(FiringPlayerActionInherentData FiringPlayerActionInherentData, IPlayerInteractiveObject PlayerInteractiveObject) : base(FiringPlayerActionInherentData.CorePlayerActionDefinition)
        {
            CoreInteractiveObject playerCoreInteractiveObject = PlayerInteractiveObject as CoreInteractiveObject;
            var targettableInteractiveObjectSelectionManager = TargettableInteractiveObjectSelectionManager.Get();
            var gameInputManager = GameInputManager.Get();
            this.FiringPlayerActionInherentData = FiringPlayerActionInherentData;
            this.PlayerObjectOrientationSystem = new PlayerObjectOrientationSystem(this.FiringPlayerActionInherentData, PlayerInteractiveObject, TargetCursorManager.Get(), targettableInteractiveObjectSelectionManager);
            this.FiringProjectileTriggerSystem = new FiringProjectileTriggerSystem(gameInputManager, playerCoreInteractiveObject, targettableInteractiveObjectSelectionManager);
            this.ExitActionSystem = new ExitActionSystem(gameInputManager, this.PlayerObjectOrientationSystem);
            this.PlayerAnimationSystem = new PlayerAnimationSystem(playerCoreInteractiveObject, FiringPlayerActionInherentData.FiringPoseAnimation);
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
            this.ExitActionSystem.Tick(d);
            if (!this.ExitActionSystem.ActionFinished)
            {
                this.PlayerObjectOrientationSystem.Tick(d);
                this.FiringProjectileTriggerSystem.Tick(d);
            }
        }

        public override void Dispose()
        {
            this.ExitActionSystem.Dispose();
            this.PlayerAnimationSystem.Dispose();
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

    class PlayerObjectOrientationSystem
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

    class FiringProjectileTriggerSystem
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
                    var firingTargetWorldLocation = CurrentlyTargettedInteractiveObject.InteractiveGameObject.GetLocalToWorld().MultiplyPoint(CurrentlyTargettedInteractiveObject.GetFiringTargetLocalPosition());
                    this.PlayerInteractiveObject.AskToFireAFiredProjectile_ToTargetPoint(firingTargetWorldLocation);
                }
                else
                {
                    this.PlayerInteractiveObject.AskToFireAFiredProjectile_Forward();
                }
            }
        }
    }

    class ExitActionSystem
    {
        public bool ActionFinished { get; private set; }
        private GameInputManager GameInputManager;
        private PlayerObjectOrientationSystem PlayerObjectOrientationSystem;

        public ExitActionSystem(GameInputManager gameInputManager, PlayerObjectOrientationSystem PlayerObjectOrientationSystem)
        {
            GameInputManager = gameInputManager;
            this.PlayerObjectOrientationSystem = PlayerObjectOrientationSystem;
        }

        public void Tick(float d)
        {
            this.ActionFinished = this.GameInputManager.CurrentInput.FiringActionReleased();
            if (this.ActionFinished)
            {
                this.Dispose();
            }
        }

        public void Dispose()
        {
            this.PlayerObjectOrientationSystem.Dispose();
        }
    }

    class PlayerAnimationSystem
    {
        private CoreInteractiveObject PlayerCoreInteractiveObject;

        public PlayerAnimationSystem(CoreInteractiveObject PlayerCoreInteractiveObject, SequencedAnimationInput FiringPoseAnimation)
        {
            this.PlayerCoreInteractiveObject = PlayerCoreInteractiveObject;
            PlayerCoreInteractiveObject.AnimationController.PlayLocomotionAnimationOverride(FiringPoseAnimation, AnimationLayerID.LocomotionLayer_1);
        }

        public void Dispose()
        {
            PlayerCoreInteractiveObject.AnimationController.DestroyAnimationLayer(AnimationLayerID.LocomotionLayer_1);
        }
    }
}