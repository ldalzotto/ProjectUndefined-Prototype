using System;
using System.Collections.Generic;
using System.Linq;
using InteractiveObjects;
using UnityEngine;

namespace RangeObjects
{
    public class SphereOverlapRangeObject : SphereRangeObjectV2
    {
        private InteractiveObjectLocalContainerSystem InteractiveObjectLocalContainerSystem;

        #region Callbacks

        private Func<InteractiveObjectPhysicsTriggerInfo, bool> InteractiveObjectSelectionGuard;

        #endregion

        public SphereOverlapRangeObject(GameObject AssociatedGameObject, SphereRangeObjectInitialization SphereRangeObjectInitialization, CoreInteractiveObject AssociatedInteractiveObject,
            Func<InteractiveObjectPhysicsTriggerInfo, bool> interactiveObjectSelectionGuard, string objectName = "",
            Action<CoreInteractiveObject> OnInteractiveObjectJusInsideAndFiltered = null, Action<CoreInteractiveObject> OnInteractiveObjectJustOutsideAndFiltered = null)
            : base(AssociatedGameObject, SphereRangeObjectInitialization, AssociatedInteractiveObject, objectName)
        {
            this.InteractiveObjectSelectionGuard = interactiveObjectSelectionGuard;
            this.InteractiveObjectLocalContainerSystem = new InteractiveObjectLocalContainerSystem(OnInteractiveObjectJusInsideAndFiltered, OnInteractiveObjectJustOutsideAndFiltered);
            this.RegisterPhysicsEventListener(new InteractiveObjectPhysicsEventListenerDelegated(interactiveObjectSelectionGuard,
                onTriggerEnterAction: this.OnTriggerEnter, onTriggerExitAction: this.OnTriggerExit));
        }

        public void ManuallyDetectInsideColliders()
        {
            var overlappingCollider = Physics.OverlapSphere(this.RangeGameObjectV2.RangeGameObject.transform.localToWorldMatrix.MultiplyPoint(this.SphereBoundingCollider.center),
                this.SphereBoundingCollider.radius);
            if (overlappingCollider != null)
            {
                for (var i = 0; i < overlappingCollider.Length; i++)
                {
                    InteractiveObjectV2Manager.Get().InteractiveObjectsIndexedByLogicCollider.TryGetValue(overlappingCollider[i], out CoreInteractiveObject interactiveObject);
                    if (interactiveObject != null)
                    {
                        if (this.InteractiveObjectSelectionGuard.Invoke(new InteractiveObjectPhysicsTriggerInfo() {Other = overlappingCollider[i], OtherInteractiveObject = interactiveObject}))
                        {
                            this.OnTriggerEnter(interactiveObject);
                        }
                    }
                }
            }
        }

        private void OnTriggerEnter(CoreInteractiveObject interactiveObject)
        {
            this.InteractiveObjectLocalContainerSystem.AddInteractiveObject(interactiveObject);
        }

        private void OnTriggerExit(CoreInteractiveObject interactiveObject)
        {
            this.InteractiveObjectLocalContainerSystem.RemoveInteractiveObject(interactiveObject);
        }

        public override void OnDestroy()
        {
            this.InteractiveObjectLocalContainerSystem.OnDestroy();
            base.OnDestroy();
        }

        #region Data Retrieval

        public HashSet<CoreInteractiveObject> GetInsideInteractiveObjects()
        {
            return this.InteractiveObjectLocalContainerSystem.InsideInteractiveObjects;
        }

        #endregion
    }
}