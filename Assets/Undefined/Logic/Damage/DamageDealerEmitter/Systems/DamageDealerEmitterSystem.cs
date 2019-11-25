using System;
using System.Collections.Generic;
using InteractiveObjects;

namespace Damage
{
    public class DamageDealerEmitterSystem
    {
        private DamageDealerEmitterSystemDefinition _damageDealerEmitterSystemDefinition;
        private CoreInteractiveObject AssociatedInteractiveObjectRef;

        private List<CoreInteractiveObject> IgnoredInteractiveObjects;
        private Action<CoreInteractiveObject> OnDamageDealtToOtherAction;

        public DamageDealerEmitterSystem(CoreInteractiveObject AssociatedInteractiveObject, DamageDealerEmitterSystemDefinition damageDealerEmitterSystemDefinition, Action<CoreInteractiveObject> OnDamageDealtToOtherAction = null,
            List<CoreInteractiveObject> IgnoredInteractiveObjects = null)
        {
            this.AssociatedInteractiveObjectRef = AssociatedInteractiveObject;
            this._damageDealerEmitterSystemDefinition = damageDealerEmitterSystemDefinition;
            this.OnDamageDealtToOtherAction = OnDamageDealtToOtherAction;
            this.IgnoredInteractiveObjects = IgnoredInteractiveObjects;
            AssociatedInteractiveObject.RegisterInteractiveObjectPhysicsEventListener(new DamageDealerSystemPhysicsListener(
                interactiveObjectSelectionGuard: this.TriggerSelectionGuard,
                onTriggerEnterAction: this.OnTriggerEnter
            ));
        }

        private bool TriggerSelectionGuard(InteractiveObjectPhysicsTriggerInfo InteractiveObjectPhysicsTriggerInfo)
        {
            return InteractiveObjectPhysicsTriggerInfo.GetOtherInteractiveObjectTag().IsTakingDamage
                   && (IgnoredInteractiveObjects == null || !IgnoredInteractiveObjects.Contains(InteractiveObjectPhysicsTriggerInfo.OtherInteractiveObject) );
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