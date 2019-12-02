using System;
using System.Collections.Generic;
using InteractiveObjects;

namespace Damage
{
    public class DamageDealerEmitterSystem
    {
        private DamageDealerEmitterSystemDefinition _damageDealerEmitterSystemDefinition;
        private CoreInteractiveObject AssociatedInteractiveObjectRef;

        private Action<CoreInteractiveObject> OnDamageDealtToOtherAction;

        public DamageDealerEmitterSystem(CoreInteractiveObject AssociatedInteractiveObject, DamageDealerEmitterSystemDefinition damageDealerEmitterSystemDefinition, 
            Func<InteractiveObjectPhysicsTriggerInfo, bool> TriggerSelectionGuard = null,
            Action<CoreInteractiveObject> OnDamageDealtToOtherAction = null)
        {
            this.AssociatedInteractiveObjectRef = AssociatedInteractiveObject;
            this._damageDealerEmitterSystemDefinition = damageDealerEmitterSystemDefinition;
            this.OnDamageDealtToOtherAction = OnDamageDealtToOtherAction;
            AssociatedInteractiveObject.RegisterInteractiveObjectPhysicsEventListener(new DamageDealerSystemPhysicsListener(
                interactiveObjectSelectionGuard: TriggerSelectionGuard,
                onTriggerEnterAction: this.OnTriggerEnter
            ));
        }

        private void OnTriggerEnter(CoreInteractiveObject Other)
        {
            if (AssociatedInteractiveObjectRef != Other)
            {
                Other.DealDamage(-1 * this._damageDealerEmitterSystemDefinition.Damage, this.AssociatedInteractiveObjectRef);
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