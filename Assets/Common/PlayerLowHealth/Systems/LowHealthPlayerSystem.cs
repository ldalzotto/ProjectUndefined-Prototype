using Health;
using InteractiveObject_Animation;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using ProjectileDeflection;

namespace PlayerLowHealth
{
    public class LowHealthPlayerSystem
    {
        private LowHealthPlayerSystemDefinition LowHealthPlayerSystemDefinition;

        private CoreInteractiveObject AssociatedInteractiveObject;
        private BaseObjectAnimatorPlayableSystem BaseObjectAnimatorPlayableSystemRef;
        private HealthSystem HealthSystemRef;

        private BoolVariable IsLowHealth;

        #region Inline Systems

        private ProjectileDeflectionSystem ProjectileDeflectionSystem;

        #endregion
        
        public LowHealthPlayerSystem(CoreInteractiveObject AssociatedInteractiveObject, BaseObjectAnimatorPlayableSystem BaseObjectAnimatorPlayableSystemRef,
            HealthSystem HealthSystem, LowHealthPlayerSystemDefinition LowHealthPlayerSystemDefinition)
        {
            this.AssociatedInteractiveObject = AssociatedInteractiveObject;
            this.BaseObjectAnimatorPlayableSystemRef = BaseObjectAnimatorPlayableSystemRef;
            this.HealthSystemRef = HealthSystem;
            this.LowHealthPlayerSystemDefinition = LowHealthPlayerSystemDefinition;
            this.ProjectileDeflectionSystem = new ProjectileDeflectionSystem(AssociatedInteractiveObject);
            this.IsLowHealth = new BoolVariable(false, this.OnLowHealthStarted, this.OnLowHealthEnded);
            HealthSystem.RegisterOnHealthValueChangedEventListener(this.OnHealthValueChanged);
        }

        public void Tick(float d)
        {
            if (this.IsLowHealth.GetValue())
            {
                this.ProjectileDeflectionSystem.Tick(d);
            }   
        }

        private void OnHealthValueChanged(float OldValue, float newValue)
        {
            this.IsLowHealth.SetValue(this.HealthSystemRef.GetHealthInPercent01() <= this.LowHealthPlayerSystemDefinition.LowHealthThreshold);
        }

        private void OnLowHealthStarted()
        {
            this.AssociatedInteractiveObject.LockSpeed(ObjectSpeedAttenuationLockToken.LOW_HEALTH);
            this.AssociatedInteractiveObject.SetAISpeedAttenuationFactor(this.LowHealthPlayerSystemDefinition.OnLowhealthSpeedAttenuationFactor, ObjectSpeedAttenuationLockToken.LOW_HEALTH);
            this.BaseObjectAnimatorPlayableSystemRef.PlayLocomotionAnimationOverride(this.LowHealthPlayerSystemDefinition.OnLowHealthLocomotionAnimation, AnimationLayerID.LocomotionLayer_1);
        }

        private void OnLowHealthEnded()
        {
            this.AssociatedInteractiveObject.UnlockSpeed();
            this.AssociatedInteractiveObject.SetAISpeedAttenuationFactor(AIMovementSpeedAttenuationFactor.RUN);
            this.AssociatedInteractiveObject.AnimationController.DestroyAnimationLayer(AnimationLayerID.LocomotionLayer_1);
        }
    }
}