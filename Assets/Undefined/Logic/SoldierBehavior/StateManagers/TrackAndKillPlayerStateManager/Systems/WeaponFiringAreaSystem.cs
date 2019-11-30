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
    /// Ability to shoot to the Player is provided by ensuring that the WeaponFiringAreaBoxRangeObject doesn't contains Obstacles.
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
        private WeaponFiringAreaSystemExternalCallbacks WeaponFiringAreaSystemExternalCallbacks;
        
        public WeaponFiringAreaSystem(CoreInteractiveObject associatedInteractiveObject, PlayerObjectStateDataSystem playerObjectStateDataSystem, 
            WeaponFiringAreaSystemExternalCallbacks WeaponFiringAreaSystemExternalCallbacks)
        {
            AssociatedInteractiveObject = associatedInteractiveObject;
            PlayerObjectStateDataSystem = playerObjectStateDataSystem;
            this.WeaponFiringAreaSystemExternalCallbacks = WeaponFiringAreaSystemExternalCallbacks;
            this.WeaponFiringAreaBoxRangeObject = new BoxRayRangeObject(associatedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent, new BoxRangeObjectInitialization()
            {
                RangeTypeID = RangeTypeID.NOT_DISPLAYED,
                IsTakingIntoAccountObstacles = false,
                BoxRangeTypeDefinition = new BoxRangeTypeDefinition()
            }, associatedInteractiveObject,
                2f,
                delegate(InteractiveObjectPhysicsTriggerInfo interactiveObjectPhysicsTriggerInfo) { return interactiveObjectPhysicsTriggerInfo.GetOtherInteractiveObjectTag().IsObstacle;  }
                , "WeaponFiringAreaBoxRangeObject");
        }

        public void Tick(float d)
        {
            this.WeaponFiringAreaBoxRangeObject.RangeGameObjectV2.RangeGameObject.transform.localPosition = this.WeaponFiringAreaSystemExternalCallbacks.weaponFirePointOriginLocalDefinitionAction.Invoke().WeaponFirePointOriginLocal;
            var PlayerObject = this.PlayerObjectStateDataSystem.PlayerObject();
            var PlayerObjectWorldPosition = PlayerObject.InteractiveGameObject.GetTransform().WorldPosition;
            this.WeaponFiringAreaBoxRangeObject.RangeGameObjectV2.RangeGameObject.transform.rotation = Quaternion.LookRotation((PlayerObjectWorldPosition + PlayerObject.GetFiringTargetLocalPosition() - this.WeaponFiringAreaBoxRangeObject.GetTransform().WorldPosition).normalized);

            var DistanceSoldierPlayer = Vector3.Distance((PlayerObjectWorldPosition + PlayerObject.GetFiringTargetLocalPosition()), this.WeaponFiringAreaBoxRangeObject.GetTransform().WorldPosition);
            this.WeaponFiringAreaBoxRangeObject.SetLocalCenter(new Vector3(0, 0, DistanceSoldierPlayer * 0.5f));
            this.WeaponFiringAreaBoxRangeObject.SetLocalSize(new Vector3(2, 2, DistanceSoldierPlayer));
        }

        public bool AreObstaclesInside()
        {
            return this.WeaponFiringAreaBoxRangeObject.InsideInteractiveObjects.Count > 0;
        }
        
        public void OnDestroy()
        {
            this.WeaponFiringAreaBoxRangeObject.OnDestroy();
      //      GameObject.Destroy(this.WeaponFiringAreaBoxRangeObject.RangeGameObjectV2.RangeGameObject);
        }

        /// <summary>
        /// Enables the <see cref="WeaponFiringAreaBoxRangeObject"/> initializing it's inside colliders and add it from physics world
        /// Because enabling will probably called multiple frames after <see cref="Disable"/>, a full manual update is performed
        /// </summary>
        public void Enable()
        {
            this.Tick(0f);
            this.WeaponFiringAreaBoxRangeObject.Enable();
            this.WeaponFiringAreaBoxRangeObject.ManuallyDetectInsideColliders();
        }

        /// <summary>
        /// Disables the <see cref="WeaponFiringAreaBoxRangeObject"/> clearing all it's inside colliders and remove it from physics world
        /// </summary>
        public void Disable()
        {
            this.WeaponFiringAreaBoxRangeObject.Disable();
        }
    }
}