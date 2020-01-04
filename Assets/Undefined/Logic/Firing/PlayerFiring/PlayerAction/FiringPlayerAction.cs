using System;
using Input;
using InteractiveObjects;
using PlayerActions;
using PlayerObject_Interfaces;
using Targetting;
using UnityEngine;
using UnityEngine.Profiling;

namespace Firing
{
    public class FiringPlayerAction : PlayerAction
    {
        public const string FiringPlayerActionUniqueID = "FiringPlayerAction";

        private CoreInteractiveObject FiringInteractiveObject;
        private FiringPlayerActionInherentData FiringPlayerActionInherentData;

        private FiringLockSelectionSystem _firingLockSelectionSystem;
        private FiringPlayerActionTargetSystem FiringPlayerActionTargetSystem;
        private PlayerObjectOrientationSystem PlayerObjectOrientationSystem;
        private ExitActionSystem ExitActionSystem;
        private FiringRangeFeedbackSystem FiringRangeFeedbackSystem;

        public FiringPlayerAction(ref FiringPlayerActionInput FiringPlayerActionInput,
            Action OnPlayerActionStartedCallback,
            Action OnPlayerActionEndCallback) : base(FiringPlayerActionInput.FiringPlayerActionInherentData.CorePlayerActionDefinition, OnPlayerActionStartedCallback, OnPlayerActionEndCallback)
        {
            this.FiringInteractiveObject = FiringPlayerActionInput.firingInteractiveObject;

            var gameInputManager = GameInputManager.Get();
            this.FiringPlayerActionInherentData = FiringPlayerActionInput.FiringPlayerActionInherentData;

            this.FiringPlayerActionTargetSystem = new FiringPlayerActionTargetSystem(this.FiringPlayerActionInherentData, this.FiringInteractiveObject, TargetCursorManager.Get());
            this._firingLockSelectionSystem = new FiringLockSelectionSystem(this.FiringPlayerActionTargetSystem.OnInteractiveObjectTargetted);
            this.PlayerObjectOrientationSystem = new PlayerObjectOrientationSystem(this.FiringInteractiveObject as IPlayerInteractiveObject, this.FiringPlayerActionTargetSystem);

            this.ExitActionSystem = new ExitActionSystem(gameInputManager);

            /// Initialisation of states
            this.Tick(0f);
            this.AfterTicks(0f);
        }

        public override void FirstExecution()
        {
            base.FirstExecution();
        }

        public override string PlayerActionUniqueID
        {
            get { return FiringPlayerActionUniqueID; }
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

        public override void Tick(float d)
        {
            Profiler.BeginSample("FiringPlayerAction");
            this._firingLockSelectionSystem.Tick();
            this.ExitActionSystem.Tick(d);
            if (!this.ExitActionSystem.ActionFinished)
            {
                this.FiringPlayerActionTargetSystem.Tick(d);
            }

            Profiler.EndSample();
        }

        public override void AfterTicks(float d)
        {
            this.ExitActionSystem.Tick(d);
            if (!this.ExitActionSystem.ActionFinished)
            {
                /// The FiringRangeFeedbackSystem is not initialized in the constructor to be sure that FiringRangeFeedbackSystem position initialization takes into account the computed position of the player
                /// for the current frame.
                if (!this.FiringRangeFeedbackSystem.IsInitialized)
                {
                    this.FiringRangeFeedbackSystem = new FiringRangeFeedbackSystem(this.FiringInteractiveObject, this.FiringPlayerActionTargetSystem);
                }

                this.FiringRangeFeedbackSystem.AfterPlayerTick(d);
            }
        }

        public override void TickTimeFrozen(float d)
        {
            if (!this.ExitActionSystem.ActionFinished)
            {
                this._firingLockSelectionSystem.Tick();
                this.FiringPlayerActionTargetSystem.Tick(d);
                this.FiringRangeFeedbackSystem.AfterPlayerTick(d);
            }
        }

        public override void LateTick(float d)
        {
            
        }

        public override void Dispose()
        {
            this._firingLockSelectionSystem.Dispose();
            this.FiringPlayerActionTargetSystem.Dispose();
            this.FiringRangeFeedbackSystem.Dispose();

            base.Dispose();
        }

        #region Data Retrieval

        public Vector3 GetCurrentTargetDirection()
        {
            return this.FiringPlayerActionTargetSystem.TargetDirection;
        }

        #endregion

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

        private CoreInteractiveObject FiringInteractiveObject;

        private ObjectVariable<CoreInteractiveObject> CurrentlyTargettedInteractiveObject;
        private GameObject TargetPlaneGameObject;
        public Vector3 TargetDirection;

        private GameObject DottedVisualFeeback;

        public FiringPlayerActionTargetSystem(FiringPlayerActionInherentData firingPlayerActionInherentDataRef, CoreInteractiveObject firingInteractiveObject, TargetCursorManager targetCursorManagerRef)
        {
            this._targetCursorManagerRef = targetCursorManagerRef;
            this.FiringInteractiveObject = firingInteractiveObject;
            this.TargetPlaneGameObject = GameObject.Instantiate(firingPlayerActionInherentDataRef.FiringHorizontalPlanePrefab);
            this.TargetPlaneGameObject.layer = LayerMask.NameToLayer(LayerConstants.FIRING_ACTION_HORIZONTAL_LAYER);
            this.DottedVisualFeeback = GameObject.Instantiate(firingPlayerActionInherentDataRef.GroundConeVisualFeedbackPrefab);
            this.CurrentlyTargettedInteractiveObject = new ObjectVariable<CoreInteractiveObject>(this.OnCurrentlyTargettedInteractiveObjectChange);
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

                this.TargetDirection = (hit.point - this.FiringInteractiveObject.GetWeaponWorldFirePoint()).normalized;
            }
        }

        private void UpdateTargetPlanePosition()
        {
            if (this.CurrentlyTargettedInteractiveObject.GetValue() != null)
            {
                this.TargetPlaneGameObject.transform.position = this.CurrentlyTargettedInteractiveObject.GetValue().InteractiveGameObject.GetLocalToWorld().MultiplyPoint(this.CurrentlyTargettedInteractiveObject.GetValue().GetFiringTargetLocalPosition());
            }
            else
            {
                this.TargetPlaneGameObject.transform.position = this.FiringInteractiveObject.InteractiveGameObject.GetLocalToWorld().MultiplyPoint(this.FiringInteractiveObject.GetFiringTargetLocalPosition());
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

            if (this.CurrentlyTargettedInteractiveObject.GetValue() != null)
            {
                this.CurrentlyTargettedInteractiveObject.GetValue().UnRegisterInteractiveObjectDestroyedEventListener(this.OnInteractiveObjectDestroyed);
            }
        }

        public void OnInteractiveObjectTargetted(CoreInteractiveObject CoreInteractiveObject)
        {
            if (CoreInteractiveObject != null)
            {
                this.CurrentlyTargettedInteractiveObject.SetValue(CoreInteractiveObject);
                this.UpdateTargetPlanePosition();
            }
        }

        private void OnInteractiveObjectDestroyed(CoreInteractiveObject destroyedInteractiveObject)
        {
            if (destroyedInteractiveObject == this.CurrentlyTargettedInteractiveObject.GetValue())
            {
                this.CurrentlyTargettedInteractiveObject.SetValue(null);
            }
        }

        private void OnCurrentlyTargettedInteractiveObjectChange(CoreInteractiveObject oldCurrentlyTargettedInteractiveObject, CoreInteractiveObject newCurrentlyTargettedInteractiveObject)
        {
            if (oldCurrentlyTargettedInteractiveObject != null)
            {
                oldCurrentlyTargettedInteractiveObject.UnRegisterInteractiveObjectDestroyedEventListener(this.OnInteractiveObjectDestroyed);
            }

            if (newCurrentlyTargettedInteractiveObject != null)
            {
                newCurrentlyTargettedInteractiveObject.RegisterInteractiveObjectDestroyedEventListener(this.OnInteractiveObjectDestroyed);
            }
        }
    }

    struct PlayerObjectOrientationSystem
    {
        private IPlayerInteractiveObject PlayerInteractiveObjectRef;
        private FiringPlayerActionTargetSystem FiringPlayerActionTargetSystemRef;

        public PlayerObjectOrientationSystem(IPlayerInteractiveObject PlayerInteractiveObjectRef,
            FiringPlayerActionTargetSystem FiringPlayerActionTargetSystemRef)
        {
            this.PlayerInteractiveObjectRef = PlayerInteractiveObjectRef;
            this.FiringPlayerActionTargetSystemRef = FiringPlayerActionTargetSystemRef;
        }

        public void FixedTick(float d)
        {
            UpdatePlayerRotationConstraint();
        }

        /// <summary>
        /// /!\ The Player rotation constraint must be updated only in the fixed tick because the TargetDirection calculated in <see cref="FiringPlayerActionTargetSystem"/> is accurate only during the Physics step.
        /// </summary>
        private void UpdatePlayerRotationConstraint()
        {
            var playerTransform = this.PlayerInteractiveObjectRef.InteractiveGameObject.Agent.transform;
            Vector3 playerNormalProjectedOrientedDirection = Vector3.ProjectOnPlane(this.FiringPlayerActionTargetSystemRef.TargetDirection, playerTransform.up).normalized;

            var rotationAngle = Quaternion.LookRotation(playerNormalProjectedOrientedDirection,
                playerTransform.up).eulerAngles;
            this.PlayerInteractiveObjectRef.SetConstraintForThisFrame(new LookDirectionConstraint(Quaternion.Euler(new Vector3(playerTransform.eulerAngles.x, rotationAngle.y, playerTransform.eulerAngles.z))));
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