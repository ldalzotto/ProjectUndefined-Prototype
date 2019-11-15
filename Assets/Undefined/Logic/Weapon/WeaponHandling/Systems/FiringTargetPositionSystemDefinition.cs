using System;
using OdinSerializer;
using UnityEngine;

namespace Weapon
{
    [Serializable]
    [SceneHandleDraw]
    public class FiringTargetPositionSystemDefinition : SerializedScriptableObject
    {
        /// <summary>
        /// This is the optimum position local position of where objects should aim to hit the associated <see cref="CoreInteractiveObject"/>. 
        /// </summary>
        [WireCircleWorld(UseTransform = true, PositionOffsetFieldName = nameof(FiringTargetPositionSystemDefinition.TargetPositionPointLocalOffset), Radius = 0.1f)]
        public Vector3 TargetPositionPointLocalOffset;
    }
}