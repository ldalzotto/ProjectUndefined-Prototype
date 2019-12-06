using System;
using OdinSerializer;
using UnityEngine;

namespace InteractiveObjects_Interfaces
{
    [Serializable]
    public class TransformMoveManagerComponentV3 : SerializedScriptableObject
    {
        public const float DefaultSpeedMultiplicationFactor = 20f;

        [HideInInspector] public bool IsPositionUpdateConstrained = false;
        public float RotationSpeed = 5f;

        /// <summary>
        /// The <see cref="SpeedMultiplicationFactor"/> can be considered as the scale of objects speed.
        /// Every internal speed calculations are done with values clamped to 1 (or Vector.one).
        /// The final speed is then multiplied by this factor.
        /// </summary>
        public float SpeedMultiplicationFactor = DefaultSpeedMultiplicationFactor;

        [Foldable(true, nameof(TransformMoveManagerComponentV3.IsPositionUpdateConstrained))]
        public TransformPositionUpdateConstraints TransformPositionUpdateConstraints;
    }

    [Serializable]
    public class TransformPositionUpdateConstraints
    {
        [Range(0f, 360f)] public float MinAngleThatAllowThePositionUpdate = 45f;
    }
}