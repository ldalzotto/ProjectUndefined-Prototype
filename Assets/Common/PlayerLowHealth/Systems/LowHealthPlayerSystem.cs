using System;
using Health;
using InteractiveObjects;
using InteractiveObjects_Interfaces;

namespace PlayerLowHealth
{
    public class LowHealthPlayerSystem
    {
        private LowHealthPlayerSystemDefinition LowHealthPlayerSystemDefinition;

        private HealthSystem HealthSystemRef;

        private BoolVariable IsLowHealth;

        #region Events callbacks

        private event Action OnPlayerLowHealthStartedEvent;
        private event Action OnPlayerLowHealthEndedEvent;

        #endregion

        public LowHealthPlayerSystem(HealthSystem HealthSystem, LowHealthPlayerSystemDefinition LowHealthPlayerSystemDefinition)
        {
            this.HealthSystemRef = HealthSystem;
            this.LowHealthPlayerSystemDefinition = LowHealthPlayerSystemDefinition;
            this.IsLowHealth = new BoolVariable(false, this.OnLowHealthStarted, this.OnLowHealthEnded);
            HealthSystem.RegisterOnHealthValueChangedEventListener(this.OnHealthValueChanged);
        }

        private void OnHealthValueChanged(float OldValue, float newValue)
        {
            this.IsLowHealth.SetValue(this.HealthSystemRef.GetHealthInPercent01() <= this.LowHealthPlayerSystemDefinition.LowHealthThreshold);
        }

        private void OnLowHealthStarted()
        {
            this.OnPlayerLowHealthStartedEvent?.Invoke();
        }

        private void OnLowHealthEnded()
        {
            this.OnPlayerLowHealthEndedEvent?.Invoke();
        }

        #region Logical Conditiions

        public bool IsHealthConsideredLow()
        {
            return this.IsLowHealth.GetValue();
        }

        #endregion

        #region Events registering

        public void RegisterPlayerLowHealthStartedEvent(Action action)
        {
            this.OnPlayerLowHealthStartedEvent += action;
        }

        public void UnRegisterPlayerLowHealthStartedEvent(Action action)
        {
            this.OnPlayerLowHealthStartedEvent -= action;
        }

        public void RegisterPlayerLowHealthEndedEvent(Action action)
        {
            this.OnPlayerLowHealthEndedEvent += action;
        }

        public void UnRegisterPlayerLowHealthEndedEvent(Action action)
        {
            this.OnPlayerLowHealthEndedEvent -= action;
        }

        #endregion
    }
}