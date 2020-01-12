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
        private WeaponHandlingSystemInitializationData WeaponHandlingSystemInitializationData;
        private Weapon WeaponReference;

        public WeaponHandlingSystem(CoreInteractiveObject AssociatedInteractiveObject,
            WeaponHandlingSystemInitializationData weaponHandlingSystemInitializationData)
        {
            WeaponHandlingSystemInitializationData = weaponHandlingSystemInitializationData;
            this.WeaponReference = this.WeaponHandlingSystemInitializationData.WeaponDefinition.BuildWeapon(AssociatedInteractiveObject);
        }

        #region Data Retrieval

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

        public InteractiveObjectActionInherentData GetCurrentWeaponProjectileFireActionDefinition()
        {
            return this.WeaponHandlingSystemInitializationData.WeaponDefinition.ProjectileFireActionDefinition;
        }

        #endregion


        /// <summary>
        /// Populates the <see cref="ProjectileFireActionInputData"/> with all available data from <see cref="WeaponHandlingSystem"/>.
        /// </summary>
        public void PopulateProjectileFireActionInputData(ref ProjectileFireActionInputData ProjectileFireActionInputData)
        {
            ProjectileFireActionInputData.WeaponReference = this.WeaponReference;
            ProjectileFireActionInputData.ProjectileFireActionDefinition = this.WeaponHandlingSystemInitializationData.WeaponDefinition.ProjectileFireActionDefinition;
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