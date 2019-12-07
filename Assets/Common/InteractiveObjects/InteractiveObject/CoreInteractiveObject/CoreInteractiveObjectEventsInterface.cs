﻿using System.CodeDom;
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

        public NavMeshPathStatus SetDestination(IAgentMovementCalculationStrategy IAgentMovementCalculationStrategy, AIMovementSpeedAttenuationFactor aiMovementSpeedAttenuationFactor)
        {
            this.SetAISpeedAttenuationFactor(aiMovementSpeedAttenuationFactor);
            return this.SetDestination(IAgentMovementCalculationStrategy);
        }

        public virtual void SetAISpeedAttenuationFactor(AIMovementSpeedAttenuationFactor aiMovementSpeedAttenuationFactor, ObjectSpeedAttenuationLockToken objectSpeedAttenuationLockToken = ObjectSpeedAttenuationLockToken.NONE)
        {
        }

        public virtual void LockSpeed(ObjectSpeedAttenuationLockToken objectSpeedAttenuationLockToken)
        {
        }

        public virtual void UnlockSpeed()
        {
        }

        public virtual AIMovementSpeedAttenuationFactor GetCurrentSpeedAttenuationFactor()
        {
            return default;
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

        #endregion

        #region Projectile Events

        /// <summary>
        /// The created projectile direction will the the weapon holder <see cref="CoreInteractiveObject"/> forward vector.
        /// </summary>
        public virtual void AskToFireAFiredProjectile_Forward()
        {
        }

        public virtual void AskToFireAFiredProjectile_ToTarget(CoreInteractiveObject Target)
        {
        }

        /// <summary>
        /// Returns the starting point in world space of a fired projectile.
        /// /!\ It is implemented only for PlayerInteractiveObject.
        /// </summary>
        public virtual Vector3 GetWeaponWorldFirePoint()
        {
            return default(Vector3);
        }

        /// <summary>
        /// Returns the max range that can shoot the currently equiped weapon is the associated interactive object is the Player.
        /// Returns the fired projectile max range if the associated interactive object is a Weapon.
        /// </summary>
        public virtual float GetFiredProjectileMaxRange()
        {
            return 0f;
        }

        /// <summary>
        /// This is the optimum position local position of where objects should aim to hit the associated <see cref="CoreInteractiveObject"/>.
        /// </summary>
        public virtual Vector3 GetFiringTargetLocalPosition()
        {
            return default(Vector3);
        }
        
        /// <summary>
        /// When the associated interactive object belongs to a weapon, then it's weapon reference is changed by <paramref name="NewWeaponHolder"/>.
        /// </summary>
        public virtual void SwitchWeaponHolder(CoreInteractiveObject NewWeaponHolder){}

        #endregion
    }
}