using System;
using InteractiveObjects;
using UnityEngine;

namespace RangeObjects
{
    public struct RangeObjectPhysicsTriggerInfo
    {
        public CoreInteractiveObject OtherInteractiveObject;
        public Collider Other;
    }

    public abstract class ARangeObjectV2PhysicsEventListener
    {
        ////// FIXED UPDATE ///////
        public virtual void OnTriggerEnter(RangeObjectPhysicsTriggerInfo PhysicsTriggerInfo)
        {
        }

        public virtual void OnTriggerExit(RangeObjectPhysicsTriggerInfo PhysicsTriggerInfo)
        {
        }

        public virtual bool ColliderSelectionGuard(RangeObjectPhysicsTriggerInfo RangeObjectPhysicsTriggerInfo)
        {
            return true;
        }
    }

    public class RangeObjectV2PhysicsEventListener_Delegated : ARangeObjectV2PhysicsEventListener
    {
        protected InteractiveObjectTagStruct InteractiveObjectSelectionGuard;
        private Action<CoreInteractiveObject> OnTriggerEnterAction = null;
        private Action<CoreInteractiveObject> OnTriggerExitAction = null;

        public RangeObjectV2PhysicsEventListener_Delegated(InteractiveObjectTagStruct interactiveObjectSelectionGuard,
            Action<CoreInteractiveObject> onTriggerEnterAction = null, Action<CoreInteractiveObject> onTriggerExitAction = null)
        {
            OnTriggerEnterAction = onTriggerEnterAction;
            OnTriggerExitAction = onTriggerExitAction;
            InteractiveObjectSelectionGuard = interactiveObjectSelectionGuard;
        }

        public override bool ColliderSelectionGuard(RangeObjectPhysicsTriggerInfo RangeObjectPhysicsTriggerInfo)
        {
            return InteractiveObjectSelectionGuard.Compare(RangeObjectPhysicsTriggerInfo.OtherInteractiveObject.InteractiveObjectTag);
        }

        public override void OnTriggerEnter(RangeObjectPhysicsTriggerInfo PhysicsTriggerInfo)
        {
            if (OnTriggerEnterAction != null) OnTriggerEnterAction.Invoke(PhysicsTriggerInfo.OtherInteractiveObject);
        }

        public override void OnTriggerExit(RangeObjectPhysicsTriggerInfo PhysicsTriggerInfo)
        {
            if (OnTriggerExitAction != null) OnTriggerExitAction.Invoke(PhysicsTriggerInfo.OtherInteractiveObject);
        }
    }
}