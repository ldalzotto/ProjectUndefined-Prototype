﻿using System.Collections.Generic;
using System.Runtime.Serialization;
using Damage;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using ProjectileDeflection;
using ProjectileDeflection_Interface;
using UnityEngine;

namespace Firing
{
    public class FiredProjectile : CoreInteractiveObject
    {
        private FiredProjectileDefinition FiredProjectileDefinition;

        private ProjectileWeaponHoldingSystem ProjectileWeaponHoldingSystem;
        private FiredProjectileMovementSystem FiredProjectileMovementSystem;
        private DamageDealerEmitterSystem _damageDealerEmitterSystem;

        public FiredProjectile(IInteractiveGameObject parent, FiredProjectileDefinition FiredProjectileDefinition, CoreInteractiveObject weaponHolder)
        {
            this.FiredProjectileDefinition = FiredProjectileDefinition;
            this.interactiveObjectTag = new InteractiveObjectTag() {IsDealingDamage = true};

            parent.CreateLogicCollider(BuildBoxColliderDefinition(FiredProjectileDefinition));
            this.BaseInit(parent, true);

            this.ProjectileWeaponHoldingSystem = new ProjectileWeaponHoldingSystem(weaponHolder);
            this.FiredProjectileMovementSystem = new FiredProjectileMovementSystem(FiredProjectileDefinition, this);
            this._damageDealerEmitterSystem = new DamageDealerEmitterSystem(this, FiredProjectileDefinition.damageDealerEmitterSystemDefinition,
                TriggerSelectionGuard: (InteractiveObjectPhysicsTriggerInfo) => FiredProjectileHasTriggerEnter_DealsDamage_And_MustBeDestroyed(InteractiveObjectPhysicsTriggerInfo.OtherInteractiveObject, this.ProjectileWeaponHoldingSystem.GetWeaponHolder()),
                OnDamageDealtToOtherAction: null);
            this.RegisterInteractiveObjectPhysicsEventListener(new InteractiveObjectPhysicsEventListenerDelegated(
                (InteractiveObjectPhysicsTriggerInfo) => FiredProjectileHasTriggerEnter_DealsDamage_And_MustBeDestroyed(InteractiveObjectPhysicsTriggerInfo.OtherInteractiveObject, this.ProjectileWeaponHoldingSystem.GetWeaponHolder()),
                this.OnObstacleTriggerEnter));
        }

        /// <summary>
        /// Because <see cref="FiredProjectile"/> are often a High Speed Rigid body, we force the RigidBody interpolation mode to Interpolate
        /// </summary>
        private static InteractiveObjectBoxLogicColliderDefinitionStruct BuildBoxColliderDefinition(FiredProjectileDefinition FiredProjectileDefinition)
        {
            InteractiveObjectBoxLogicColliderDefinitionStruct InteractiveObjectBoxLogicColliderDefinitionStruct = FiredProjectileDefinition.InteractiveObjectBoxLogicColliderDefinition;
            InteractiveObjectBoxLogicColliderDefinitionStruct.RigidbodyInterpolation = RigidbodyInterpolation.Extrapolate;
            InteractiveObjectBoxLogicColliderDefinitionStruct.Enabled = true;
            return InteractiveObjectBoxLogicColliderDefinitionStruct;
        }

        public override void Tick(float d)
        {
            base.Tick(d);
        }

        public override void FixedTick(float d)
        {
            this.FiredProjectileMovementSystem.FixedTick(d);
            base.FixedTick(d);
        }

        public override void Init()
        {
        }

        private void OnObstacleTriggerEnter(CoreInteractiveObject OtherInteractiveObject)
        {
            this.AskToDestroy();
        }

        public override void SwitchWeaponHolder(CoreInteractiveObject NewWeaponHolder)
        {
            this.ProjectileWeaponHoldingSystem.SwitchWeaponHolder(NewWeaponHolder);
        }

        public override ProjectileDeflectedPropertiesStruct OnInteractiveObjectDeflected(CoreInteractiveObject DelfectionActorObject)
        {
            DeflectionCalculations.ForwardDeflection(DelfectionActorObject, this);
            return this.FiredProjectileDefinition.ProjectileDeflectedProperties;
        }

        #region Logical Conditions

        public static bool FiredProjectileHasTriggerEnter_DealsDamage_And_MustBeDestroyed(CoreInteractiveObject OtherInteractiveObject, CoreInteractiveObject WeaponHolder)
        {
            /// Fired projectile deals damage and is destroyed when :
            return OtherInteractiveObject != WeaponHolder /// Collided interactive object is not the weapon holder (to avoir self harming)
                   && /// AND
                   (OtherInteractiveObject.InteractiveObjectTag.IsObstacle /// Collided object is an Obstacle -> damage taken by obstacle will be ignored
                    || /// OR
                    (OtherInteractiveObject.InteractiveObjectTag.IsTakingDamage /// Collided object is taking damange 
                     && /// AND
                     /// Weapon holder or target is the Player -> this will prevent a projectile lauinched by an AI to deal damage to another AI  
                     (WeaponHolder.InteractiveObjectTag.IsPlayer || OtherInteractiveObject.InteractiveObjectTag.IsPlayer))
                   );
        }

        #endregion

        public void AskToDestroy()
        {
            this.isAskingToBeDestroyed = true;
        }
    }
}