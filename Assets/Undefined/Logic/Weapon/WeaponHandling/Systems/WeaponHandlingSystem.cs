using Firing;
using InteractiveObjects;
using SkillAction;
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

        public WeaponHandlingSystem(CoreInteractiveObject AssociatedInteractiveObject, WeaponHandlingSystemInitializationData weaponHandlingSystemInitializationData)
        {
            WeaponHandlingSystemInitializationData = weaponHandlingSystemInitializationData;
            this.WeaponReference = this.WeaponHandlingSystemInitializationData.WeaponDefinition.BuildWeapon(AssociatedInteractiveObject);
        }

        /// <summary>
        /// Spawns a fired projectile at <see cref="GetWorldWeaponFirePoint"/> pointing in the <see cref="NormalizedWorldDirection"/> direction. 
        /// </summary>
        public void AskToFireAFiredProjectile_ToDirection(Vector3 NormalizedWorldDirection)
        {
            var interactiveObject_GameActionExection_Casted = (this.WeaponReference.WeaponHolder as IEM_SkillActionExecution);
            if (interactiveObject_GameActionExection_Casted != null && interactiveObject_GameActionExection_Casted.ActionAuthorizedToBeExecuted(this.WeaponHandlingSystemInitializationData.WeaponDefinition.ProjectileFireActionDefinition))
            {
                interactiveObject_GameActionExection_Casted.ExecuteSkillAction(new ProjectileFireAction(
                    new ProjectileFireActionInputData(this.WeaponReference, this.WeaponHandlingSystemInitializationData.WeaponDefinition.ProjectileFireActionDefinition, NormalizedWorldDirection)));
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