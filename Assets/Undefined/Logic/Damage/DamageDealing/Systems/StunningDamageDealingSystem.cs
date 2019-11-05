using System;
using Health;

namespace Damage
{
    public class StunningDamageDealingSystem
    {
        private float StunnedTimer;
        [VE_Nested] private BoolVariable isStunned;
        public BoolVariable IsStunned => this.isStunned;
        private StunningDamageDealingSystemDefinition StunningDamageDealingSystemDefinition;

        #region External Dependencies

        private HealthSystem HealthSystem;

        #endregion

        public StunningDamageDealingSystem(StunningDamageDealingSystemDefinition stunningDamageDealingSystemDefinition, HealthSystem healthSystem, Action OnStunningDamageDealingStartedAction,
            Action OnStunningDamageDealingEndedAction)
        {
            StunningDamageDealingSystemDefinition = stunningDamageDealingSystemDefinition;
            HealthSystem = healthSystem;
            this.isStunned = new BoolVariable(false, OnStunningDamageDealingStartedAction, OnStunningDamageDealingEndedAction);
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
            if (this.StunnedTimer >= this.StunningDamageDealingSystemDefinition.StunTime)
            {
                this.IsStunned.SetValue(false);
            }
        }
    }
}