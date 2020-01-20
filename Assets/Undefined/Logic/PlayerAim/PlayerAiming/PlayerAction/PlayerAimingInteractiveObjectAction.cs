using System;
using CoreGame;
using Input;
using InteractiveObjectAction;
using InteractiveObjects;
using PlayerObject_Interfaces;
using Targetting;
using UnityEngine;
using UnityEngine.Profiling;

namespace PlayerAim
{
    public class PlayerAimingInteractiveObjectAction : AInteractiveObjectAction
    {
        public const string PlayerAimingInteractiveObjectActionUniqueID = "PlayerAimingInteractiveObjectAction";

        private CoreInteractiveObject FiringInteractiveObject;
        private PlayerAimingInteractiveObjectActionInherentData _playerAimingInteractiveObjectActionInherentData;

        private FiringLockSelectionSystem _firingLockSelectionSystem;
        private FiringPlayerActionTargetSystem FiringPlayerActionTargetSystem;
        private PlayerObjectOrientationSystem PlayerObjectOrientationSystem;
        private ExitActionSystem ExitActionSystem;
        private InteractiveObjectTargettedVisualFeedback InteractiveObjectTargettedVisualFeedback;
        private PlayerAimRangeFeedbackSystem _playerAimRangeFeedbackSystem;

        public PlayerAimingInteractiveObjectAction(ref FiringInteractiveObjectActionInput firingInteractiveObjectActionInput) :
            base(firingInteractiveObjectActionInput.PlayerAimingInteractiveObjectActionInherentData.coreInteractiveObjectActionDefinition)
        {
            this.FiringInteractiveObject = firingInteractiveObjectActionInput.firingInteractiveObject;

            var gameInputManager = GameInputManager.Get();
            this._playerAimingInteractiveObjectActionInherentData = firingInteractiveObjectActionInput.PlayerAimingInteractiveObjectActionInherentData;

            this.FiringPlayerActionTargetSystem = new FiringPlayerActionTargetSystem(this._playerAimingInteractiveObjectActionInherentData, this.FiringInteractiveObject, TargetCursorManager.Get());
            this._firingLockSelectionSystem = new FiringLockSelectionSystem(this.FiringPlayerActionTargetSystem.OnInteractiveObjectTargetted);
            this.PlayerObjectOrientationSystem = new PlayerObjectOrientationSystem(this.FiringInteractiveObject as IPlayerInteractiveObject, this.FiringPlayerActionTargetSystem);
            this.InteractiveObjectTargettedVisualFeedback = new InteractiveObjectTargettedVisualFeedback(firingInteractiveObjectActionInput.PlayerAimingInteractiveObjectActionInherentData, _firingLockSelectionSystem, Camera.main);

            this.ExitActionSystem = new ExitActionSystem(gameInputManager);

            /// Initialisation of states
            this.Tick(0f);
            this.AfterTicks(0f);
        }

        public override void FirstExecution()
        {
            base.FirstExecution();
            if (this.FiringInteractiveObject is IEM_IFiringAInteractiveObjectAction_EventsListener IFiringAInteractiveObjectAction_EventsListener)
            {
                IFiringAInteractiveObjectAction_EventsListener.OnFiringInteractiveObjectActionStart(this._playerAimingInteractiveObjectActionInherentData);
            }
        }

        public override string InteractiveObjectActionUniqueID
        {
            get { return PlayerAimingInteractiveObjectActionUniqueID; }
        }

        public override bool FinishedCondition()
        {
            return this.ExitActionSystem.ActionFinished || base.FinishedCondition();
        }

        public override void FixedTick(float d)
        {
            if (!this.ExitActionSystem.ActionFinished)
            {
                /// Calling CreatePlayerAimRangeFeedbackSystemIfNotCreated is a good place here because FixedTick will be called just after a physics world update.
                CreatePlayerAimRangeFeedbackSystemIfNotCreated();

                this.PlayerObjectOrientationSystem.FixedTick(d);
            }
        }

        public override void FixedTickTimeFrozen(float d)
        {
            this.FixedTick(d);
        }

        public override void Tick(float d)
        {
            Profiler.BeginSample("PlayerAimingInteractiveObjectAction");
            this._firingLockSelectionSystem.Tick();
            this.InteractiveObjectTargettedVisualFeedback.Tick(d);

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
                this._playerAimRangeFeedbackSystem.AfterPlayerTick(d);
            }
        }

        public override void TickTimeFrozen(float d)
        {
            this.ExitActionSystem.Tick(d);
            if (!this.ExitActionSystem.ActionFinished)
            {
                this._firingLockSelectionSystem.Tick();
                this.FiringPlayerActionTargetSystem.Tick(d);
                this._playerAimRangeFeedbackSystem.AfterPlayerTick(d);
            }
        }

        public override void LateTick(float d)
        {
        }

        public override void Dispose()
        {
            this._firingLockSelectionSystem.Dispose();
            this.FiringPlayerActionTargetSystem.Dispose();
            this._playerAimRangeFeedbackSystem.Dispose();
            this.InteractiveObjectTargettedVisualFeedback.Dispose();

            if (this.FiringInteractiveObject is IEM_IFiringAInteractiveObjectAction_EventsListener IFiringAInteractiveObjectAction_EventsListener)
            {
                IFiringAInteractiveObjectAction_EventsListener.OnFiringInteractiveObjectActionEnd(this._playerAimingInteractiveObjectActionInherentData);
            }

            base.Dispose();
        }


        /// <summary>
        /// The PlayerAimRangeFeedbackSystem is not initialized in the constructor to be sure that PlayerAimRangeFeedbackSystem position initialization takes into account the computed position of the player
        /// for the current frame.
        /// /!\ The creation of <see cref="_playerAimRangeFeedbackSystem"/> must absolutely be done after that physics objects have been taken into account from the physics engine.
        ///     This is because the <see cref="FiringPlayerActionTargetSystem"/> uses operations on newly created physics object to calculate the target direction <see cref="FiringPlayerActionTargetSystem.Tick"/>
        /// </summary>
        private void CreatePlayerAimRangeFeedbackSystemIfNotCreated()
        {
            if (!this._playerAimRangeFeedbackSystem.IsInitialized)
            {
                this._playerAimRangeFeedbackSystem = new PlayerAimRangeFeedbackSystem(this.FiringInteractiveObject, this.FiringPlayerActionTargetSystem);
            }
        }

        #region Data Retrieval

        public Vector3 GetCurrentTargetDirection()
        {
            return this.FiringPlayerActionTargetSystem.TargetDirection;
        }

        public CoreInteractiveObject GetCurrentlyTargettedInteractiveObject()
        {
            return this._firingLockSelectionSystem.GetCurrentlyTargettedInteractiveObject();
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

        public FiringPlayerActionTargetSystem(PlayerAimingInteractiveObjectActionInherentData playerAimingInteractiveObjectActionInherentDataRef, CoreInteractiveObject firingInteractiveObject, TargetCursorManager targetCursorManagerRef)
        {
            this._targetCursorManagerRef = targetCursorManagerRef;
            this.FiringInteractiveObject = firingInteractiveObject;
            this.TargetPlaneGameObject = GameObject.Instantiate(playerAimingInteractiveObjectActionInherentDataRef.FiringHorizontalPlanePrefab);
            this.TargetPlaneGameObject.layer = LayerMask.NameToLayer(LayerConstants.FIRING_ACTION_HORIZONTAL_LAYER);
            this.DottedVisualFeeback = GameObject.Instantiate(playerAimingInteractiveObjectActionInherentDataRef.GroundConeVisualFeedbackPrefab);

            /// Until it's first update, the DottedVisualFeeback is set far away from screen.
            this.DottedVisualFeeback.transform.position = new Vector3(99999f, 99999f, 99999f);

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
            if (CoreInteractiveObject != null && !CoreInteractiveObject.IsAskingToBeDestroyed)
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

    struct InteractiveObjectTargettedVisualFeedback
    {
        private InteractiveObjectTargettedVisualFeedbackObject InteractiveObjectTargettedVisualFeedbackObject;
        private FiringLockSelectionSystem FiringLockSelectionSystemRef;
        private Camera mainCamera;

        public InteractiveObjectTargettedVisualFeedback(PlayerAimingInteractiveObjectActionInherentData PlayerAimingInteractiveObjectActionInherentData,
            FiringLockSelectionSystem FiringLockSelectionSystemRef, Camera mainCamera)
        {
            this.FiringLockSelectionSystemRef = FiringLockSelectionSystemRef;
            this.InteractiveObjectTargettedVisualFeedbackObject = new InteractiveObjectTargettedVisualFeedbackObject(
                new InteractiveObjectTargettedVisualFeedbackGameObject(null, PlayerAimingInteractiveObjectActionInherentData.InteractiveObjectTargettedVisualFeedbackPrefab),
                new InteractiveObjectTargettedVisualFeedbackObjectDefinition(PlayerAimingInteractiveObjectActionInherentData.InteractiveObjectTargettedVisualFeedbackAnimation));
            this.mainCamera = mainCamera;
            this.Tick(0f);
        }

        /// <summary>
        /// /!\ <see cref="InteractiveObjectTargettedVisualFeedback"/> must absolutely be updated after <see cref="FiringLockSelectionSystem"/> because the update of <see cref="FiringLockSelectionSystem"/> may change the result
        /// of <see cref="FiringLockSelectionSystem.GetCurrentlyTargettedInteractiveObject"/> (see <see cref="FiringLockSelectionSystem.Tick"/>)
        /// </summary>
        public void Tick(float d)
        {
            var currentlyTargettedInteractiveObject = this.FiringLockSelectionSystemRef.GetCurrentlyTargettedInteractiveObject();
            if (currentlyTargettedInteractiveObject == null)
            {
                this.InteractiveObjectTargettedVisualFeedbackObject.InteractiveObjectTargettedVisualFeedbackGameObject.SetWorldPosition(new Vector3(999999f, 9999999f, 99999999f));
            }
            else
            {
                var objTransform = currentlyTargettedInteractiveObject.InteractiveGameObject.GetTransform();

                this.InteractiveObjectTargettedVisualFeedbackObject.InteractiveObjectTargettedVisualFeedbackGameObject.SetWorldPosition(new Vector3(objTransform.WorldPosition.x, currentlyTargettedInteractiveObject.InteractiveGameObject.GetAverageModelWorldBounds().max.y, objTransform.WorldPosition.z));
                this.InteractiveObjectTargettedVisualFeedbackObject.InteractiveObjectTargettedVisualFeedbackGameObject.SetWorldRotation(Quaternion.LookRotation(this.mainCamera.transform.position - currentlyTargettedInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition));
            }
            
            this.InteractiveObjectTargettedVisualFeedbackObject.Tick(d);
        }

        public void Dispose()
        {
            if (this.InteractiveObjectTargettedVisualFeedbackObject != null)
            {
                this.InteractiveObjectTargettedVisualFeedbackObject.Destroy();
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
    /// The <see cref="IEM_IFiringAInteractiveObjectAction_EventsListener"/> is implement by any <see cref="CoreInteractiveObject"/> that wants to execute logic when
    /// the <see cref="PlayerAimingInteractiveObjectAction"/> is starting and stopping.
    /// PlayerAimingInteractiveObjectAction
    /// </summary>
    public interface IEM_IFiringAInteractiveObjectAction_EventsListener
    {
        void OnFiringInteractiveObjectActionStart(PlayerAimingInteractiveObjectActionInherentData playerAimingInteractiveObjectActionInherentData);
        void OnFiringInteractiveObjectActionEnd(PlayerAimingInteractiveObjectActionInherentData playerAimingInteractiveObjectActionInherentData);
    }
}