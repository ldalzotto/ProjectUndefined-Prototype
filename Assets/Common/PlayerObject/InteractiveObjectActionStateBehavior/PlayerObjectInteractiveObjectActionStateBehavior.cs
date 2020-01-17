using System.Collections.Generic;
using Input;
using InteractiveObjectAction;
using ProjectileDeflection;

namespace PlayerObject
{
    /// <summary>
    /// The <see cref="PlayerObjectInteractiveObjectActionStateManager"/> is responsible of executing and stopping InteractiveObjectAction that aren't skills
    /// (by calling the <see cref="InteractiveObjectActionPlayerSystem"/>) based on some conditions.
    /// It acts as a Layer between the <see cref="PlayerInteractiveObject"/> and the <see cref="InteractiveObjectActionPlayerSystem"/> by introducing some logical condition between.
    /// </summary>
    public class PlayerObjectInteractiveObjectActionStateManager
    {
        private FiringInteractiveObjectActionStateBehavior FiringInteractiveObjectActionStateBehavior;
        private ProjectileDeflectionInteractiveObjectActionStateBehavior ProjectileDeflectionInteractiveObjectActionStateBehavior;

        public PlayerObjectInteractiveObjectActionStateManager(GameInputManager gameInputManager, InteractiveObjectActionPlayerSystem interactiveObjectActionPlayerSystem,
            InteractiveObjectActionInherentData firingInteractiveObjectActionInherentData, ProjectileDeflectionTrackingInteractiveObjectActionInherentData projectileDeflectionTrackingInteractiveObjectActionInherentData)
        {
            this.FiringInteractiveObjectActionStateBehavior = new FiringInteractiveObjectActionStateBehavior(gameInputManager, interactiveObjectActionPlayerSystem, firingInteractiveObjectActionInherentData);
            this.ProjectileDeflectionInteractiveObjectActionStateBehavior = new ProjectileDeflectionInteractiveObjectActionStateBehavior(projectileDeflectionTrackingInteractiveObjectActionInherentData, interactiveObjectActionPlayerSystem);
        }

        public void Tick(float d)
        {
            this.FiringInteractiveObjectActionStateBehavior.Tick(d);
            this.ProjectileDeflectionInteractiveObjectActionStateBehavior.Tick(d);
        }

        public void TickTimeFrozen(float d)
        {
            this.FiringInteractiveObjectActionStateBehavior.TickTimeFrozen(d);
        }

        #region External Events

        public void StopTargetting()
        {
            this.FiringInteractiveObjectActionStateBehavior.GetCurrentStateManager().StopTargetting();
        }

        public void OnLowOnHealthStarted()
        {
            this.ProjectileDeflectionInteractiveObjectActionStateBehavior.GetCurrentStateManager().OnLowOnHealthStarted();
        }

        public void OnLowOnHealthEnded()
        {
            this.ProjectileDeflectionInteractiveObjectActionStateBehavior.GetCurrentStateManager().OnLowOnHealthEnded();
        }

        #endregion

        #region Logical conditions

        public bool IsAiming()
        {
            return this.FiringInteractiveObjectActionStateBehavior.IsAiming();
        }

        public bool IsTrackingNearDeflectableObjects()
        {
            return this.ProjectileDeflectionInteractiveObjectActionStateBehavior.IsTrackingNearDeflectableObjects();
        }

        #endregion
    }

    #region Aiming

    public enum AimingInteractiveObjectActionState
    {
        LISTENING,
        AIMING
    }

    public abstract class AAimingInteractiveObjectActionStateManager : StateManager
    {
        public virtual void StopTargetting()
        {
        }
    }

    public class ListeningAimingInteractiveObjectActionStateManager : AAimingInteractiveObjectActionStateManager
    {
        private FiringInteractiveObjectActionStateBehavior FiringInteractiveObjectActionStateBehavior;
        private AimingInteractiveObjectActionStateBehaviorInputDataSystem _aimingInteractiveObjectActionStateBehaviorInputDataSystemRef;

        public ListeningAimingInteractiveObjectActionStateManager(FiringInteractiveObjectActionStateBehavior FiringInteractiveObjectActionStateBehaviorRef,
            ref AimingInteractiveObjectActionStateBehaviorInputDataSystem aimingInteractiveObjectActionStateBehaviorInputDataSystemRef)
        {
            this.FiringInteractiveObjectActionStateBehavior = FiringInteractiveObjectActionStateBehaviorRef;
            this._aimingInteractiveObjectActionStateBehaviorInputDataSystemRef = aimingInteractiveObjectActionStateBehaviorInputDataSystemRef;
        }

        public override void Tick(float d)
        {
            base.Tick(d);
            this.PlayerActionTriggering();
        }

        public override void TickTimeFrozen(float d)
        {
            base.TickTimeFrozen(d);
            this.Tick(d);
        }

        /// <summary>
        /// Starts a new <see cref="AInteractiveObjectAction"/> if input condition and player inherent conditions are met.
        /// </summary>
        private void PlayerActionTriggering()
        {
            if (this._aimingInteractiveObjectActionStateBehaviorInputDataSystemRef.GameInputManager.CurrentInput.FiringActionDown())
            {
                this._aimingInteractiveObjectActionStateBehaviorInputDataSystemRef.InteractiveObjectActionPlayerSystem.ExecuteActionV2
                    (this._aimingInteractiveObjectActionStateBehaviorInputDataSystemRef.firingInteractiveObjectActionInherentData);
                this.FiringInteractiveObjectActionStateBehavior.SetState(AimingInteractiveObjectActionState.AIMING);
            }
        }
    }

    public class AimingAimingInteractiveObjectActionStateManager : AAimingInteractiveObjectActionStateManager
    {
        private FiringInteractiveObjectActionStateBehavior FiringInteractiveObjectActionStateBehavior;

        public AimingAimingInteractiveObjectActionStateManager(FiringInteractiveObjectActionStateBehavior firingInteractiveObjectActionStateBehavior)
        {
            FiringInteractiveObjectActionStateBehavior = firingInteractiveObjectActionStateBehavior;
        }

        public override void StopTargetting()
        {
            this.FiringInteractiveObjectActionStateBehavior.SetState(AimingInteractiveObjectActionState.LISTENING);
        }
    }

    public struct AimingInteractiveObjectActionStateBehaviorInputDataSystem
    {
        public GameInputManager GameInputManager;
        public InteractiveObjectActionPlayerSystem InteractiveObjectActionPlayerSystem;
        public InteractiveObjectActionInherentData firingInteractiveObjectActionInherentData;

        public AimingInteractiveObjectActionStateBehaviorInputDataSystem(GameInputManager gameInputManager, InteractiveObjectActionPlayerSystem interactiveObjectActionPlayerSystem,
            InteractiveObjectActionInherentData firingInteractiveObjectActionInherentData)
        {
            GameInputManager = gameInputManager;
            InteractiveObjectActionPlayerSystem = interactiveObjectActionPlayerSystem;
            this.firingInteractiveObjectActionInherentData = firingInteractiveObjectActionInherentData;
        }
    }

    public class FiringInteractiveObjectActionStateBehavior : StateBehavior<AimingInteractiveObjectActionState, AAimingInteractiveObjectActionStateManager>
    {
        private AimingInteractiveObjectActionStateBehaviorInputDataSystem _aimingInteractiveObjectActionStateBehaviorInputDataSystem;

        public FiringInteractiveObjectActionStateBehavior(GameInputManager gameInputManager, InteractiveObjectActionPlayerSystem interactiveObjectActionPlayerSystem,
            InteractiveObjectActionInherentData firingInteractiveObjectActionInherentData)
        {
            this._aimingInteractiveObjectActionStateBehaviorInputDataSystem = new AimingInteractiveObjectActionStateBehaviorInputDataSystem(
                gameInputManager, interactiveObjectActionPlayerSystem, firingInteractiveObjectActionInherentData);
            this.StateManagersLookup = new Dictionary<AimingInteractiveObjectActionState, AAimingInteractiveObjectActionStateManager>()
            {
                {AimingInteractiveObjectActionState.LISTENING, new ListeningAimingInteractiveObjectActionStateManager(this, ref this._aimingInteractiveObjectActionStateBehaviorInputDataSystem)},
                {AimingInteractiveObjectActionState.AIMING, new AimingAimingInteractiveObjectActionStateManager(this)}
            };
            base.Init(AimingInteractiveObjectActionState.LISTENING);
        }

        protected override void Init(AimingInteractiveObjectActionState StartState)
        {
            base.Init(StartState);
        }

        public bool IsAiming()
        {
            return this.GetCurrentState() == AimingInteractiveObjectActionState.AIMING;
        }
    }

    #endregion

    #region Projectile Deflection

    public enum ProjectileDeflectionInteractiveObjectActionState
    {
        LISTENING,
        TRACKING_NEAR_DEFLECTABLE_OBJECTS
    }

    public abstract class ProjectileDeflectionInteractiveObjectActionStateManager : StateManager
    {
        public virtual void OnLowOnHealthStarted()
        {
        }

        public virtual void OnLowOnHealthEnded()
        {
        }
    }

    public class ListeningProjectileDeflectionInteractiveObjectActionStateManager : ProjectileDeflectionInteractiveObjectActionStateManager
    {
        private ProjectileDeflectionInteractiveObjectActionStateBehavior ProjectileDeflectionInteractiveObjectActionStateBehavior;

        public ListeningProjectileDeflectionInteractiveObjectActionStateManager(ProjectileDeflectionInteractiveObjectActionStateBehavior projectileDeflectionInteractiveObjectActionStateBehavior)
        {
            ProjectileDeflectionInteractiveObjectActionStateBehavior = projectileDeflectionInteractiveObjectActionStateBehavior;
        }

        public override void OnLowOnHealthStarted()
        {
            this.ProjectileDeflectionInteractiveObjectActionStateBehavior.SetState(ProjectileDeflectionInteractiveObjectActionState.TRACKING_NEAR_DEFLECTABLE_OBJECTS);
        }
    }

    public class TrackingNearDeflectableObject_ProjectileDeflectionInteractiveObjectActionStateManager : ProjectileDeflectionInteractiveObjectActionStateManager
    {
        private ProjectileDeflectionInteractiveObjectActionStateBehavior ProjectileDeflectionInteractiveObjectActionStateBehavior;
        private ProjectileDeflectionTrackingInteractiveObjectActionInherentData _projectileDeflectionTrackingInteractiveObjectActionInherentData;
        private InteractiveObjectActionPlayerSystem InteractiveObjectActionPlayerSystem;

        public TrackingNearDeflectableObject_ProjectileDeflectionInteractiveObjectActionStateManager(ProjectileDeflectionInteractiveObjectActionStateBehavior ProjectileDeflectionInteractiveObjectActionStateBehavior,
            ProjectileDeflectionTrackingInteractiveObjectActionInherentData projectileDeflectionTrackingInteractiveObjectActionInherentData,
            InteractiveObjectActionPlayerSystem interactiveObjectActionPlayerSystem)
        {
            this.ProjectileDeflectionInteractiveObjectActionStateBehavior = ProjectileDeflectionInteractiveObjectActionStateBehavior;
            _projectileDeflectionTrackingInteractiveObjectActionInherentData = projectileDeflectionTrackingInteractiveObjectActionInherentData;
            InteractiveObjectActionPlayerSystem = interactiveObjectActionPlayerSystem;
        }

        public override void OnStateEnter()
        {
            base.OnStateEnter();
            this.InteractiveObjectActionPlayerSystem.ExecuteActionV2(this._projectileDeflectionTrackingInteractiveObjectActionInherentData);
        }

        public override void OnLowOnHealthEnded()
        {
            this.InteractiveObjectActionPlayerSystem.StopAction(_projectileDeflectionTrackingInteractiveObjectActionInherentData.InteractiveObjectActionUniqueID);
            this.ProjectileDeflectionInteractiveObjectActionStateBehavior.SetState(ProjectileDeflectionInteractiveObjectActionState.LISTENING);
        }
    }

    public class ProjectileDeflectionInteractiveObjectActionStateBehavior : StateBehavior<ProjectileDeflectionInteractiveObjectActionState, ProjectileDeflectionInteractiveObjectActionStateManager>
    {
        public ProjectileDeflectionInteractiveObjectActionStateBehavior(ProjectileDeflectionTrackingInteractiveObjectActionInherentData projectileDeflectionTrackingInteractiveObjectActionInherentData,
            InteractiveObjectActionPlayerSystem interactiveObjectActionPlayerSystem)
        {
            this.StateManagersLookup = new Dictionary<ProjectileDeflectionInteractiveObjectActionState, ProjectileDeflectionInteractiveObjectActionStateManager>()
            {
                {ProjectileDeflectionInteractiveObjectActionState.LISTENING, new ListeningProjectileDeflectionInteractiveObjectActionStateManager(this)},
                {
                    ProjectileDeflectionInteractiveObjectActionState.TRACKING_NEAR_DEFLECTABLE_OBJECTS, new TrackingNearDeflectableObject_ProjectileDeflectionInteractiveObjectActionStateManager(this,
                        projectileDeflectionTrackingInteractiveObjectActionInherentData, interactiveObjectActionPlayerSystem)
                }
            };

            base.Init(ProjectileDeflectionInteractiveObjectActionState.LISTENING);
        }

        public bool IsTrackingNearDeflectableObjects()
        {
            return this.GetCurrentState() == ProjectileDeflectionInteractiveObjectActionState.TRACKING_NEAR_DEFLECTABLE_OBJECTS;
        }
    }

    #endregion
}