using CoreGame;
using Input;
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

        private TargetCursorSystem TargetCursorSystem;
        private PlayerObjectOrientationSystem PlayerObjectOrientationSystem;
        private FiringProjectileTriggerSystem FiringProjectileTriggerSystem;
        private ExitActionSystem ExitActionSystem;

        public FiringPlayerAction(FiringPlayerActionInherentData FiringPlayerActionInherentData, IPlayerInteractiveObject PlayerInteractiveObject) : base(FiringPlayerActionInherentData.CorePlayerActionDefinition)
        {
            var targettableInteractiveObjectSelectionManager = TargettableInteractiveObjectSelectionManager.Get();
            var gameInputManager = GameInputManager.Get();
            this.FiringPlayerActionInherentData = FiringPlayerActionInherentData;
            this.TargetCursorSystem = new TargetCursorSystem(PlayerInteractiveObject, gameInputManager);
            this.PlayerObjectOrientationSystem = new PlayerObjectOrientationSystem(this.FiringPlayerActionInherentData, PlayerInteractiveObject, this.TargetCursorSystem, targettableInteractiveObjectSelectionManager);
            this.FiringProjectileTriggerSystem = new FiringProjectileTriggerSystem(gameInputManager, PlayerInteractiveObject as CoreInteractiveObject, targettableInteractiveObjectSelectionManager);
            this.ExitActionSystem = new ExitActionSystem(gameInputManager, this.TargetCursorSystem, this.PlayerObjectOrientationSystem);
        }

        public override void FirstExecution()
        {
            base.FirstExecution();
            this.TargetCursorSystem.CreateTargetCursor();
        }

        public override bool FinishedCondition()
        {
            return this.ExitActionSystem.ActionFinished;
        }

        public override void Tick(float d)
        {
            this.ExitActionSystem.Tick(d);
            if (!this.ExitActionSystem.ActionFinished)
            {
                this.TargetCursorSystem.Tick(d);
                this.PlayerObjectOrientationSystem.Tick(d);
                this.FiringProjectileTriggerSystem.Tick(d);
            }
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
        private TargetCursorSystem TargetCursorSystemRef;
        private TargettableInteractiveObjectSelectionManager TargettableInteractiveObjectSelectionManager;

        public PlayerObjectOrientationSystem(FiringPlayerActionInherentData firingPlayerActionInherentDataRef, IPlayerInteractiveObject PlayerInteractiveObjectRef, TargetCursorSystem TargetCursorSystemRef,
            TargettableInteractiveObjectSelectionManager TargettableInteractiveObjectSelectionManager)
        {
            this.HorizontalPlaneGameObject = GameObject.Instantiate(firingPlayerActionInherentDataRef.FiringHorizontalPlanePrefab);
            this.HorizontalPlaneGameObject.layer = LayerMask.NameToLayer(LayerConstants.FIRING_ACTION_HORIZONTAL_LAYER);
            this.PlayerInteractiveObjectRef = PlayerInteractiveObjectRef;
            this.TargetCursorSystemRef = TargetCursorSystemRef;
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
                var projectionRay = Camera.main.ScreenPointToRay(this.TargetCursorSystemRef.GetTargetCursorScreenPosition());
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
            if (this.GameInputManager.CurrentInput.GetInputCondition(InputID.FIRING_PROJECTILE_DOWN_HOLD))
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
        private TargetCursorSystem TargetCursorSystem;
        private PlayerObjectOrientationSystem PlayerObjectOrientationSystem;

        public ExitActionSystem(GameInputManager gameInputManager, TargetCursorSystem targetCursorSystem, PlayerObjectOrientationSystem PlayerObjectOrientationSystem)
        {
            GameInputManager = gameInputManager;
            TargetCursorSystem = targetCursorSystem;
            this.PlayerObjectOrientationSystem = PlayerObjectOrientationSystem;
        }

        public void Tick(float d)
        {
            this.ActionFinished = this.GameInputManager.CurrentInput.FiringActionReleased();
            if (this.ActionFinished)
            {
                this.TargetCursorSystem.Dispose();
                this.PlayerObjectOrientationSystem.Dispose();
            }
        }
    }
}