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
        private IPlayerInteractiveObject IPlayerInteractiveObject;
        private FiringPlayerActionInherentData FiringPlayerActionInherentData;

        #region External Dependencies

        private TargettableInteractiveObjectSelectionManager TargettableInteractiveObjectSelectionManager = TargettableInteractiveObjectSelectionManager.Get();

        #endregion

        private PlayerObjectOrientationSystem PlayerObjectOrientationSystem;
        private FiringProjectileTriggerSystem FiringProjectileTriggerSystem;
        private ExitActionSystem ExitActionSystem;
        private PlayerSpeedSystem PlayerSpeedSystem;
        private FiringRangeFeedbackSystem FiringRangeFeedbackSystem;

        public FiringPlayerAction(FiringPlayerActionInherentData FiringPlayerActionInherentData, IPlayerInteractiveObject PlayerInteractiveObject) : base(FiringPlayerActionInherentData.CorePlayerActionDefinition)
        {
            this.IPlayerInteractiveObject = PlayerInteractiveObject;

            var PlayerCoreInteractiveObject = PlayerInteractiveObject as CoreInteractiveObject;
            var gameInputManager = GameInputManager.Get();
            this.FiringPlayerActionInherentData = FiringPlayerActionInherentData;
            this.PlayerObjectOrientationSystem = new PlayerObjectOrientationSystem(this.FiringPlayerActionInherentData, PlayerInteractiveObject, TargetCursorManager.Get(), this.TargettableInteractiveObjectSelectionManager);

            /// This is to change InteractiveObject rotation at the first frame of action execution
            this.PlayerObjectOrientationSystem.Tick(0f);

            this.FiringProjectileTriggerSystem = new FiringProjectileTriggerSystem(gameInputManager, PlayerCoreInteractiveObject, this.TargettableInteractiveObjectSelectionManager);
            this.ExitActionSystem = new ExitActionSystem(gameInputManager);
            this.PlayerSpeedSystem = new PlayerSpeedSystem(PlayerCoreInteractiveObject);

            PlayerInteractiveObject.OnPlayerStartTargetting(this.FiringPlayerActionInherentData.FiringPoseAnimationV2);
        }

        public override void FirstExecution()
        {
            base.FirstExecution();
        }

        public override bool FinishedCondition()
        {
            return this.ExitActionSystem.ActionFinished || base.FinishedCondition();
        }

        public override void FixedTick(float d)
        {
            if (!this.ExitActionSystem.ActionFinished)
            {
                this.PlayerObjectOrientationSystem.FixedTick(d);
            }
        }

        public override void BeforePlayerTick(float d)
        {
            Profiler.BeginSample("FiringPlayerAction");
            this.ExitActionSystem.Tick(d);
            if (!this.ExitActionSystem.ActionFinished)
            {
                this.PlayerObjectOrientationSystem.Tick(d);
                this.FiringProjectileTriggerSystem.Tick(d);
            }

            Profiler.EndSample();
        }

        public override void AfterPlayerTick(float d)
        {
            this.ExitActionSystem.Tick(d);
            if (!this.ExitActionSystem.ActionFinished)
            {
                /// The FiringRangeFeedbackSystem is not initialized in the constructor to be sure that FiringRangeFeedbackSystem position initialization takes into account the computed position of the player
                /// for the current frame.
                if (!this.FiringRangeFeedbackSystem.IsInitialized)
                {
                    this.FiringRangeFeedbackSystem = new FiringRangeFeedbackSystem(this.IPlayerInteractiveObject as CoreInteractiveObject, this.TargettableInteractiveObjectSelectionManager);
                }

                this.FiringRangeFeedbackSystem.AfterPlayerTick(d);
            }
        }


        public override void LateTick(float d)
        {
            this.PlayerObjectOrientationSystem.LateTick(d);
        }

        public override void Dispose()
        {
            this.PlayerObjectOrientationSystem.Dispose();
            this.PlayerSpeedSystem.Dispose();
            this.FiringRangeFeedbackSystem.Dispose();

            this.IPlayerInteractiveObject.OnPlayerStoppedTargetting();
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

        private bool UpdatedThisFrame;


        public PlayerObjectOrientationSystem(FiringPlayerActionInherentData firingPlayerActionInherentDataRef, IPlayerInteractiveObject PlayerInteractiveObjectRef, TargetCursorManager targetCursorManagerRef,
            TargettableInteractiveObjectSelectionManager TargettableInteractiveObjectSelectionManager)
        {
            this.HorizontalPlaneGameObject = GameObject.Instantiate(firingPlayerActionInherentDataRef.FiringHorizontalPlanePrefab);
            this.HorizontalPlaneGameObject.layer = LayerMask.NameToLayer(LayerConstants.FIRING_ACTION_HORIZONTAL_LAYER);
            this.PlayerInteractiveObjectRef = PlayerInteractiveObjectRef;
            this._targetCursorManagerRef = targetCursorManagerRef;
            this.TargettableInteractiveObjectSelectionManager = TargettableInteractiveObjectSelectionManager;
            this.UpdatedThisFrame = false;
        }


        public void FixedTick(float d)
        {
            if (!UpdatedThisFrame)
            {
                UpdatedThisFrame = true;
                UpdatePlayerRotationConstraint();
            }
        }

        public void Tick(float d)
        {
            if (!UpdatedThisFrame)
            {
                UpdatedThisFrame = true;
                UpdatePlayerRotationConstraint();
            }
        }

        public void LateTick(float d)
        {
            this.UpdatedThisFrame = false;
        }

        private void UpdatePlayerRotationConstraint()
        {
            if (this.TargettableInteractiveObjectSelectionManager.IsCurrentlyTargetting())
            {
                var playerTransform = this.PlayerInteractiveObjectRef.InteractiveGameObject.InteractiveGameObjectParent.transform;
                var rotationAngle = Quaternion.LookRotation((this.TargettableInteractiveObjectSelectionManager.GetCurrentlyTargettedInteractiveObject().InteractiveGameObject.GetTransform().WorldPosition - playerTransform.position).normalized,
                    playerTransform.up).eulerAngles;
                this.PlayerInteractiveObjectRef.SetConstraintForThisFrame(new LookDirectionConstraint(Quaternion.Euler(new Vector3(playerTransform.eulerAngles.x, rotationAngle.y, playerTransform.eulerAngles.z))));
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

                    this.PlayerInteractiveObjectRef.SetConstraintForThisFrame(new LookDirectionConstraint(Quaternion.Euler(new Vector3(playerTransform.eulerAngles.x, rotationAngle, playerTransform.eulerAngles.z))));
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
            /// We use the FiringActionDownHold to be sure to not miss the frame where button has been released.
            this.ActionFinished = !this.GameInputManager.CurrentInput.FiringActionDownHold();
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