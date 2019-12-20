using System;
using InteractiveObjects;
using RangeObjects;
using UnityEngine;

namespace Health
{
    /// <summary>
    /// Responsible of recovering health by calling <see cref="CoreInteractiveObject.OnRecoverHealth"/> when an interactive object is in range of <see cref="RecoveringHealthRangeTrigger"/>.
    /// </summary>
    public class RecoveringHealthEmitterSystem
    {
        private RecoveringHealthEmitterSystemDefinition RecoveringHealthEmitterSystemDefinition;

        private RangeObjectV2 RecoveringHealthRangeTrigger;

        #region Callbacks

        /// <summary>
        /// This callback is called whenever health has been given to at leath one interactive object.
        /// </summary>
        private Action OnHealthRecoveredCallback;

        #endregion

        public RecoveringHealthEmitterSystem(CoreInteractiveObject AssociatedInteractiveObject, RecoveringHealthEmitterSystemDefinition RecoveringHealthEmitterSystemDefinition,
            Action OnHealthRecoveredCallback = null)
        {
            this.RecoveringHealthEmitterSystemDefinition = RecoveringHealthEmitterSystemDefinition;
            this.OnHealthRecoveredCallback = OnHealthRecoveredCallback;
            this.RecoveringHealthRangeTrigger = RangeObjectV2Builder.Build(AssociatedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent,
                RecoveringHealthEmitterSystemDefinition.RecveringHealthTriggerDefinition, AssociatedInteractiveObject,
                "RecoveringHealthRangeTrigger");
            this.RecoveringHealthRangeTrigger.RegisterPhysicsEventListener(new InteractiveObjectPhysicsEventListenerDelegated(this.PhysicsEventGuard,
                onTriggerEnterAction: this.OnTriggerEnter));
        }

        public void Tick(float d)
        {
            this.RecoveringHealthRangeTrigger.Tick(d);
        }

        public void Destroy()
        {
            this.RecoveringHealthRangeTrigger.OnDestroy();
        }

        private bool PhysicsEventGuard(InteractiveObjectPhysicsTriggerInfo InteractiveObjectPhysicsTriggerInfo)
        {
            return InteractiveObjectPhysicsTriggerInfo.GetOtherInteractiveObjectTag().IsTakingDamage;
        }

        private void OnTriggerEnter(CoreInteractiveObject CoreInteractiveObject)
        {
            CoreInteractiveObject.OnRecoverHealth(this.RecoveringHealthEmitterSystemDefinition.RecoveredHealth);
            this.OnHealthRecoveredCallback?.Invoke();
        }
    }
}