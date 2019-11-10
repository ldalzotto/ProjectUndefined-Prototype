using InteractiveObjects_Interfaces;
using UnityEngine;

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

        public virtual void AskToFireAFiredProjectile(Vector3 WorldTargetDirection)
        {
        }

        public virtual Vector3 GetFiringTargetLocalPosition()
        {
            return this.InteractiveGameObject.GetTransform().WorldPosition;
        }

        #endregion
    }
}