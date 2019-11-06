using System;
using System.Collections.Generic;
using InteractiveObjects;

namespace Damage
{
    public class DamageDealerSystem
    {
        private DamageDealerSystemDefinition DamageDealerSystemDefinition;
        private CoreInteractiveObject AssociatedInteractiveObjectRef;

        private Action<CoreInteractiveObject> OnDamageDealtToOtherAction;

        public DamageDealerSystem(CoreInteractiveObject AssociatedInteractiveObject, DamageDealerSystemDefinition DamageDealerSystemDefinition, Action<CoreInteractiveObject> OnDamageDealtToOtherAction = null,
            List<CoreInteractiveObject> IgnoredInteractiveObjects = null)
        {
            this.AssociatedInteractiveObjectRef = AssociatedInteractiveObject;
            this.DamageDealerSystemDefinition = DamageDealerSystemDefinition;
            this.OnDamageDealtToOtherAction = OnDamageDealtToOtherAction;
            AssociatedInteractiveObject.RegisterInteractiveObjectPhysicsEventListener(new DamageDealerSystemPhysicsListener(
                interactiveObjectSelectionGuard: (InteractiveObjectPhysicsTriggerInfo =>
                    InteractiveObjectPhysicsTriggerInfo.GetOtherInteractiveObjectTag().IsTakingDamage
                    && IgnoredInteractiveObjects == null || !IgnoredInteractiveObjects.Contains(InteractiveObjectPhysicsTriggerInfo.OtherInteractiveObject)),
                onTriggerEnterAction: this.OnTriggerEnter
            ));
        }

        private void OnTriggerEnter(CoreInteractiveObject Other)
        {
            if (AssociatedInteractiveObjectRef != Other)
            {
                Other.DealDamage(-1 * this.DamageDealerSystemDefinition.Damage);
                this.OnDamageDealtToOtherAction?.Invoke(Other);
            }
        }
    }

    class DamageDealerSystemPhysicsListener : InteractiveObjectPhysicsEventListenerDelegated
    {
        public DamageDealerSystemPhysicsListener(Func<InteractiveObjectPhysicsTriggerInfo, bool> interactiveObjectSelectionGuard,
            Action<CoreInteractiveObject> onTriggerEnterAction = null, Action<CoreInteractiveObject> onTriggerExitAction = null) : base(interactiveObjectSelectionGuard, onTriggerEnterAction, onTriggerExitAction)
        {
        }
    }
}