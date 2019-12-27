using System;
using System.Collections.Generic;
using InteractiveObjects;
using PlayerObject;
using RangeObjects;
using UnityEngine;
using Weapon;

namespace SoliderAIBehavior
{
    public struct WeaponFiringAreaSystemExternalCallbacks
    {
        public Func<WeaponHandlingFirePointOriginLocalDefinition> weaponFirePointOriginLocalDefinitionAction;

        public WeaponFiringAreaSystemExternalCallbacks(Func<WeaponHandlingFirePointOriginLocalDefinition> weaponFirePointOriginLocalDefinitionAction)
        {
            this.weaponFirePointOriginLocalDefinitionAction = weaponFirePointOriginLocalDefinitionAction;
        }
    }

    /// <summary>
    /// Holds and update informations about the ability to shoot to the Player. This is achieved by creating a trigger <see cref="WeaponFiringAreaBoxRangeObject"/> between the
    /// <see cref="SoliderEnemy.GetWeaponFirePointOriginLocalAction"/> and the <see cref="PlayerInteractiveObject.GetFiringTargetLocalPosition"/>.
    /// The <see cref="WeaponFiringAreaBoxRangeObject"/> is then adjusted from the Player speed and fired projectile speed to a predicted position (<see cref="PreshotPlayerPosition"/>).
    /// The ability to shoot to the Player is provided by ensuring that the WeaponFiringAreaBoxRangeObject doesn't contains Obstacles.
    /// </summary>
    public class WeaponFiringAreaSystem
    {
        private CoreInteractiveObject AssociatedInteractiveObject;
        private PlayerObjectStateDataSystem PlayerObjectStateDataSystem;

        /// <summary>
        /// The trigger only WeaponFiringAreaBoxRangeObject
        /// /!\ Must be disposed on destroy
        /// </summary>
        private BoxRayRangeObject WeaponFiringAreaBoxRangeObject;

        /// <summary>
        /// A trasform object representing the end of the <see cref="WeaponFiringAreaBoxRangeObject"/>. This transform is used
        /// by <see cref="ShootingAtPlayerStateManager"/> as a LookinTowards constraint.
        /// </summary>
        private Transform EndOfRayTransformPoint;

        private IFiringProjectileCallback IFiringProjectileCallback;
        private IWeaponDataRetrieval IWeaponDataRetrieval;

        public WeaponFiringAreaSystem(CoreInteractiveObject associatedInteractiveObject, PlayerObjectStateDataSystem playerObjectStateDataSystem,
            IFiringProjectileCallback IFiringProjectileCallback, IWeaponDataRetrieval IWeaponDataRetrieval)
        {
            AssociatedInteractiveObject = associatedInteractiveObject;
            PlayerObjectStateDataSystem = playerObjectStateDataSystem;
            this.IFiringProjectileCallback = IFiringProjectileCallback;
            this.IWeaponDataRetrieval = IWeaponDataRetrieval;

            this.WeaponFiringAreaBoxRangeObject = new BoxRayRangeObject(associatedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent, new BoxRangeObjectInitialization()
                {
                    RangeTypeID = RangeTypeID.NOT_DISPLAYED,
                    IsTakingIntoAccountObstacles = false,
                    BoxRangeTypeDefinition = new BoxRangeTypeDefinition()
                }, associatedInteractiveObject,
                2f,
                delegate(InteractiveObjectPhysicsTriggerInfo interactiveObjectPhysicsTriggerInfo) { return interactiveObjectPhysicsTriggerInfo.GetOtherInteractiveObjectTag().IsObstacle; }
                , "WeaponFiringAreaBoxRangeObject");
            this.EndOfRayTransformPoint = new GameObject("WeaponFiringAreaBoxRangeObject_EndOfRayTransform").transform;
            this.EndOfRayTransformPoint.transform.parent = associatedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent.transform;
        }

        public void Tick(float d)
        {
            var PredictedPlayerObjectFiringTargetWorldPosition = this.PreshotPlayerPosition();

            this.EndOfRayTransformPoint.position = PredictedPlayerObjectFiringTargetWorldPosition;
            this.WeaponFiringAreaBoxRangeObject.RangeGameObjectV2.RangeGameObject.transform.localPosition = this.IFiringProjectileCallback.GetWeaponFirePointOriginLocalDefinitionAction.Invoke().WeaponFirePointOriginLocal;
            this.WeaponFiringAreaBoxRangeObject.RangeGameObjectV2.RangeGameObject.transform.rotation = Quaternion.LookRotation((PredictedPlayerObjectFiringTargetWorldPosition - this.WeaponFiringAreaBoxRangeObject.GetTransform().WorldPosition).normalized);
            var DistanceSoldierPlayer = Vector3.Distance(PredictedPlayerObjectFiringTargetWorldPosition, this.WeaponFiringAreaBoxRangeObject.GetTransform().WorldPosition);

            this.WeaponFiringAreaBoxRangeObject.SetLocalCenter(new Vector3(0, 0, DistanceSoldierPlayer * 0.5f));
            this.WeaponFiringAreaBoxRangeObject.SetLocalSize(new Vector3(2, 2, DistanceSoldierPlayer));
        }

        /// <summary>
        /// Taking into account the <see cref="AssociatedInteractiveObject"/> firing projectile travel speed and the <see cref="PlayerObjectStateDataSystem"/> InteractiveObject speed,
        /// calculates a predicted position of the Player where the AI will aim for that will read the target if it continues to move at the same speed,
        /// along the same direction.
        /// </summary>
        private Vector3 PreshotPlayerPosition()
        {
            var AssociatedInteractiveObjectFiringShootPosition = this.AssociatedInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition + this.IFiringProjectileCallback.GetWeaponFirePointOriginLocalDefinitionAction.Invoke().WeaponFirePointOriginLocal;
            var PlayerObject = this.PlayerObjectStateDataSystem.PlayerObject();
            var PlayerObjectWorldPosition = PlayerObject.InteractiveGameObject.GetTransform().WorldPosition;
            var PlayerObjectFiringTargetWorldPosition = PlayerObjectWorldPosition + PlayerObject.GetFiringTargetLocalPosition();

            var projectileTravelTimeToPlayer = Vector3.Distance(PlayerObjectFiringTargetWorldPosition, AssociatedInteractiveObjectFiringShootPosition) / this.IWeaponDataRetrieval.GetIWeaponHandlingSystem_DataRetrievalAction.GetFiredProjectileTravelSpeed();

            var PredictedPlayerObjectFiringTargetWorldPosition = PlayerObjectFiringTargetWorldPosition + (PlayerObject.GetWorldSpeedScaled() * projectileTravelTimeToPlayer);
            return PredictedPlayerObjectFiringTargetWorldPosition;
        }

        public bool AreObstaclesInside()
        {
            return this.WeaponFiringAreaBoxRangeObject.GetInsideInteractiveObjects().Count > 0;
        }

        public Vector3 GetWorldRayForwardDirection()
        {
            return this.WeaponFiringAreaBoxRangeObject.RangeGameObjectV2.RangeGameObject.transform.forward;
        }

        public Transform GetPredictedTransform()
        {
            return this.EndOfRayTransformPoint;
        }

        public void OnDestroy()
        {
            this.WeaponFiringAreaBoxRangeObject.OnDestroy();
            GameObject.Destroy(this.EndOfRayTransformPoint.gameObject);
        }

        /// <summary>
        /// Enables the <see cref="WeaponFiringAreaBoxRangeObject"/> initializing it's inside colliders and add it from physics world
        /// Because enabling will probably called multiple frames after <see cref="Disable"/>, a full manual update is performed
        /// </summary>
        public void Enable()
        {
            this.EndOfRayTransformPoint.gameObject.SetActive(true);
            this.Tick(0f);
            this.WeaponFiringAreaBoxRangeObject.Enable();
            this.WeaponFiringAreaBoxRangeObject.ManuallyDetectInsideColliders();
        }

        /// <summary>
        /// Disables the <see cref="WeaponFiringAreaBoxRangeObject"/> clearing all it's inside colliders and remove it from physics world
        /// </summary>
        public void Disable()
        {
            this.EndOfRayTransformPoint.gameObject.SetActive(false);
            this.WeaponFiringAreaBoxRangeObject.Disable();
        }
    }
}