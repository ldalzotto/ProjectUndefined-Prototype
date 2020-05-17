﻿using System.Collections.Generic;
using InteractiveObjects;
using UnityEngine;

namespace RangeObjects
{
    public class RangeObjectV2PhysicsEventListenerComponent : MonoBehaviour
    {
        private CoreInteractiveObject AssociatedInteractiveObject;
        private List<Collider> CurrentlyTrackedColliders = new List<Collider>();
        private List<ARangeObjectV2PhysicsEventListener> PhysicsEventListeners = new List<ARangeObjectV2PhysicsEventListener>();

        public void Init(CoreInteractiveObject AssociatedInteractiveObject)
        {
            this.AssociatedInteractiveObject = AssociatedInteractiveObject;
        }

        public void AddPhysicsEventListener(ARangeObjectV2PhysicsEventListener ARangeObjectV2PhysicsEventListener)
        {
            this.PhysicsEventListeners.Add(ARangeObjectV2PhysicsEventListener);
        }

        public void OnTriggerEnter(Collider other)
        {
            InteractiveObjectV2Manager.Get().InteractiveObjectsIndexedByLogicCollider.TryGetValue(other, out CoreInteractiveObject OtherInteractiveObject);
            if (OtherInteractiveObject != null && ((this.AssociatedInteractiveObject != null && OtherInteractiveObject != this.AssociatedInteractiveObject) || this.AssociatedInteractiveObject == null))
            {
                this.CurrentlyTrackedColliders.Add(other);
                var RangeObjectPhysicsTriggerInfo = new RangeObjectPhysicsTriggerInfo {Other = other, OtherInteractiveObject = OtherInteractiveObject};
                for (var i = 0; i < this.PhysicsEventListeners.Count; i++)
                {
                    if (this.PhysicsEventListeners[i].ColliderSelectionGuard(RangeObjectPhysicsTriggerInfo))
                    {
                        this.PhysicsEventListeners[i].OnTriggerEnter(RangeObjectPhysicsTriggerInfo);
                    }
                }
            }
        }

        public void OnTriggerExit(Collider other)
        {
            InteractiveObjectV2Manager.Get().InteractiveObjectsIndexedByLogicCollider.TryGetValue(other, out CoreInteractiveObject OtherInteractiveObject);
            if (OtherInteractiveObject != null && ((this.AssociatedInteractiveObject != null && OtherInteractiveObject != this.AssociatedInteractiveObject) || this.AssociatedInteractiveObject == null))
            {
                this.CurrentlyTrackedColliders.Remove(other);
                var RangeObjectPhysicsTriggerInfo = new RangeObjectPhysicsTriggerInfo {Other = other, OtherInteractiveObject = OtherInteractiveObject};
                for (var i = 0; i < this.PhysicsEventListeners.Count; i++)
                {
                    if (this.PhysicsEventListeners[i].ColliderSelectionGuard(RangeObjectPhysicsTriggerInfo))
                    {
                        this.PhysicsEventListeners[i].OnTriggerExit(RangeObjectPhysicsTriggerInfo);
                    }
                }
            }
        }

        public void Destroy()
        {
            for (var i = this.CurrentlyTrackedColliders.Count - 1; i >= 0; i--)
            {
                this.OnTriggerExit(this.CurrentlyTrackedColliders[i]);
            }

            this.PhysicsEventListeners.Clear();
            this.CurrentlyTrackedColliders.Clear();
        }
    }
}