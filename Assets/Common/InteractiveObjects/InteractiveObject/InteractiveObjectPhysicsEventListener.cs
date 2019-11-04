using System;
using UnityEngine;

namespace InteractiveObjects
{
    public abstract class AInteractiveObjectPhysicsEventListener
    {
        ////// FIXED UPDATE ///////
        public virtual void OnTriggerEnter(InteractiveObjectPhysicsTriggerInfo PhysicsTriggerInfo)
        {
        }

        public virtual void OnTriggerExit(InteractiveObjectPhysicsTriggerInfo PhysicsTriggerInfo)
        {
        }

        public virtual bool ColliderSelectionGuard(InteractiveObjectPhysicsTriggerInfo interactiveObjectPhysicsTriggerInfo)
        {
            return true;
        }
    }

    public struct InteractiveObjectPhysicsTriggerInfo
    {
        public CoreInteractiveObject OtherInteractiveObject;
        public Collider Other;

        public InteractiveObjectTag GetOtherInteractiveObjectTag()
        {
            return this.OtherInteractiveObject.InteractiveObjectTag;
        }
    }

    public class InteractiveObjectPhysicsEventListenerDelegated : AInteractiveObjectPhysicsEventListener
    {
        protected Func<InteractiveObjectTag, bool> InteractiveObjectSelectionGuard;
        private Action<CoreInteractiveObject> OnTriggerEnterAction = null;
        private Action<CoreInteractiveObject> OnTriggerExitAction = null;

        public InteractiveObjectPhysicsEventListenerDelegated(Func<InteractiveObjectTag, bool> interactiveObjectSelectionGuard,
            Action<CoreInteractiveObject> onTriggerEnterAction = null, Action<CoreInteractiveObject> onTriggerExitAction = null)
        {
            OnTriggerEnterAction = onTriggerEnterAction;
            OnTriggerExitAction = onTriggerExitAction;
            InteractiveObjectSelectionGuard = interactiveObjectSelectionGuard;
        }

        public override bool ColliderSelectionGuard(InteractiveObjectPhysicsTriggerInfo interactiveObjectPhysicsTriggerInfo)
        {
            return InteractiveObjectSelectionGuard.Invoke(interactiveObjectPhysicsTriggerInfo.GetOtherInteractiveObjectTag());
        }

        public override void OnTriggerEnter(InteractiveObjectPhysicsTriggerInfo PhysicsTriggerInfo)
        {
            if (OnTriggerEnterAction != null) OnTriggerEnterAction.Invoke(PhysicsTriggerInfo.OtherInteractiveObject);
        }

        public override void OnTriggerExit(InteractiveObjectPhysicsTriggerInfo PhysicsTriggerInfo)
        {
            if (OnTriggerExitAction != null) OnTriggerExitAction.Invoke(PhysicsTriggerInfo.OtherInteractiveObject);
        }
    }
}