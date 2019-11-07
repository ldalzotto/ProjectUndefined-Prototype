using InteractiveObjects_Interfaces;

namespace InteractiveObjects
{
    public abstract partial class CoreInteractiveObject
    {
        #region AI Events

        public virtual void SetDestination(IAgentMovementCalculationStrategy IAgentMovementCalculationStrategy)
        {
        }

        public virtual void SetAISpeedAttenuationFactor(AIMovementSpeedDefinition AIMovementSpeedDefinition)
        {
        }

        #endregion

        #region Attractive Object Events

        public virtual void OnOtherAttractiveObjectJustIntersected(CoreInteractiveObject OtherInteractiveObject)
        {
        }

        public virtual void OnOtherAttractiveObjectIntersectedNothing(CoreInteractiveObject OtherInteractiveObject)
        {
        }

        public virtual void OnOtherAttractiveObjectNoMoreIntersected(CoreInteractiveObject OtherInteractiveObject)
        {
        }

        #endregion

        #region Disarm Object Events

        public virtual void OnOtherDisarmObjectTriggerEnter(CoreInteractiveObject OtherInteractiveObject)
        {
        }

        public virtual void OnOtherDisarmobjectTriggerExit(CoreInteractiveObject OtherInteractiveObject)
        {
        }

        #endregion

        #region Health Events

        public virtual void DealDamage(float Damage)
        {
        }

        public virtual void OnHealthBelowZero()
        {
        }

        #endregion

        #region Projectile Events

        public virtual void AskToFireAFiredProjectile()
        {
        }

        #endregion
    }
}