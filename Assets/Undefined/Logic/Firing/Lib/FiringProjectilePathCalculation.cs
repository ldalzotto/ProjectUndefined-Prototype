using InteractiveObjects;
using UnityEngine;

namespace Firing
{
    public static class FiringProjectilePathCalculation
    {
        /// <summary>
        /// Calculates the initial orientation and position of a FiredProjectile.
        /// It is used by WeaponHandlingSystems when <see cref="CoreInteractiveObject.AskToFireAFiredProjectile_ToTargetPoint"/>
        /// </summary>
        public static void CalculateProjectilePath_ToTargetPoint(CoreInteractiveObject WeaponHolder, CoreInteractiveObject Target, out Vector3 StartWorldPosition, out Quaternion StartOrientation)
        {
            StartWorldPosition = WeaponHolder.GetWeaponWorldFirePoint();
            var firingTargetWorldLocation = Target.InteractiveGameObject.GetLocalToWorld().MultiplyPoint(Target.GetFiringTargetLocalPosition());
            StartOrientation = Quaternion.LookRotation((firingTargetWorldLocation - StartWorldPosition).normalized);
        }

        /// <summary>
        /// Calculates the initial orientation and position of a FiredProjectile when shooting forward
        /// </summary>
        public static void CalculateProjectilePath_Forward(CoreInteractiveObject WeaponHolder, out Vector3 StartWorldPosition, out Quaternion StartOrientation)
        {
            StartWorldPosition = WeaponHolder.GetWeaponWorldFirePoint();
            StartOrientation = Quaternion.LookRotation(WeaponHolder.InteractiveGameObject.InteractiveGameObjectParent.transform.forward);
        }
    }
}