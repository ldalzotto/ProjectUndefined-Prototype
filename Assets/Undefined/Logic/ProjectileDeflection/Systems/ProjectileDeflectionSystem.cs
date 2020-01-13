using System;
using System.Collections.Generic;
using Input;
using InteractiveObjects;
using ProjectileDeflection_Interface;
using RangeObjects;
using UnityEngine;

namespace ProjectileDeflection
{
    /// <summary>
    /// Responsible of deflecting projectiles when conditions are met.
    /// The <see cref="ProjectileDeflectionSystem"/> is associated to an "Actor" (referenced by <see cref="AssociatedInteractiveObject"/>).
    /// It is up to the <see cref="ProjectileDeflectionSystem"/> to notify other <see cref="CoreInteractiveObject"/> that they have been deflected (by calling <see cref="CoreInteractiveObject.InteractiveObjectDeflected"/>
    /// to deflected projectiles.)
    /// Deflection trajectory calculations are handled by <see cref="DeflectionCalculations"/>.
    /// </summary>
    public class ProjectileDeflectionSystem
    {
        #region External Dependencies

        private GameInputManager GameInputManager = GameInputManager.Get();
        private InteractiveObjectV2Manager InteractiveObjectV2Manager = InteractiveObjectV2Manager.Get();

        #endregion

        private ProjectileDeflectionActorDefinition _projectileDeflectionActorDefinition;
        private CoreInteractiveObject AssociatedInteractiveObject;

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
        /// The <see cref="ProjectileDeflectionSystem"/> must be updated at the earliest possible time.
        /// </summary>
        private bool UpdatedThisFrame;

        public ProjectileDeflectionSystem(CoreInteractiveObject associatedInteractiveObject, ProjectileDeflectionActorDefinition projectileDeflectionActorDefinition)
        {
            AssociatedInteractiveObject = associatedInteractiveObject;
            this._projectileDeflectionActorDefinition = projectileDeflectionActorDefinition;
            this.ProjectileDeflectionFeedbackIconSystem = new ProjectileDeflectionFeedbackIconSystem(associatedInteractiveObject);
            this.ObjectsInsideDeflectionRangeSystem = new ObjectsInsideDeflectionRangeSystem(associatedInteractiveObject, this._projectileDeflectionActorDefinition,
                OnInteractiveObjectJusInsideAndFiltered: delegate(CoreInteractiveObject interactiveObject) { this.ProjectileDeflectionFeedbackIconSystem.OnInteractiveObjectJustInsideDeflectionRange(interactiveObject); },
                OnInteractiveObjectJustOutsideAndFiltered: delegate(CoreInteractiveObject interactiveObject) { this.ProjectileDeflectionFeedbackIconSystem.OnInteractiveObjectJustOutsideDeflectionRange(interactiveObject); });
            this.UpdatedThisFrame = false;
        }

        public void FixedTick(float d)
        {
            TickDeflection(d);
        }

        public void Tick(float d)
        {
            this.ObjectsInsideDeflectionRangeSystem.Tick(d);
            TickDeflection(d);
            this.ProjectileDeflectionFeedbackIconSystem.Tick(d);
        }

        public void TickTimeFrozen(float d)
        {
            this.ProjectileDeflectionFeedbackIconSystem.TickTimeFrozen(d);
        }

        public void LateTick(float d)
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

        public void Destroy()
        {
            this.ObjectsInsideDeflectionRangeSystem.Destroy();
        }

        #region Data Retrieval

        public float GetProjectileDetectionRadius()
        {
            return this._projectileDeflectionActorDefinition.ProjectileDetectionRadius;
        }

        #endregion

        #region External Events

        /// <summary>
        /// Projectile deflection is only needed when the health is low.
        /// </summary>
        public void OnLowHealthStarted()
        {
            this.ObjectsInsideDeflectionRangeSystem.OnLowHealthStarted();
        }

        /// <summary>
        /// Projectile deflection is no more needed when the health is no more low.
        /// </summary>
        public void OnLowHealthEnded()
        {
            this.ObjectsInsideDeflectionRangeSystem.OnLowHealthEnded();
        }

        #endregion

        public IEnumerable<CoreInteractiveObject> GetInsideDeflectableInteractiveObjects()
        {
            return this.ObjectsInsideDeflectionRangeSystem.GetInsideDeflectableInteractiveObjects();
        }
    }
}