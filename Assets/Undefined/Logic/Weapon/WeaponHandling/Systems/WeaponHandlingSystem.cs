using Firing;
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
            FiringProjectilePathCalculation.CalculateProjectilePath_Forward(WeaponHandlingSystemInitializationData.Parent, out Vector3 firedProjectilePosition, out Quaternion firedProjectileRotation);
            this.WeaponReference.SpawnFiredProjectile(new TransformStruct() {WorldPosition = firedProjectilePosition, WorldRotationEuler = firedProjectileRotation.eulerAngles});
        }

        /// <summary>
        /// Spawns a fired projectile at <see cref="GetWorldWeaponFirePoint"/> pointing to <paramref name="Target"/>.
        /// </summary>
        public void AskToFireAFiredProjectile_ToTarget(CoreInteractiveObject Target)
        {
            FiringProjectilePathCalculation.CalculateProjectilePath_ToTargetPoint(WeaponHandlingSystemInitializationData.Parent, Target, out Vector3 firedProjectilePosition, out Quaternion firedProjectileRotation);
            this.WeaponReference.SpawnFiredProjectile(new TransformStruct() {WorldPosition = firedProjectilePosition, WorldRotationEuler = firedProjectileRotation.eulerAngles});
        }

        /// <summary>
        /// Returns the maximum range of the currenlty equipped <see cref="WeaponReference"/>.
        /// </summary>
        public float GetFiredProjectileMaxRange()
        {
            return this.WeaponReference.GetFiredProjectileMaxRange();
        }

        /// <summary>
        /// Returns the starting point of a fired projectile in world space.
        /// </summary>
        public Vector3 GetWorldWeaponFirePoint()
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