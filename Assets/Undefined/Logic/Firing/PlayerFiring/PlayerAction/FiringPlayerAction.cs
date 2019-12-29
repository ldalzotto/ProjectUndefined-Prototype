using System;
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

        private FiringLockSelectionManager FiringLockSelectionManager;
        private FiringPlayerActionTargetSystem FiringPlayerActionTargetSystem;
        private PlayerObjectOrientationSystem PlayerObjectOrientationSystem;
        private FiringProjectileTriggerSystem FiringProjectileTriggerSystem;
        private ExitActionSystem ExitActionSystem;
        private FiringRangeFeedbackSystem FiringRangeFeedbackSystem;

        public FiringPlayerAction(FiringPlayerActionInherentData FiringPlayerActionInherentData, IPlayerInteractiveObject PlayerInteractiveObject,
            Action OnPlayerActionStartedCallback,
            Action OnPlayerActionEndCallback) : base(FiringPlayerActionInherentData.CorePlayerActionDefinition, OnPlayerActionStartedCallback, OnPlayerActionEndCallback)
        {
            this.IPlayerInteractiveObject = PlayerInteractiveObject;

            var PlayerCoreInteractiveObject = PlayerInteractiveObject as CoreInteractiveObject;
            var gameInputManager = GameInputManager.Get();
            this.FiringPlayerActionInherentData = FiringPlayerActionInherentData;

            this.FiringPlayerActionTargetSystem = new FiringPlayerActionTargetSystem(this.FiringPlayerActionInherentData, PlayerInteractiveObject, TargetCursorManager.Get());
            this.FiringLockSelectionManager = new FiringLockSelectionManager(this.FiringPlayerActionTargetSystem.OnInteractiveObjectTargetted);
            this.PlayerObjectOrientationSystem = new PlayerObjectOrientationSystem(PlayerInteractiveObject, this.FiringPlayerActionTargetSystem);

            /// This is to change InteractiveObject rotation at the first frame of action execution
            this.PlayerObjectOrientationSystem.Tick(0f);

            this.FiringProjectileTriggerSystem = new FiringProjectileTriggerSystem(gameInputManager, PlayerCoreInteractiveObject, this.FiringPlayerActionTargetSystem);
            this.ExitActionSystem = new ExitActionSystem(gameInputManager);
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
            this.FiringLockSelectionManager.Tick();
            this.ExitActionSystem.Tick(d);
            if (!this.ExitActionSystem.ActionFinished)
            {
                this.FiringPlayerActionTargetSystem.Tick(d);
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
                    this.FiringRangeFeedbackSystem = new FiringRangeFeedbackSystem(this.IPlayerInteractiveObject as CoreInteractiveObject, this.FiringPlayerActionTargetSystem);
                }

                this.FiringRangeFeedbackSystem.AfterPlayerTick(d);
            }
        }

        public override void TickTimeFrozen(float d)
        {
            if (!this.ExitActionSystem.ActionFinished)
            {
                this.FiringLockSelectionManager.Tick();
                this.FiringPlayerActionTargetSystem.Tick(d);
                this.FiringRangeFeedbackSystem.AfterPlayerTick(d);
            }
        }


        public override void LateTick(float d)
        {
            this.PlayerObjectOrientationSystem.LateTick(d);
        }

        public override void Dispose()
        {
            this.FiringLockSelectionManager.Dispose();
            this.FiringPlayerActionTargetSystem.Dispose();
            this.FiringRangeFeedbackSystem.Dispose();

            base.Dispose();
        }


        public override void GUITick()
        {
        }

        public override void GizmoTick()
        {
        }
    }

    public class FiringPlayerActionTargetSystem
    {
        private TargetCursorManager _targetCursorManagerRef;

        private IPlayerInteractiveObject PlayerInteractiveObjectRef;

        private CoreInteractiveObject CurrentlyTargettedInteractiveObject;
        private GameObject TargetPlaneGameObject;
        public Vector3 TargetDirection;

        private GameObject DottedVisualFeeback;

        public FiringPlayerActionTargetSystem(FiringPlayerActionInherentData firingPlayerActionInherentDataRef, IPlayerInteractiveObject PlayerInteractiveObjectRef, TargetCursorManager targetCursorManagerRef)
        {
            this._targetCursorManagerRef = targetCursorManagerRef;
            this.PlayerInteractiveObjectRef = PlayerInteractiveObjectRef;
            this.TargetPlaneGameObject = GameObject.Instantiate(firingPlayerActionInherentDataRef.FiringHorizontalPlanePrefab);
            this.TargetPlaneGameObject.layer = LayerMask.NameToLayer(LayerConstants.FIRING_ACTION_HORIZONTAL_LAYER);
            this.DottedVisualFeeback = GameObject.Instantiate(firingPlayerActionInherentDataRef.DottedVisualFeebackPrefab);

            this.Tick(0f);
        }

        public void Tick(float d)
        {
            this.UpdateTargetPlanePosition();

            var projectionRay = Camera.main.ScreenPointToRay(this._targetCursorManagerRef.GetTargetCursorScreenPosition());
            if (Physics.Raycast(projectionRay, out RaycastHit hit, Mathf.Infinity, 1 << LayerMask.NameToLayer(LayerConstants.FIRING_ACTION_HORIZONTAL_LAYER)))
            {
                if (Physics.Raycast(hit.point, Vector3.down, out RaycastHit groundHit, 100f, 1 << LayerMask.NameToLayer(LayerConstants.PUZZLE_GROUND_LAYER)))
                {
                    this.DottedVisualFeeback.transform.position = groundHit.point;
                }

                this.TargetDirection = (hit.point - (this.PlayerInteractiveObjectRef as CoreInteractiveObject).GetWeaponWorldFirePoint()).normalized;
            }
        }

        private void UpdateTargetPlanePosition()
        {
            if (this.CurrentlyTargettedInteractiveObject != null)
            {
                this.TargetPlaneGameObject.transform.position = this.CurrentlyTargettedInteractiveObject.InteractiveGameObject.GetLocalToWorld().MultiplyPoint(this.CurrentlyTargettedInteractiveObject.GetFiringTargetLocalPosition());
            }
            else
            {
                var playerObject = this.PlayerInteractiveObjectRef as CoreInteractiveObject;
                this.TargetPlaneGameObject.transform.position = playerObject.InteractiveGameObject.GetLocalToWorld().MultiplyPoint(playerObject.GetFiringTargetLocalPosition());
            }
        }

        public void Dispose()
        {
            if (this.TargetPlaneGameObject != null)
            {
                GameObject.Destroy(this.TargetPlaneGameObject);
            }

            if (this.DottedVisualFeeback != null)
            {
                GameObject.Destroy(this.DottedVisualFeeback);
            }
        }

        public void OnInteractiveObjectTargetted(CoreInteractiveObject CoreInteractiveObject)
        {
            if (CoreInteractiveObject != null)
            {
                this.CurrentlyTargettedInteractiveObject = CoreInteractiveObject;
                this.UpdateTargetPlanePosition();
            }
        }

        #region Data Retrieval

        #endregion
    }

    struct PlayerObjectOrientationSystem
    {
        private IPlayerInteractiveObject PlayerInteractiveObjectRef;
        private FiringPlayerActionTargetSystem FiringPlayerActionTargetSystemRef;

        private bool UpdatedThisFrame;


        public PlayerObjectOrientationSystem(IPlayerInteractiveObject PlayerInteractiveObjectRef,
            FiringPlayerActionTargetSystem FiringPlayerActionTargetSystemRef)
        {
            this.PlayerInteractiveObjectRef = PlayerInteractiveObjectRef;
            this.FiringPlayerActionTargetSystemRef = FiringPlayerActionTargetSystemRef;
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
            var playerTransform = this.PlayerInteractiveObjectRef.InteractiveGameObject.Agent.transform;
            Vector3 playerNormalProjectedOrientedDirection = Vector3.ProjectOnPlane(this.FiringPlayerActionTargetSystemRef.TargetDirection, playerTransform.up).normalized;

            var rotationAngle = Quaternion.LookRotation(playerNormalProjectedOrientedDirection,
                playerTransform.up).eulerAngles;
            this.PlayerInteractiveObjectRef.SetConstraintForThisFrame(new LookDirectionConstraint(Quaternion.Euler(new Vector3(playerTransform.eulerAngles.x, rotationAngle.y, playerTransform.eulerAngles.z))));
        }
    }

    struct FiringProjectileTriggerSystem
    {
        private GameInputManager GameInputManager;
        private CoreInteractiveObject PlayerInteractiveObject;
        private FiringPlayerActionTargetSystem FiringPlayerActionTargetSystemRef;

        public FiringProjectileTriggerSystem(GameInputManager gameInputManager, CoreInteractiveObject PlayerInteractiveObject, FiringPlayerActionTargetSystem FiringPlayerActionTargetSystemRef)
        {
            GameInputManager = gameInputManager;
            this.PlayerInteractiveObject = PlayerInteractiveObject;
            this.FiringPlayerActionTargetSystemRef = FiringPlayerActionTargetSystemRef;
        }

        public void Tick(float d)
        {
            if (this.GameInputManager.CurrentInput.FiringProjectileDH())
            {
                this.PlayerInteractiveObject.AskToFireAFiredProjectile_ToDirection(this.FiringPlayerActionTargetSystemRef.TargetDirection);
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
}