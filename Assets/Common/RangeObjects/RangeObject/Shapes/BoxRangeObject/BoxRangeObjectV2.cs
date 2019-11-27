using System;
using System.Collections.Generic;
using GeometryIntersection;
using InteractiveObjects;
using UnityEngine;

namespace RangeObjects
{
    public class BoxRangeObjectV2 : RangeObjectV2
    {
        private BoxRangeObjectInitialization BoxRangeObjectInitialization;

        public BoxRangeObjectV2(GameObject AssociatedGameObject, BoxRangeObjectInitialization BoxRangeObjectInitialization, CoreInteractiveObject AssociatedInteractiveObject, string objectName = "")
        {
            this.RangeType = RangeType.BOX;
            this.BoxRangeObjectInitialization = BoxRangeObjectInitialization;
            var RangeGameObjectV2 = new RangeGameObjectV2(AssociatedGameObject, this.BoxRangeObjectInitialization, this, AssociatedInteractiveObject, objectName);
            base.Init(RangeGameObjectV2, BoxRangeObjectInitialization);
        }

        public BoxDefinition GetBoxBoundingColliderDefinition()
        {
            var BoxCollider = (BoxCollider) RangeGameObjectV2.BoundingCollider;
            return new BoxDefinition(BoxCollider);
        }

        public void SetLocalCenter(Vector3 localCenter)
        {
            var BoxCollider = (BoxCollider) RangeGameObjectV2.BoundingCollider;
            BoxCollider.center = localCenter;
        }

        public void SetLocalSize(Vector3 localSize)
        {
            var BoxCollider = (BoxCollider) RangeGameObjectV2.BoundingCollider;
            BoxCollider.size = localSize;
        }
    }

    /// <summary>
    /// Derived from <see cref="BoxRangeObjectV2"/>, it is designed to be used as an alternative of physics raycast and stores all
    /// insides interactive objects <see cref="InsideInteractiveObjects"/>.
    /// </summary>
    public class BoxRayRangeObject : BoxRangeObjectV2
    {
        private float RayWidth;
        public List<CoreInteractiveObject> InsideInteractiveObjects { get; private set; } = new List<CoreInteractiveObject>();

        public BoxRayRangeObject(GameObject AssociatedGameObject, BoxRangeObjectInitialization BoxRangeObjectInitialization, CoreInteractiveObject AssociatedInteractiveObject,
            float rayWidth, Func<InteractiveObjectPhysicsTriggerInfo, bool> InteractiveObjectSelectionGuard, string objectName = "")
            : base(AssociatedGameObject, BoxRangeObjectInitialization, AssociatedInteractiveObject, objectName)
        {
            this.RayWidth = rayWidth;
            this.RegisterPhysicsEventListener(new InteractiveObjectPhysicsEventListenerDelegated(
                InteractiveObjectSelectionGuard,
                onTriggerEnterAction: this.OnTriggerEnter,
                onTriggerExitAction: this.OnTriggerExit
            ));
        }

        public void UpdateRayDimensions(float Distance, bool Inverted = false)
        {
            if (Inverted)
            {
                Distance *= -1;
            }

            this.SetLocalCenter(new Vector3(0, 0, Distance * 0.5f));
            this.SetLocalSize(new Vector3(this.RayWidth, this.RayWidth, Distance));
        }

        private void OnTriggerEnter(CoreInteractiveObject interactiveObject)
        {
            this.InsideInteractiveObjects.Add(interactiveObject);
            interactiveObject.RegisterInteractiveObjectDestroyedEventListener(this.OnInteractiveObjectdestroyed);
        }

        private void OnTriggerExit(CoreInteractiveObject interactiveObject)
        {
            this.InsideInteractiveObjects.Remove(interactiveObject);
            interactiveObject.UnRegisterInteractiveObjectDestroyedEventListener(this.OnInteractiveObjectdestroyed);
        }

        private void OnInteractiveObjectdestroyed(CoreInteractiveObject interactiveObject)
        {
            this.InsideInteractiveObjects.Remove(interactiveObject);
            interactiveObject.UnRegisterInteractiveObjectDestroyedEventListener(this.OnInteractiveObjectdestroyed);
        }
    }
}