using Health;
using InteractiveObjects;
using InteractiveObjects_Interfaces;

namespace PlayerObject
{
    public class LowHealthPlayerSystem
    {
        private LowHealthPlayerSystemDefinition LowHealthPlayerSystemDefinition;

        private CoreInteractiveObject AssociatedInteractiveObject;
        private HealthSystem HealthSystemRef;


        private BoolVariable IsLowHealth;

        public LowHealthPlayerSystem(CoreInteractiveObject AssociatedInteractiveObject, HealthSystem HealthSystem, LowHealthPlayerSystemDefinition LowHealthPlayerSystemDefinition)
        {
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
            this.AssociatedInteractiveObject.LockSpeed(ObjectSpeedAttenuationLockToken.LOW_HEALTH);
            this.AssociatedInteractiveObject.SetAISpeedAttenuationFactor(this.LowHealthPlayerSystemDefinition.OnLowhealthSpeedAttenuationFactor, ObjectSpeedAttenuationLockToken.LOW_HEALTH);
        }

        private void OnLowHealthEnded()
        {
            this.AssociatedInteractiveObject.UnlockSpeed();
            this.AssociatedInteractiveObject.SetAISpeedAttenuationFactor(AIMovementSpeedAttenuationFactor.RUN);
        }
    }
}