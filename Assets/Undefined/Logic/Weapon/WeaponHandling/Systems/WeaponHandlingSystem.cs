using InteractiveObjects;
using UnityEngine;

namespace Weapon
{
    public class WeaponHandlingSystem
    {
        public WeaponHandlingSystemInitializationData WeaponHandlingSystemInitializationData;
        private Weapon WeaponReference;

        public WeaponHandlingSystem(CoreInteractiveObject AssociatedInteractiveObject, WeaponHandlingSystemInitializationData weaponHandlingSystemInitializationData)
        {
            WeaponHandlingSystemInitializationData = weaponHandlingSystemInitializationData;
            this.WeaponReference = this.WeaponHandlingSystemInitializationData.WeaponDefinition.BuildWeapon(AssociatedInteractiveObject);
        }

        public void AskToFireAFiredProjectile()
        {
            var parent = WeaponHandlingSystemInitializationData.Parent;
            //Eq (1)
            var firedProjectilePosition = parent.InteractiveGameObject.GetTransform().WorldPosition + WeaponHandlingSystemInitializationData.WeaponFirePointOriginLocal;
            var firedProjectileRotation = parent.InteractiveGameObject.GetTransform().WorldRotationEuler;
            this.WeaponReference.SpawnFiredProjectile(new TransformStruct() {WorldPosition = firedProjectilePosition, WorldRotationEuler = firedProjectileRotation});
        }

        public void AskToFireAFiredProjectile(Vector3 WorldTargetDirection)
        {
            var parent = WeaponHandlingSystemInitializationData.Parent;
            var firedProjectilePosition = parent.InteractiveGameObject.GetTransform().WorldPosition + WeaponHandlingSystemInitializationData.WeaponFirePointOriginLocal;
            var firedProjectileRotation = Quaternion.LookRotation(WorldTargetDirection.normalized).eulerAngles;
            this.WeaponReference.SpawnFiredProjectile(new TransformStruct() {WorldPosition = firedProjectilePosition, WorldRotationEuler = firedProjectileRotation});
        }
    }

    public class WeaponHandlingSystemInitializationData
    {
        public CoreInteractiveObject Parent;
        public Vector3 WeaponFirePointOriginLocal;
        public WeaponDefinition WeaponDefinition;

        public WeaponHandlingSystemInitializationData(CoreInteractiveObject parent, Vector3 weaponFirePointOriginLocal, WeaponDefinition WeaponDefinition)
        {
            Parent = parent;
            WeaponFirePointOriginLocal = weaponFirePointOriginLocal;
            this.WeaponDefinition = WeaponDefinition;
        }
    }
}