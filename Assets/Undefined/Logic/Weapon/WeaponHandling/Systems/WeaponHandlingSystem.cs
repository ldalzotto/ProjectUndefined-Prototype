using InteractiveObjects;
using PlayerActions;
using UnityEngine;

namespace Weapon
{
    public interface IWeaponHandlingSystem_DataRetrieval
    {
        float GetFiredProjectileMaxRange();
        float GetFiredProjectileTravelSpeed();
        Vector3 GetWorldWeaponFirePoint();
    }

    public class WeaponHandlingSystem : IWeaponHandlingSystem_DataRetrieval
    {
        public WeaponHandlingSystemInitializationData WeaponHandlingSystemInitializationData;
        private Weapon WeaponReference;
        private PlayerActionPlayerSystem AssociatedInteractiveObjectPlayerActionPlayerSystem;

        public WeaponHandlingSystem(CoreInteractiveObject AssociatedInteractiveObject,
            WeaponHandlingSystemInitializationData weaponHandlingSystemInitializationData,
            PlayerActionPlayerSystem AssociatedInteractiveObjectPlayerActionPlayerSystem)
        {
            WeaponHandlingSystemInitializationData = weaponHandlingSystemInitializationData;
            this.AssociatedInteractiveObjectPlayerActionPlayerSystem = AssociatedInteractiveObjectPlayerActionPlayerSystem;
            this.WeaponReference = this.WeaponHandlingSystemInitializationData.WeaponDefinition.BuildWeapon(AssociatedInteractiveObject);
        }

        /// <summary>
        /// Spawns a fired projectile at <see cref="GetWorldWeaponFirePoint"/> pointing in the <see cref="NormalizedWorldDirection"/> direction. 
        /// </summary>
        public void AskToFireAFiredProjectile_ToDirection(Vector3 NormalizedWorldDirection)
        {
            if (this.AssociatedInteractiveObjectPlayerActionPlayerSystem.IsActionOfTypeAllowedToBePlaying(ProjectileFireAction.ProjectileFireActionUniqueID))
            {
                this.AssociatedInteractiveObjectPlayerActionPlayerSystem.ExecuteAction(
                    new ProjectileFireAction(new ProjectileFireActionInputData(this.WeaponReference, this.WeaponHandlingSystemInitializationData.WeaponDefinition.ProjectileFireActionDefinition, NormalizedWorldDirection)));
            }
        }

        /// <summary>
        /// Returns the maximum range of the currenlty equipped <see cref="WeaponReference"/>.
        /// </summary>
        public float GetFiredProjectileMaxRange()
        {
            return this.WeaponReference.GetFiredProjectileMaxRange();
        }

        public float GetFiredProjectileTravelSpeed()
        {
            return this.WeaponReference.GetFiredProjectileTravelSpeed();
        }

        /// <summary>
        /// Returns the starting point of a fired projectile in world space.
        /// </summary>
        public Vector3 GetWorldWeaponFirePoint()
        {
            var parent = WeaponHandlingSystemInitializationData.Parent;
            return parent.InteractiveGameObject.GetTransform().WorldPosition + WeaponHandlingSystemInitializationData.WeaponHandlingFirePointOriginLocalDefinition.WeaponFirePointOriginLocal;
        }

        public void Destroy()
        {
            this.WeaponReference.Destroy();
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