using System.CodeDom;
using InteractiveObjects_Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace InteractiveObjects
{
    public abstract partial class CoreInteractiveObject
    {
        #region AI Events

        public virtual NavMeshPathStatus SetDestination(IAgentMovementCalculationStrategy IAgentMovementCalculationStrategy)
        {
            return NavMeshPathStatus.PathInvalid;
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

        /// <param name="DamageDealerInteractiveObject">The reference to the <see cref="CoreInteractiveObject"/> that is provoking damages.</param>
        public virtual void DealDamage(float Damage, CoreInteractiveObject DamageDealerInteractiveObject)
        {
        }

        public virtual void OnHealthChanged(float oldVlaue, float newValue)
        {
        }

        #endregion

        #region Projectile Events

        /// <summary>
        /// The created projectile direction will the the weapon holder <see cref="CoreInteractiveObject"/> forward vector.
        /// </summary>
        public virtual void AskToFireAFiredProjectile_Forward()
        {
        }

        public virtual void AskToFireAFiredProjectile_ToTargetPoint(Vector3 WorldDestination)
        {
        }

        /// <summary>
        /// This is the optimum position local position of where objects should aim to hit the associated <see cref="CoreInteractiveObject"/>.
        /// </summary>
        public virtual Vector3 GetFiringTargetLocalPosition()
        {
            return default(Vector3);
        }

        #endregion
    }
}