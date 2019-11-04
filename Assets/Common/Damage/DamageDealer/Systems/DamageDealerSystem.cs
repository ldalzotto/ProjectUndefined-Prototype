﻿using System;
using InteractiveObjects;

namespace Damage
{
    public class DamageDealerSystem
    {
        private DamageDealerSystemDefinition DamageDealerSystemDefinition;
        private CoreInteractiveObject AssociatedInteractiveObjectRef;

        public DamageDealerSystem(CoreInteractiveObject AssociatedInteractiveObject, DamageDealerSystemDefinition DamageDealerSystemDefinition)
        {
            this.AssociatedInteractiveObjectRef = AssociatedInteractiveObject;
            this.DamageDealerSystemDefinition = DamageDealerSystemDefinition;
            AssociatedInteractiveObject.RegisterInteractiveObjectPhysicsEventListener(new DamageDealerSystemPhysicsListener(
                interactiveObjectSelectionGuard: (InteractiveObjectTag => InteractiveObjectTag.IsTakingDamage),
                onTriggerEnterAction: this.OnTriggerEnter
            ));
        }

        private void OnTriggerEnter(CoreInteractiveObject Other)
        {
            if (AssociatedInteractiveObjectRef != Other)
            {
                Other.DealDamage(-1 * this.DamageDealerSystemDefinition.Damage);
            }
        }
    }

    class DamageDealerSystemPhysicsListener : InteractiveObjectPhysicsEventListenerDelegated
    {
        public DamageDealerSystemPhysicsListener(Func<InteractiveObjectTag, bool> interactiveObjectSelectionGuard,
            Action<CoreInteractiveObject> onTriggerEnterAction = null, Action<CoreInteractiveObject> onTriggerExitAction = null) : base(interactiveObjectSelectionGuard, onTriggerEnterAction, onTriggerExitAction)
        {
        }
    }
}