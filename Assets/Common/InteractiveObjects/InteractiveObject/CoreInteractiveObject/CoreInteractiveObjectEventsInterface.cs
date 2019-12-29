using System;
using InteractiveObjects_Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace InteractiveObjects
{
    public abstract partial class CoreInteractiveObject
    {

        #region Speed Data Retrieval

        /// <summary>
        /// Returns the world space speed used by the physics engine.
        /// </summary>
        public virtual Vector3 GetWorldSpeedScaled()
        {
            return default(Vector3);
        }

        #endregion

        #region Health Events

        /// <param name="DamageDealerInteractiveObject">The reference to the <see cref="CoreInteractiveObject"/> that is provoking damages.</param>
        public virtual void DealDamage(float Damage, CoreInteractiveObject DamageDealerInteractiveObject)
        {
        }

        /// <summary>
        /// Must be used if we want to heal the <see cref="CoreInteractiveObject"/>.
        /// /!\ Calling <see cref="DealDamage"/> with negative value may potentially trigger dealing damage associated events and may
        /// cause unexpected behavior.
        /// </summary>
        public virtual void OnRecoverHealth(float recoveredHealthAmount)
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
        
        public virtual void AskToFireAFiredProjectile_ToDirection(Vector3 WorldDirection){}

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
        public virtual void SwitchWeaponHolder(CoreInteractiveObject NewWeaponHolder)
        {
        }

        #endregion

        #region deflection events

        /// <param name="DelfectionActorObject">It's the InteractiveObject that has triggered the deflection of the concerned InteractiveObject.</param>
        public virtual bool AskIfProjectileCanBeDeflected(CoreInteractiveObject DelfectionActorObject)
        {
            return false;
        }

        /// <summary>
        /// Effective deflection of the concerned InteractiveObject
        /// </summary>
        /// <param name="DelfectionActorObject">It's the InteractiveObject that has triggered the deflection of the concerned InteractiveObject.</param>
        public virtual void InteractiveObjectDeflected(CoreInteractiveObject DelfectionActorObject)
        {
        }

        #endregion
    }
}