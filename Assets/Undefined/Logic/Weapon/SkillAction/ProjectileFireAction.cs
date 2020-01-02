using Firing;
using InteractiveObjects;
using SkillAction;
using UnityEngine;
using Weapon;

namespace Weapon
{
    public struct ProjectileFireActionInputData
    {
        public Weapon WeaponReference;
        public ProjectileFireActionDefinition ProjectileFireActionDefinition;
        public Vector3 NormalizedWorldDirection;

        public ProjectileFireActionInputData(Weapon weaponReference, ProjectileFireActionDefinition projectileFireActionDefinition, Vector3 normalizedWorldDirection)
        {
            WeaponReference = weaponReference;
            ProjectileFireActionDefinition = projectileFireActionDefinition;
            NormalizedWorldDirection = normalizedWorldDirection;
        }
    }

    public class ProjectileFireAction : SkillAction.SkillAction
    {
        private ProjectileFireActionInputData ProjectileFireActionInputData;

        public ProjectileFireAction(ProjectileFireActionInputData ProjectileFireActionInputData) : base(ProjectileFireActionInputData.ProjectileFireActionDefinition)
        {
            this.ProjectileFireActionInputData = ProjectileFireActionInputData;
        }

        public override void OnActionStart()
        {
            FiringProjectilePathCalculation.CalculateProjectilePath_ToDirection(this.ProjectileFireActionInputData.WeaponReference.WeaponHolder, this.ProjectileFireActionInputData.NormalizedWorldDirection, out Vector3 firedProjectilePosition, out Quaternion firedProjectileRotation);
            this.ProjectileFireActionInputData.WeaponReference.SpawnFiredProjectile(new TransformStruct() {WorldPosition = firedProjectilePosition, WorldRotationEuler = firedProjectileRotation.eulerAngles});
        }

        public override bool HasEnded()
        {
            return true;
        }
    }
}