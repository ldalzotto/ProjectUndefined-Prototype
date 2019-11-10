using System;
using UnityEngine;

namespace Weapon
{
    [Serializable]
    [SceneHandleDraw]
    public class FiringTargetPositionSystemDefinition
    {
        [WireCircleWorld(UseTransform = true, PositionOffsetFieldName = nameof(FiringTargetPositionSystemDefinition.TargetPositionPointLocalOffset), Radius = 0.1f)]
        public Vector3 TargetPositionPointLocalOffset;
    }
}