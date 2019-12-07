using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using Input;
using InteractiveObjects;
using UnityEngine;

namespace ProjectileDeflection
{
    public class ProjectileDeflectionSystem
    {
        #region External Dependencies

        private GameInputManager GameInputManager = GameInputManager.Get();
        private InteractiveObjectV2Manager InteractiveObjectV2Manager = InteractiveObjectV2Manager.Get();

        #endregion

        private CoreInteractiveObject AssociatedInteractiveObject;

        public ProjectileDeflectionSystem(CoreInteractiveObject associatedInteractiveObject)
        {
            AssociatedInteractiveObject = associatedInteractiveObject;
        }

        public void Tick(float d)
        {
            if (GameInputManager.CurrentInput.DeflectProjectileDown())
            {
                var overlappedColliders = Physics.OverlapSphere(this.AssociatedInteractiveObject.InteractiveGameObject.GetLogicColliderBoxDefinition().GetWorldCenter(), 10f);
                if (overlappedColliders != null && overlappedColliders.Length > 0)
                {
                    for (var i = 0; i < overlappedColliders.Length; i++)
                    {
                        this.InteractiveObjectV2Manager.InteractiveObjectsIndexedByLogicCollider.TryGetValue(overlappedColliders[i], out CoreInteractiveObject overlappedInteractiveObject);
                        if (overlappedInteractiveObject != null && overlappedInteractiveObject.InteractiveObjectTag.IsDealingDamage)
                        {
                            overlappedInteractiveObject.DeflectProjectile(this.AssociatedInteractiveObject);
                        }
                    }
                }
            }
        }
    }
}