using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using Input;
using InteractiveObjects;
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

        private ProjectileDeflectionDefinition ProjectileDeflectionDefinition;

        private CoreInteractiveObject AssociatedInteractiveObject;

        /// <summary>
        /// The <see cref="ProjectileDeflectionSystem"/> must be updated at the earliest possible time.
        /// </summary>
        private bool UpdatedThisFrame;

        public ProjectileDeflectionSystem(CoreInteractiveObject associatedInteractiveObject, ProjectileDeflectionDefinition ProjectileDeflectionDefinition)
        {
            AssociatedInteractiveObject = associatedInteractiveObject;
            this.ProjectileDeflectionDefinition = ProjectileDeflectionDefinition;
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
                    var overlappedColliders = Physics.OverlapSphere(this.AssociatedInteractiveObject.InteractiveGameObject.GetLogicColliderBoxDefinition().GetWorldCenter(), this.ProjectileDeflectionDefinition.ProjectileDetectionRadius);
                    if (overlappedColliders != null && overlappedColliders.Length > 0)
                    {
                        for (var i = 0; i < overlappedColliders.Length; i++)
                        {
                            this.InteractiveObjectV2Manager.InteractiveObjectsIndexedByLogicCollider.TryGetValue(overlappedColliders[i], out CoreInteractiveObject overlappedInteractiveObject);
                            if (overlappedInteractiveObject != null && overlappedInteractiveObject.InteractiveObjectTag.IsDealingDamage)
                            {
                                /// Deflecting
                                DeflectProjectile(overlappedInteractiveObject);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// /!\ We have to make sure that the projectile deflection check is called before the projectile position is updated. Otherwise,
        /// the deflect will only be taken into account the next frame.
        /// </summary>
        private void DeflectProjectile(CoreInteractiveObject overlappedInteractiveObject)
        {
            overlappedInteractiveObject.SwitchWeaponHolder(this.AssociatedInteractiveObject);
            overlappedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent.transform.forward = -overlappedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent.transform.forward;
        }

        #region Data Retrieval

        public float GetProjectileDetectionRadius()
        {
            return this.ProjectileDeflectionDefinition.ProjectileDetectionRadius;
        }

        #endregion
    }
}