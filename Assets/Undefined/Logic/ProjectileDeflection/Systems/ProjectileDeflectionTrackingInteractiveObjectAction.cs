using System;
using System.Collections.Generic;
using Input;
using InteractiveObjectAction;
using InteractiveObjects;
using ProjectileDeflection_Interface;
using RangeObjects;
using UnityEngine;

namespace ProjectileDeflection
{
    public struct ProjectileDeflectionSystemInput : IInteractiveObjectActionInput
    {
        public ProjectileDeflectionTrackingInteractiveObjectActionInherentData ProjectileDeflectionTrackingInteractiveObjectActionInherentData;
        public CoreInteractiveObject AssociatedInteractiveObject;

        public ProjectileDeflectionSystemInput(ProjectileDeflectionTrackingInteractiveObjectActionInherentData projectileDeflectionTrackingInteractiveObjectActionInherentData, CoreInteractiveObject associatedInteractiveObject)
        {
            this.ProjectileDeflectionTrackingInteractiveObjectActionInherentData = projectileDeflectionTrackingInteractiveObjectActionInherentData;
            AssociatedInteractiveObject = associatedInteractiveObject;
        }
    }

    /// <summary>
    /// Responsible of :
    ///  * Tracking projectiles in range of deflection <see cref="ProjectileDeflectionFeedbackIconSystem"/> 
    ///  * Creating the feedback icon on projectiles in are deflectable <see cref="ProjectileDeflectionFeedbackIconSystem"/>
    /// </summary>
    public class ProjectileDeflectionTrackingInteractiveObjectAction : AInteractiveObjectAction
    {
        public static string ProjectileDeflectionSystemUniqueID = "ProjectileDeflectionTrackingInteractiveObjectAction";

        public override string InteractiveObjectActionUniqueID
        {
            get { return ProjectileDeflectionSystemUniqueID; }
        }

        #region External Dependencies

        private GameInputManager GameInputManager = GameInputManager.Get();
        private InteractiveObjectV2Manager InteractiveObjectV2Manager = InteractiveObjectV2Manager.Get();

        #endregion

        private ProjectileDeflectionSystemInput ProjectileDeflectionSystemInput;

        /// <summary>
        /// When the Player try to deflect projectiles, object deflection results are stored temporaly to be processed after the deflect step. <see cref="ProcessDeflectionResults"/>
        /// /!\ This is really important because the events called in the processing of events may lead to modification of <see cref="ObjectsInsideDeflectionRangeSystem"/> while <see cref="ComputeDeflectedInteractiveObject"/>
        /// is already iterating over interactive objects inside <see cref="ObjectsInsideDeflectionRangeSystem"/>.
        /// </summary>
        private List<CoreInteractiveObject> SuccessfullyProjectileDeflectedPropertiesBuffered = new List<CoreInteractiveObject>();

        #region Systems

        private ProjectileDeflectionFeedbackIconSystem ProjectileDeflectionFeedbackIconSystem;
        private ObjectsInsideDeflectionRangeSystem ObjectsInsideDeflectionRangeSystem;

        #endregion

        /// <summary>
        /// The <see cref="ProjectileDeflectionTrackingInteractiveObjectAction"/> must be updated at the earliest possible time.
        /// </summary>
        private bool UpdatedThisFrame;

        public ProjectileDeflectionTrackingInteractiveObjectAction(ProjectileDeflectionSystemInput ProjectileDeflectionSystemInput)
            : base(new CoreInteractiveObjectActionDefinition())
        {
            this.ProjectileDeflectionSystemInput = ProjectileDeflectionSystemInput;
            this.ProjectileDeflectionFeedbackIconSystem = new ProjectileDeflectionFeedbackIconSystem(this.ProjectileDeflectionSystemInput.AssociatedInteractiveObject);
            this.ObjectsInsideDeflectionRangeSystem = new ObjectsInsideDeflectionRangeSystem(this.ProjectileDeflectionSystemInput.AssociatedInteractiveObject, this.ProjectileDeflectionSystemInput.ProjectileDeflectionTrackingInteractiveObjectActionInherentData,
                OnInteractiveObjectJusInsideAndFiltered: delegate(CoreInteractiveObject interactiveObject) { this.ProjectileDeflectionFeedbackIconSystem.OnInteractiveObjectJustInsideDeflectionRange(interactiveObject); },
                OnInteractiveObjectJustOutsideAndFiltered: delegate(CoreInteractiveObject interactiveObject) { this.ProjectileDeflectionFeedbackIconSystem.OnInteractiveObjectJustOutsideDeflectionRange(interactiveObject); });
            this.UpdatedThisFrame = false;
        }

        public override void FixedTick(float d)
        {
            TickDeflection(d);
        }

        public override void Tick(float d)
        {
            this.ObjectsInsideDeflectionRangeSystem.Tick(d);
            TickDeflection(d);
            this.ProjectileDeflectionFeedbackIconSystem.Tick(d);
        }

        public override void TickTimeFrozen(float d)
        {
            this.ProjectileDeflectionFeedbackIconSystem.TickTimeFrozen(d);
        }

        public override void LateTick(float d)
        {
            this.UpdatedThisFrame = false;
        }

        /// <summary>
        /// /!\ We have to make sure that the projectile deflection check is called before the projectile position is updated. Otherwise,
        /// the deflect will only be taken into account the next frame.
        /// </summary>
        private void TickDeflection(float d)
        {
            if (!this.UpdatedThisFrame)
            {
                this.UpdatedThisFrame = true;

                this.ObjectsInsideDeflectionRangeSystem.Tick(d);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            this.ObjectsInsideDeflectionRangeSystem.Destroy();
        }

        public IEnumerable<CoreInteractiveObject> GetInsideDeflectableInteractiveObjects()
        {
            return this.ObjectsInsideDeflectionRangeSystem.GetInsideDeflectableInteractiveObjects();
        }
    }
}