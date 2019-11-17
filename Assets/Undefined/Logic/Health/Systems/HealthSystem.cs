using System;

namespace Health
{
    public class HealthSystem
    {
        private FloatVariable CurrentHealth;
        private HealthSystemDefinition HealthSystemDefinition;

        /// <summary>
        /// An event is created when <see cref="CurrentHealth"/> value change to allow multiple process to hook at value. 
        /// </summary>
        private event OnValueChangedDelegate OnHealthValueChangedEvent;

        public HealthSystem(HealthSystemDefinition HealthSystemDefinition, OnValueChangedDelegate OnHealthValueChangedAction = null)
        {
            this.HealthSystemDefinition = HealthSystemDefinition;

            /// By default, the constructor OnValueChangedDelegate is registered to OnValueChangedEvent
            if (OnHealthValueChangedAction != null)
            {
                this.OnHealthValueChangedEvent += OnHealthValueChangedAction;
            }

            this.CurrentHealth = new FloatVariable(this.HealthSystemDefinition.StartHealth, this.OnHealthValueChanged);
        }

        public void RegisterOnHealthValueChangedEventListener(OnValueChangedDelegate OnValueChangedDelegate)
        {
            this.OnHealthValueChangedEvent += OnValueChangedDelegate;
            /// Initialize the added event by manually calling it
            OnValueChangedDelegate.Invoke(this.CurrentHealth.GetValue(), this.CurrentHealth.GetValue());
        }

        private void OnHealthValueChanged(float OldValue, float newValue)
        {
            this.OnHealthValueChangedEvent?.Invoke(OldValue, newValue);
        }

        public void ChangeHealth(float HealthReduction)
        {
            this.CurrentHealth.SetValue(this.CurrentHealth.GetValue() + HealthReduction);
        }

        #region DataRetrieval

        public float GetMaxHealth()
        {
            return this.HealthSystemDefinition.StartHealth;
        }

        #endregion
    }
}