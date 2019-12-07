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

        public ProjectileDeflectionSystem(CoreInteractiveObject associatedInteractiveObject, ProjectileDeflectionDefinition ProjectileDeflectionDefinition)
        {
            AssociatedInteractiveObject = associatedInteractiveObject;
            this.ProjectileDeflectionDefinition = ProjectileDeflectionDefinition;
        }

        public void Tick(float d)
        {
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

        private void DeflectProjectile(CoreInteractiveObject overlappedInteractiveObject)
        {
            overlappedInteractiveObject.SwitchWeaponHolder(this.AssociatedInteractiveObject);
            overlappedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent.transform.forward = -overlappedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent.transform.forward;
        }
    }
}