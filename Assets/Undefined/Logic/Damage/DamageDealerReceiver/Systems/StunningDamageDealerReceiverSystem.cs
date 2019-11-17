using System;
using Health;

namespace Damage
{
    public class StunningDamageDealerReceiverSystem
    {
        private float StunnedTimer;
        [VE_Nested] private BoolVariable isStunned;
        public BoolVariable IsStunned => this.isStunned;
        private StunningDamageDealerReceiverSystemDefinition _stunningDamageDealerReceiverSystemDefinition;

        #region External Dependencies

        private HealthSystem HealthSystem;

        #endregion

        public StunningDamageDealerReceiverSystem(StunningDamageDealerReceiverSystemDefinition stunningDamageDealerReceiverSystemDefinition, HealthSystem healthSystem, Action OnStunningDamageDealerReceiverStartedAction = null,
            Action OnStunningDamageDealerReceiverEndedAction = null)
        {
            _stunningDamageDealerReceiverSystemDefinition = stunningDamageDealerReceiverSystemDefinition;
            HealthSystem = healthSystem;
            this.isStunned = new BoolVariable(false, OnStunningDamageDealerReceiverStartedAction, OnStunningDamageDealerReceiverEndedAction);
        }

        public void DealDamage(float Damage)
        {
            this.StunnedTimer = 0f;
            this.IsStunned.SetValue(true);
            this.HealthSystem.ChangeHealth(Damage);
        }

        public void Tick(float d)
        {
            this.StunnedTimer += d;
            if (this.StunnedTimer >= this._stunningDamageDealerReceiverSystemDefinition.StunTime)
            {
                this.IsStunned.SetValue(false);
            }
        }
    }
}