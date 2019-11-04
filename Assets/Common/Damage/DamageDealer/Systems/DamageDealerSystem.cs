using System;
using InteractiveObjects;
using UnityEngine;

namespace Damage
{
    public class DamageDealerSystem
    {
        private DamageDealerSystemDefinition DamageDealerSystemDefinition;

        public DamageDealerSystem(CoreInteractiveObject AssociatedInteractiveObject, DamageDealerSystemDefinition DamageDealerSystemDefinition)
        {
            this.DamageDealerSystemDefinition = DamageDealerSystemDefinition;
            AssociatedInteractiveObject.RegisterInteractiveObjectPhysicsEventListener(new DamageDealerSystemPhysicsListener(
                interactiveObjectSelectionGuard: (InteractiveObjectTag => InteractiveObjectTag.IsTakingDamage),
                onTriggerEnterAction: this.OnTriggerEnter
            ));
        }

        private void OnTriggerEnter(CoreInteractiveObject Other)
        {
            Debug.Log("DEAL DAMAGE");
            Other.DealDamage(this.DamageDealerSystemDefinition.Damage);
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