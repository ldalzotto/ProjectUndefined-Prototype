using System;
using UnityEngine;

namespace CoreGame
{
    [Serializable]
    public class TransformMoveManagerComponentV3
    {
        [HideInInspector] public bool IsPositionUpdateConstrained = false;
        public float RotationSpeed = 5f;
        public float SpeedMultiplicationFactor = 20f;

        [Foldable(true, nameof(IsPositionUpdateConstrained))]
        public TransformPositionUpdateConstraints TransformPositionUpdateConstraints;
    }

    [Serializable]
    public class TransformPositionUpdateConstraints
    {
        [Range(0f, 360f)] public float MinAngleThatAllowThePositionUpdate = 45f;
    }
}