using System;
using OdinSerializer;
using UnityEngine;

namespace Weapon
{
    [Serializable]
    public class WeaponHandlingFirePointOriginLocalDefinition : SerializedScriptableObject
    {
        /// <summary>
        /// This point is the starting point of all <see cref="PlayerAim.FiredProjectile"/> fired from the associated <see cref="Weapon"/>.
        /// This point is local to the <see cref="WeaponHandlingSystem"/> associated <see cref="InteractiveObjects.CoreInteractiveObject"/>.
        /// </summary>
        [WireCircleWorld(UseTransform = true, PositionOffsetFieldName = nameof(WeaponHandlingFirePointOriginLocalDefinition.WeaponFirePointOriginLocal), Radius = 0.1f)]
        public Vector3 WeaponFirePointOriginLocal;
    }
}