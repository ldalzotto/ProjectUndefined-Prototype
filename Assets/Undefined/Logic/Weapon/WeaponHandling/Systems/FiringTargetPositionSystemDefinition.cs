using System;
using OdinSerializer;
using UnityEngine;

namespace Weapon
{
    [Serializable]
    [SceneHandleDraw]
    public class FiringTargetPositionSystemDefinition : SerializedScriptableObject
    {
        [WireCircleWorld(UseTransform = true, PositionOffsetFieldName = nameof(FiringTargetPositionSystemDefinition.TargetPositionPointLocalOffset), Radius = 0.1f)]
        public Vector3 TargetPositionPointLocalOffset;
    }
}