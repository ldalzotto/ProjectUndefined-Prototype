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
        private BaseObjectAnimatorPlayableSystem BaseObjectAnimatorPlayableSystemRef;
        private HealthSystem HealthSystemRef;

        private BoolVariable IsLowHealth;

        public LowHealthPlayerSystem(CoreInteractiveObject AssociatedInteractiveObject, BaseObjectAnimatorPlayableSystem BaseObjectAnimatorPlayableSystemRef,
            HealthSystem HealthSystem, LowHealthPlayerSystemDefinition LowHealthPlayerSystemDefinition)
        {
            this.AssociatedInteractiveObject = AssociatedInteractiveObject;
            this.BaseObjectAnimatorPlayableSystemRef = BaseObjectAnimatorPlayableSystemRef;
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
            this.BaseObjectAnimatorPlayableSystemRef.PlayLocomotionAnimationOverride(this.LowHealthPlayerSystemDefinition.OnLowHealthLocomotionAnimation, AnimationLayerID.LocomotionLayer_1);
        }

        private void OnLowHealthEnded()
        {
            this.AssociatedInteractiveObject.RemoveSpeedConstraints();
            this.AssociatedInteractiveObject.SetAISpeedAttenuationFactor(AIMovementSpeedAttenuationFactor.RUN);
            this.AssociatedInteractiveObject.AnimationController.DestroyAnimationLayer(AnimationLayerID.LocomotionLayer_1);
        }

        #region Logical Conditiions

        public bool IsHealthConsideredLow()
        {
            return this.IsLowHealth.GetValue();
        }

        #endregion
    }
}