﻿using InteractiveObjects;
using UnityEngine;

namespace PlayerAim
{
    public static class FiringProjectilePathCalculation
    {
        /// <summary>
        /// Calculates the initial orientation and position of a <see cref="FiredProjectile"/>.
        /// It is used by WeaponHandlingSystems when <see cref="CoreInteractiveObject.AskToFireAFiredProjectile_ToTargetPoint"/>
        /// </summary>
        public static void CalculateProjectilePath_ToTargetPoint(CoreInteractiveObject WeaponHolder, CoreInteractiveObject Target, out Vector3 StartWorldPosition, out Quaternion StartOrientation)
        {
            StartWorldPosition = WeaponHolder.GetWeaponWorldFirePoint();
            var firingTargetWorldLocation = Target.InteractiveGameObject.GetLocalToWorld().MultiplyPoint(Target.GetFiringTargetLocalPosition());
            StartOrientation = Quaternion.LookRotation((firingTargetWorldLocation - StartWorldPosition).normalized);
        }

        /// <summary>
        /// Calculates the initial orientation and position of a <see cref="FiredProjectile"/> shooted in the direction represented by <<paramref name="NormalizedWorldDirection"/>.
        /// </summary>
        public static void CalculateProjectilePath_ToDirection(CoreInteractiveObject WeaponHolder, Vector3 NormalizedWorldDirection, out Vector3 StartWorldPosition, out Quaternion StartOrientation)
        {
            StartWorldPosition = WeaponHolder.GetWeaponWorldFirePoint();
            StartOrientation = Quaternion.LookRotation(NormalizedWorldDirection);
        }

        /// <summary>
        /// Calculates the initial orientation and position of a <see cref="FiredProjectile"/> when shooting forward
        /// </summary>
        public static void CalculateProjectilePath_Forward(CoreInteractiveObject WeaponHolder, out Vector3 StartWorldPosition, out Quaternion StartOrientation)
        {
            StartWorldPosition = WeaponHolder.GetWeaponWorldFirePoint();
            StartOrientation = Quaternion.LookRotation(WeaponHolder.InteractiveGameObject.InteractiveGameObjectParent.transform.forward);
        }
    }
}