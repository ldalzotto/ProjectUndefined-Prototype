using System;
using OdinSerializer;
using UnityEngine;

namespace InteractiveObjects_Interfaces
{
    [Serializable]
    public class TransformMoveManagerComponentV3 : SerializedScriptableObject
    {
        [HideInInspector] public bool IsPositionUpdateConstrained = false;
        public float RotationSpeed = 5f;
        public float SpeedMultiplicationFactor = 20f;

        [Foldable(true, nameof(TransformMoveManagerComponentV3.IsPositionUpdateConstrained))]
        public TransformPositionUpdateConstraints TransformPositionUpdateConstraints;
    }

    [Serializable]
    public class TransformPositionUpdateConstraints
    {
        [Range(0f, 360f)] public float MinAngleThatAllowThePositionUpdate = 45f;
    }
}