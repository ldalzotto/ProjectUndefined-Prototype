using System;
using OdinSerializer;
using UnityEngine;

namespace Weapon
{
    [Serializable]
    public class WeaponHandlingFirePointOriginLocalDefinition : SerializedScriptableObject
    {
        [WireCircleWorld(UseTransform = true, PositionOffsetFieldName = nameof(WeaponHandlingFirePointOriginLocalDefinition.WeaponFirePointOriginLocal), Radius = 0.1f)]
        public Vector3 WeaponFirePointOriginLocal;
    }
}