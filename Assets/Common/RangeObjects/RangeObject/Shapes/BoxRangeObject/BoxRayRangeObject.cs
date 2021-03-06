﻿using System;
using System.Collections.Generic;
using GeometryIntersection;
using InteractiveObjects;
using UnityEngine;

namespace RangeObjects
{
    /// <summary>
    /// Derived from <see cref="BoxRangeObjectV2"/>, it is designed to be used as an alternative of physics raycast and stores all
    /// insides interactive objects <see cref="InsideInteractiveObjects"/>.
    /// </summary>
    public class BoxRayRangeObject : BoxRangeObjectV2
    {
        private float RayWidth;

        private InteractiveObjectLocalContainerSystem InteractiveObjectLocalContainerSystem;
        
        private Func<InteractiveObjectPhysicsTriggerInfo, bool> InteractiveObjectSelectionGuard;

        public BoxRayRangeObject(GameObject AssociatedGameObject, BoxRangeObjectInitialization BoxRangeObjectInitialization, CoreInteractiveObject AssociatedInteractiveObject,
            float rayWidth, Func<InteractiveObjectPhysicsTriggerInfo, bool> InteractiveObjectSelectionGuard, string objectName = "")
            : base(AssociatedGameObject, BoxRangeObjectInitialization, AssociatedInteractiveObject, objectName)
        {
            this.InteractiveObjectSelectionGuard = InteractiveObjectSelectionGuard;
            this.InteractiveObjectLocalContainerSystem = new InteractiveObjectLocalContainerSystem(null, null);
            this.RayWidth = rayWidth;
            this.RegisterPhysicsEventListener(new InteractiveObjectPhysicsEventListenerDelegated(
                InteractiveObjectSelectionGuard,
                onTriggerEnterAction: this.OnTriggerEnter,
                onTriggerExitAction: this.OnTriggerExit
            ));
        }

        public void ManuallyDetectInsideColliders()
        {
            var overlappingCollider = PhysicsIntersection.BoxOverlapColliderWorld(this.RangeGameObjectV2.BoundingCollider as BoxCollider);
            if (overlappingCollider != null)
            {
                for (var i = 0; i < overlappingCollider.Length; i++)
                {
                    InteractiveObjectV2Manager.Get().InteractiveObjectsIndexedByLogicCollider.TryGetValue(overlappingCollider[i], out CoreInteractiveObject interactiveObject);
                    if (interactiveObject != null)
                    {
                        if (this.InteractiveObjectSelectionGuard.Invoke(new InteractiveObjectPhysicsTriggerInfo() {Other = overlappingCollider[i], OtherInteractiveObject = interactiveObject}))
                        {
                            //  Debug.Log(MyLog.Format(interactiveObject.InteractiveGameObject.GetAssociatedGameObjectName()));
                            this.OnTriggerEnter(interactiveObject);
                        }
                    }
                }
            }
        }


        public void UpdateRayDimensions(float Distance, bool Inverted = false)
        {
            if (Inverted)
            {
                this.SetLocalCenter(new Vector3(0, 0, -1 * Distance * 0.5f));
            }
            else
            {
                this.SetLocalCenter(new Vector3(0, 0, Distance * 0.5f));
            }


            this.SetLocalSize(new Vector3(this.RayWidth, this.RayWidth, Distance));
        }

        private void OnTriggerEnter(CoreInteractiveObject interactiveObject)
        {
            this.InteractiveObjectLocalContainerSystem.AddInteractiveObject(interactiveObject);
        }

        private void OnTriggerExit(CoreInteractiveObject interactiveObject)
        {
           this.InteractiveObjectLocalContainerSystem.RemoveInteractiveObject(interactiveObject);
        }
        
        public void Disable()
        {
            if (this.RangeGameObjectV2.RangeGameObject.activeSelf)
            {
                this.RangeGameObjectV2.RangeGameObject.SetActive(false);
                this.InteractiveObjectLocalContainerSystem.OnDestroy();
            }
        }

        /// <summary>
        /// /!\ <see cref="Enable"/> doesn't manually update <see cref="InsideInteractiveObjects"/> (<seealso cref="ManuallyDetectInsideColliders"/>)
        /// </summary>
        public void Enable()
        {
            if (!this.RangeGameObjectV2.RangeGameObject.activeSelf)
            {
                this.RangeGameObjectV2.RangeGameObject.SetActive(true);
            }
        }

        #region Data Retrieval

        public HashSet<CoreInteractiveObject> GetInsideInteractiveObjects()
        {
            return this.InteractiveObjectLocalContainerSystem.InsideInteractiveObjects;
        }

        #endregion
    }
}