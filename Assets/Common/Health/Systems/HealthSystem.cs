using System;

namespace Health
{
    public class HealthSystem
    {
        public float CurrentHealth;
        private HealthSystemDefinition HealthSystemDefinition;

        public HealthSystem(HealthSystemDefinition HealthSystemDefinition, Action OnHealthBelowZeroAction)
        {
            this.HealthSystemDefinition = HealthSystemDefinition;
            this.CurrentHealth = this.HealthSystemDefinition.StartHealth;
            this.OnHealthBelowZeroAction = OnHealthBelowZeroAction;
        }

        private Action OnHealthBelowZeroAction;

        public void ChangeHealth(float HealthReduction)
        {
            this.CurrentHealth = this.CurrentHealth + HealthReduction;
            if (this.CurrentHealth <= 0)
            {
                this.OnHealthBelowZeroAction.Invoke();
            }
        }
    }
}