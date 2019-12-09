using System;
using Input;
using InteractiveObjects;
using ProjectileDeflection_Interface;
using UnityEngine;

namespace ProjectileDeflection
{
    /// <summary>
    /// Responsible of deflecting projectiles when conditions are met.
    /// </summary>
    public class ProjectileDeflectionSystem
    {
        #region External Dependencies

        private GameInputManager GameInputManager = GameInputManager.Get();
        private InteractiveObjectV2Manager InteractiveObjectV2Manager = InteractiveObjectV2Manager.Get();

        #endregion

        #region Callbacks

        private Action<ProjectileDeflectedPropertiesStruct> OnProjectileSuccessfullyDeflected;

        #endregion

        private ProjectileDeflectionActorDefinition _projectileDeflectionActorDefinition;

        private CoreInteractiveObject AssociatedInteractiveObject;

        /// <summary>
        /// The <see cref="ProjectileDeflectionSystem"/> must be updated at the earliest possible time.
        /// </summary>
        private bool UpdatedThisFrame;

        public ProjectileDeflectionSystem(CoreInteractiveObject associatedInteractiveObject, ProjectileDeflectionActorDefinition projectileDeflectionActorDefinition,
            Action<ProjectileDeflectedPropertiesStruct> OnProjectileSuccessfullyDeflected = null)
        {
            AssociatedInteractiveObject = associatedInteractiveObject;
            this._projectileDeflectionActorDefinition = projectileDeflectionActorDefinition;
            this.OnProjectileSuccessfullyDeflected = OnProjectileSuccessfullyDeflected;
            this.UpdatedThisFrame = false;
        }

        public void FixedTick(float d)
        {
            TickDeflection();
        }

        public void Tick(float d)
        {
            TickDeflection();
        }

        public void LateTick(float d)
        {
            this.UpdatedThisFrame = false;
        }

        private void TickDeflection()
        {
            if (!this.UpdatedThisFrame)
            {
                this.UpdatedThisFrame = true;
                if (GameInputManager.CurrentInput.DeflectProjectileDown())
                {
                    var overlappedColliders = Physics.OverlapSphere(this.AssociatedInteractiveObject.InteractiveGameObject.GetLogicColliderBoxDefinition().GetWorldCenter(), this._projectileDeflectionActorDefinition.ProjectileDetectionRadius);
                    if (overlappedColliders != null && overlappedColliders.Length > 0)
                    {
                        for (var i = 0; i < overlappedColliders.Length; i++)
                        {
                            Debug.DrawLine(this.AssociatedInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition, overlappedColliders[i].transform.position, Color.clear, 1f);
                            this.InteractiveObjectV2Manager.InteractiveObjectsIndexedByLogicCollider.TryGetValue(overlappedColliders[i], out CoreInteractiveObject overlappedInteractiveObject);
                            if (overlappedInteractiveObject != null && overlappedInteractiveObject.InteractiveObjectTag.IsDealingDamage)
                            {
                                /// Deflecting
                                /// /!\ We have to make sure that the projectile deflection check is called before the projectile position is updated. Otherwise,
                                /// the deflect will only be taken into account the next frame.
                                var InteractiveObjectDeflectionResult = overlappedInteractiveObject.OnInteractiveObjectAskingToBeDeflected(this.AssociatedInteractiveObject, out bool success);
                                if (success)
                                {
                                    this.OnProjectileSuccessfullyDeflected?.Invoke(InteractiveObjectDeflectionResult);
                                }
                            }
                        }
                    }
                }
            }
        }

        #region Data Retrieval

        public float GetProjectileDetectionRadius()
        {
            return this._projectileDeflectionActorDefinition.ProjectileDetectionRadius;
        }

        #endregion
    }
}