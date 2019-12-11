using System;
using Health;
using InteractiveObject_Animation;
using InteractiveObjects;
using InteractiveObjects_Interfaces;

namespace PlayerLowHealth
{
    public class LowHealthPlayerSystem
    {
        private LowHealthPlayerSystemDefinition LowHealthPlayerSystemDefinition;

        private CoreInteractiveObject AssociatedInteractiveObject;
        private HealthSystem HealthSystemRef;

        private BoolVariable IsLowHealth;

        #region Callbacks

        private Action OnLowHealthStartedCallback;
        private Action OnLowHealthEndedCallback;

        #endregion

        public LowHealthPlayerSystem(CoreInteractiveObject AssociatedInteractiveObject,
            HealthSystem HealthSystem, LowHealthPlayerSystemDefinition LowHealthPlayerSystemDefinition,
            Action OnLowHealthStartedCallback = null, Action OnLowHealthEndedCallback = null)
        {
            this.OnLowHealthStartedCallback = OnLowHealthStartedCallback;
            this.OnLowHealthEndedCallback = OnLowHealthEndedCallback;
            
            this.AssociatedInteractiveObject = AssociatedInteractiveObject;
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
            this.AssociatedInteractiveObject.ConstrainSpeed(new NotAboveSpeedAttenuationConstraint(this.LowHealthPlayerSystemDefinition.OnLowhealthSpeedAttenuationFactor));
            this.AssociatedInteractiveObject.SetAISpeedAttenuationFactor(this.LowHealthPlayerSystemDefinition.OnLowhealthSpeedAttenuationFactor);
            this.OnLowHealthStartedCallback?.Invoke();
        }

        private void OnLowHealthEnded()
        {
            this.AssociatedInteractiveObject.RemoveSpeedConstraints();
            this.AssociatedInteractiveObject.SetAISpeedAttenuationFactor(AIMovementSpeedAttenuationFactor.RUN);
            this.OnLowHealthEndedCallback?.Invoke();
        }

        #region Logical Conditiions

        public bool IsHealthConsideredLow()
        {
            return this.IsLowHealth.GetValue();
        }

        #endregion
    }
}