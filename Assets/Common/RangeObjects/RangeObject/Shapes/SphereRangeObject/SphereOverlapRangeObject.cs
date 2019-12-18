using System;
using System.Collections.Generic;
using InteractiveObjects;
using UnityEngine;

namespace RangeObjects
{
    public class SphereOverlapRangeObject : SphereRangeObjectV2
    {
        private HashSet<CoreInteractiveObject> InsideInteractiveObjects = new HashSet<CoreInteractiveObject>();
        public List<CoreInteractiveObject> InsideInteractiveObjectsList { get; private set; } = new List<CoreInteractiveObject>();

        #region Callbacks

        private Func<InteractiveObjectPhysicsTriggerInfo, bool> InteractiveObjectSelectionGuard;
        private Action<CoreInteractiveObject> OnInteractiveObjectJusInsideAndFiltered;
        private Action<CoreInteractiveObject> OnInteractiveObjectJustOutsideAndFiltered;

        #endregion

        public SphereOverlapRangeObject(GameObject AssociatedGameObject, SphereRangeObjectInitialization SphereRangeObjectInitialization, CoreInteractiveObject AssociatedInteractiveObject,
            Func<InteractiveObjectPhysicsTriggerInfo, bool> interactiveObjectSelectionGuard, string objectName = "",
            Action<CoreInteractiveObject> OnInteractiveObjectJusInsideAndFiltered = null, Action<CoreInteractiveObject> OnInteractiveObjectJustOutsideAndFiltered = null)
            : base(AssociatedGameObject, SphereRangeObjectInitialization, AssociatedInteractiveObject, objectName)
        {
            this.InteractiveObjectSelectionGuard = interactiveObjectSelectionGuard;
            this.OnInteractiveObjectJusInsideAndFiltered = OnInteractiveObjectJusInsideAndFiltered;
            this.OnInteractiveObjectJustOutsideAndFiltered = OnInteractiveObjectJustOutsideAndFiltered;
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
            if (this.InsideInteractiveObjects.Add(interactiveObject))
            {
                interactiveObject.RegisterInteractiveObjectDestroyedEventListener(this.OnInteractiveObjectdestroyed);
                this.InsideInteractiveObjectsList.Add(interactiveObject);
                this.OnInteractiveObjectJusInsideAndFiltered?.Invoke(interactiveObject);
            }
        }

        private void OnTriggerExit(CoreInteractiveObject interactiveObject)
        {
            if (this.InsideInteractiveObjects.Remove(interactiveObject))
            {
                interactiveObject.UnRegisterInteractiveObjectDestroyedEventListener(this.OnInteractiveObjectdestroyed);
                this.InsideInteractiveObjectsList.Remove(interactiveObject);
                this.OnInteractiveObjectJustOutsideAndFiltered?.Invoke(interactiveObject);
            }
        }

        private void OnInteractiveObjectdestroyed(CoreInteractiveObject interactiveObject)
        {
            if (this.InsideInteractiveObjects.Remove(interactiveObject))
            {
                this.InsideInteractiveObjectsList.Remove(interactiveObject);
                this.OnInteractiveObjectJustOutsideAndFiltered?.Invoke(interactiveObject);
                interactiveObject.UnRegisterInteractiveObjectDestroyedEventListener(this.OnInteractiveObjectdestroyed);
            }
        }

        public override void OnDestroy()
        {
            for (var i = this.InsideInteractiveObjectsList.Count - 1; i >= 0; i--)
            {
                this.OnTriggerExit(this.InsideInteractiveObjectsList[i]);
            }

            base.OnDestroy();
        }
    }
}