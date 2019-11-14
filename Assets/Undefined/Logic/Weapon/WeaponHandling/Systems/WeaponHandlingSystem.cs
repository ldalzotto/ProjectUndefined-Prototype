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

        /// <summary>
        /// Spawns a fired projectile at <see cref="GetWorldWeaponFirePoint"/> in the associated <see cref="CoreInteractiveObject"/> forward vector direction.
        /// </summary>
        public void AskToFireAFiredProjectile_Forward()
        {
            var parent = WeaponHandlingSystemInitializationData.Parent;
            //Eq (1)
            var firedProjectilePosition = GetWorldWeaponFirePoint();
            var firedProjectileRotation = parent.InteractiveGameObject.GetTransform().WorldRotationEuler;
            this.WeaponReference.SpawnFiredProjectile(new TransformStruct() {WorldPosition = firedProjectilePosition, WorldRotationEuler = firedProjectileRotation});
        }
        
        /// <summary>
        /// Spawns a fired projectile at <see cref="GetWorldWeaponFirePoint"/> pointing to <paramref name="WorldDestination"/>.
        /// </summary>
        public void AskToFireAFiredProjectile_ToTargetPoint(Vector3 WorldDestination)
        {
            var firedProjectilePosition = GetWorldWeaponFirePoint();
            var firedProjectileRotation = Quaternion.LookRotation((WorldDestination - firedProjectilePosition).normalized).eulerAngles;
            this.WeaponReference.SpawnFiredProjectile(new TransformStruct() {WorldPosition = firedProjectilePosition, WorldRotationEuler = firedProjectileRotation});
        }

        private Vector3 GetWorldWeaponFirePoint()
        {
            var parent = WeaponHandlingSystemInitializationData.Parent;
            return parent.InteractiveGameObject.GetTransform().WorldPosition + WeaponHandlingSystemInitializationData.WeaponHandlingFirePointOriginLocalDefinition.WeaponFirePointOriginLocal;
        }
    }

    public class WeaponHandlingSystemInitializationData
    {
        public CoreInteractiveObject Parent;
        public WeaponHandlingFirePointOriginLocalDefinition WeaponHandlingFirePointOriginLocalDefinition;
        public WeaponDefinition WeaponDefinition;

        public WeaponHandlingSystemInitializationData(CoreInteractiveObject parent, WeaponHandlingFirePointOriginLocalDefinition WeaponHandlingFirePointOriginLocalDefinition, WeaponDefinition WeaponDefinition)
        {
            Parent = parent;
            this.WeaponHandlingFirePointOriginLocalDefinition = WeaponHandlingFirePointOriginLocalDefinition;
            this.WeaponDefinition = WeaponDefinition;
        }
    }
}