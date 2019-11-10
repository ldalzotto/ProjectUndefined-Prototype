using System;
using OdinSerializer;
using UnityEngine;

namespace Weapon
{
    [Serializable]
    [SceneHandleDraw]
    public class WeaponHandlingSystemDefinition : SerializedScriptableObject
    {
        [WireCircleWorld(UseTransform = true, PositionOffsetFieldName = nameof(WeaponHandlingSystemDefinition.WeaponFirePointOriginLocal), Radius = 0.1f)]
        public Vector3 WeaponFirePointOriginLocal;

        [Inline()] public WeaponDefinition WeaponDefinition;
    }
}