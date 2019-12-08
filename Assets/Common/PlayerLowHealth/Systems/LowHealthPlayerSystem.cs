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

        private ProjectileDeflectionSystem projectileDeflectionSystem;

        public ProjectileDeflectionSystem ProjectileDeflectionSystem
        {
            get { return this.projectileDeflectionSystem; }
        }
        #endregion
        
        public LowHealthPlayerSystem(CoreInteractiveObject AssociatedInteractiveObject, BaseObjectAnimatorPlayableSystem BaseObjectAnimatorPlayableSystemRef,
            HealthSystem HealthSystem, LowHealthPlayerSystemDefinition LowHealthPlayerSystemDefinition, ProjectileDeflectionDefinition ProjectileDeflectionDefinition)
        {
            this.AssociatedInteractiveObject = AssociatedInteractiveObject;
            this.BaseObjectAnimatorPlayableSystemRef = BaseObjectAnimatorPlayableSystemRef;
            this.HealthSystemRef = HealthSystem;
            this.LowHealthPlayerSystemDefinition = LowHealthPlayerSystemDefinition;
            this.projectileDeflectionSystem = new ProjectileDeflectionSystem(AssociatedInteractiveObject, ProjectileDeflectionDefinition);
            this.IsLowHealth = new BoolVariable(false, this.OnLowHealthStarted, this.OnLowHealthEnded);
            HealthSystem.RegisterOnHealthValueChangedEventListener(this.OnHealthValueChanged);
        }

        public void FixedTick(float d)
        {
            if (this.IsHealthConsideredLow())
            {
                this.projectileDeflectionSystem.FixedTick(d);
            }   
        }

        public void Tick(float d)
        {
            if (this.IsHealthConsideredLow())
            {
                this.projectileDeflectionSystem.Tick(d);
            }   
        }

        public void LateTick(float d)
        {
            this.projectileDeflectionSystem.LateTick(d);
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

        #region Logical Conditiions

        public bool IsHealthConsideredLow()
        {
            return this.IsLowHealth.GetValue();
        }

        #endregion
    }
}