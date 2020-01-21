using System.Collections.Generic;
using Input;
using InteractiveObjectAction;
using PlayerDash;
using ProjectileDeflection;
using Skill;
using UnityEngine;

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
        private PlayerDashActionStateBehavior PlayerDashActionStateBehavior;

        private SkillSystem SkillSystemRef;

        public PlayerObjectInteractiveObjectActionStateManager(GameInputManager gameInputManager, InteractiveObjectActionPlayerSystem interactiveObjectActionPlayerSystem,
            SkillSystem SkillSystemRef,
            InteractiveObjectActionInherentData firingInteractiveObjectActionInherentData,
            ProjectileDeflectionTrackingInteractiveObjectActionInherentData projectileDeflectionTrackingInteractiveObjectActionInherentData,
            PlayerDashActionStateBehaviorInputDataSystemDefinition PlayerDashActionStateBehaviorInputDataSystemDefinition)
        {
            this.SkillSystemRef = SkillSystemRef;
            this.FiringInteractiveObjectActionStateBehavior = new FiringInteractiveObjectActionStateBehavior(gameInputManager, interactiveObjectActionPlayerSystem, firingInteractiveObjectActionInherentData);
            this.ProjectileDeflectionInteractiveObjectActionStateBehavior = new ProjectileDeflectionInteractiveObjectActionStateBehavior(projectileDeflectionTrackingInteractiveObjectActionInherentData, interactiveObjectActionPlayerSystem);
            this.PlayerDashActionStateBehavior = new PlayerDashActionStateBehavior(PlayerDashActionStateBehaviorInputDataSystemDefinition, interactiveObjectActionPlayerSystem);
        }

        public void Tick(float d)
        {
            this.FiringInteractiveObjectActionStateBehavior.Tick(d);
            this.ProjectileDeflectionInteractiveObjectActionStateBehavior.Tick(d);
            this.PlayerDashActionStateBehavior.Tick(d);
        }

        public void TickTimeFrozen(float d)
        {
            this.FiringInteractiveObjectActionStateBehavior.TickTimeFrozen(d);
            this.ProjectileDeflectionInteractiveObjectActionStateBehavior.TickTimeFrozen(d);
            this.PlayerDashActionStateBehavior.TickTimeFrozen(d);
        }

        #region External Events

        public void StopTargetting()
        {
            this.FiringInteractiveObjectActionStateBehavior.GetCurrentStateManager().StopTargetting();
        }

        public void OnLowOnHealthStarted()
        {
            /// We call the OnLowOnHealthStarted events only for skill slots that are constrainted in such a way.
            /// This is to prevent executing actions that are not intended.
            foreach (var lowHealthConstrainedSkills in this.SkillSystemRef.GetAllSkillSlotsThatAreLowOnHealthConstrainted())
            {
                if (lowHealthConstrainedSkills.CompareAssociatedInteractiveObjectActionInherentData(DeflectingProjectileInteractiveObjectAction.DeflectingProjectileInteractiveObjectActionUniqueID))
                {
                    this.ProjectileDeflectionInteractiveObjectActionStateBehavior.GetCurrentStateManager().OnLowOnHealthStarted();
                }
            }
        }

        public void OnLowOnHealthEnded()
        {
            this.ProjectileDeflectionInteractiveObjectActionStateBehavior.GetCurrentStateManager().OnLowOnHealthEnded();
        }

        public bool TryingToExecuteDashTeleportationAction()
        {
            return this.PlayerDashActionStateBehavior.TryingToExecuteDashTeleportationAction();
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

        public Vector3? GetPlayerDash_TargetPointWorldPosition()
        {
            return this.PlayerDashActionStateBehavior.GetTargetPointWorldPosition();
        }
    }

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