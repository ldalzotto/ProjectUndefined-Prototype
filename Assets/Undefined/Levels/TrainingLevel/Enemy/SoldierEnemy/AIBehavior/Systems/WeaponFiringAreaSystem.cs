using System;
using System.Collections.Generic;
using InteractiveObjects;
using PlayerObject;
using RangeObjects;
using UnityEngine;

namespace TrainingLevel
{
    /// <summary>
    /// Holds and update informations about the ability to shoot to the Player. This is achieved by creating a trigger WeaponFiringAreaBoxRangeObject between the
    /// <see cref="SoliderEnemy.GetWeaponFirePointOriginLocalAction"/> and the <see cref="PlayerInteractiveObject.GetFiringTargetLocalPosition"/>.
    /// Ability to shoot to the Player is provided by ensuring that the WeaponFiringAreaBoxRangeObject doesn't contains Obstacles.
    /// </summary>
    public class WeaponFiringAreaSystem : AInteractiveObjectPhysicsEventListener
    {
        private CoreInteractiveObject AssociatedInteractiveObject;
        private PlayerObjectStateDataSystem PlayerObjectStateDataSystem;
        /// <summary>
        /// The trigger only WeaponFiringAreaBoxRangeObject
        /// /!\ Must be disposed on destroy
        /// </summary>
        private BoxRangeObjectV2 WeaponFiringAreaBoxRangeObject;
        private Func<Vector3> GetWeaponFirePointOriginLocalAction;
        
        private List<Collider> InsideWeaponFiringAreaObstacles = new List<Collider>();

        public WeaponFiringAreaSystem(CoreInteractiveObject associatedInteractiveObject, PlayerObjectStateDataSystem playerObjectStateDataSystem, Func<Vector3> weaponFirePointOriginLocalAction)
        {
            AssociatedInteractiveObject = associatedInteractiveObject;
            PlayerObjectStateDataSystem = playerObjectStateDataSystem;
            GetWeaponFirePointOriginLocalAction = weaponFirePointOriginLocalAction;
            this.WeaponFiringAreaBoxRangeObject = new BoxRangeObjectV2(associatedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent, new BoxRangeObjectInitialization()
            {
                RangeTypeID = RangeTypeID.NOT_DISPLAYED,
                IsTakingIntoAccountObstacles = false,
                BoxRangeTypeDefinition = new BoxRangeTypeDefinition()
            }, associatedInteractiveObject, "WeaponFiringAreaBoxRangeObject");
            this.WeaponFiringAreaBoxRangeObject.RegisterPhysicsEventListener(this);
        }

        public void Tick(float d)
        {
            this.WeaponFiringAreaBoxRangeObject.RangeGameObjectV2.RangeGameObject.transform.localPosition = this.GetWeaponFirePointOriginLocalAction.Invoke();
            var PlayerObject = this.PlayerObjectStateDataSystem.PlayerObject();
            var PlayerObjectWorldPosition = PlayerObject.InteractiveGameObject.GetTransform().WorldPosition;
            this.WeaponFiringAreaBoxRangeObject.RangeGameObjectV2.RangeGameObject.transform.rotation = Quaternion.LookRotation((PlayerObjectWorldPosition + PlayerObject.GetFiringTargetLocalPosition() - this.WeaponFiringAreaBoxRangeObject.GetTransform().WorldPosition).normalized);

            var DistanceSoldierPlayer = Vector3.Distance((PlayerObjectWorldPosition + PlayerObject.GetFiringTargetLocalPosition()), this.WeaponFiringAreaBoxRangeObject.GetTransform().WorldPosition);
            this.WeaponFiringAreaBoxRangeObject.SetLocalCenter(new Vector3(0, 0, DistanceSoldierPlayer * 0.5f));
            this.WeaponFiringAreaBoxRangeObject.SetLocalSize(new Vector3(2, 2, DistanceSoldierPlayer));
        }

        public bool AreObstaclesInside()
        {
            return this.InsideWeaponFiringAreaObstacles.Count > 0;
        }

        public override bool ColliderSelectionGuard(InteractiveObjectPhysicsTriggerInfo interactiveObjectPhysicsTriggerInfo)
        {
            return interactiveObjectPhysicsTriggerInfo.GetOtherInteractiveObjectTag().IsObstacle;
        }

        public override void OnTriggerEnter(InteractiveObjectPhysicsTriggerInfo PhysicsTriggerInfo)
        {
            this.InsideWeaponFiringAreaObstacles.Add(PhysicsTriggerInfo.Other);
        }

        public override void OnTriggerExit(InteractiveObjectPhysicsTriggerInfo PhysicsTriggerInfo)
        {
            this.InsideWeaponFiringAreaObstacles.Remove(PhysicsTriggerInfo.Other);
        }

        public void Destroy()
        {
            this.WeaponFiringAreaBoxRangeObject.OnDestroy();
            GameObject.Destroy(this.WeaponFiringAreaBoxRangeObject.RangeGameObjectV2.RangeGameObject);
        }
    }
}