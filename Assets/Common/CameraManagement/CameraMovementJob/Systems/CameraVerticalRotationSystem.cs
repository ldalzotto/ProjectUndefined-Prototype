using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

namespace CameraManagement
{
    public struct CameraVerticalRotationState
    {
        [ReadOnly] public float MinAngle;
        [ReadOnly] public float MaxAngle;
        [ReadOnly] public float DampingSpeed;

        public float CurrentDeltaAngle;
    }

    [Serializable]
    public class CameraVerticalRotationComponent
    {
        public float MinAngle;
        public float MaxAngle;
        public float DampingSpeed;
    }

    public class CameraVerticalRotationSystem
    {
        private CameraMovementConfiguration CameraMovementConfiguration;

        public CameraVerticalRotationSystem(CameraMovementConfiguration cameraMovementConfiguration)
        {
            CameraMovementConfiguration = cameraMovementConfiguration;
        }

        public void InitState(ref CameraMovementJobState CameraMovementJobState)
        {
            CameraMovementJobState.CameraVerticalRotationState.CurrentDeltaAngle = 0f;
        }

        public void SetupJob(ref CameraMovementJobState CameraMovementJobState)
        {
            CameraMovementJobState.CameraVerticalRotationState.MinAngle = this.CameraMovementConfiguration.CameraVerticalRotationComponent.MinAngle;
            CameraMovementJobState.CameraVerticalRotationState.MaxAngle = this.CameraMovementConfiguration.CameraVerticalRotationComponent.MaxAngle;
            CameraMovementJobState.CameraVerticalRotationState.DampingSpeed = this.CameraMovementConfiguration.CameraVerticalRotationComponent.DampingSpeed;
        }

        public static quaternion CameraVerticalRotation(ref CameraMovementJobState CameraMovementJobStateStruct)
        {
            var TargetCursorComponent = CameraMovementJobStateStruct.TargetCursorComponent;
            var CameraObject = CameraMovementJobStateStruct.CameraObject;
            var CameraVerticalRotationState = CameraMovementJobStateStruct.CameraVerticalRotationState;

            var clampedYCursorScreenPosition = 1 - math.clamp(TargetCursorComponent.TargetCursorScreenPosition.y, 0, 1);

            var targetVerticalAngle = math.lerp(CameraVerticalRotationState.MinAngle, CameraVerticalRotationState.MaxAngle, clampedYCursorScreenPosition);

            CameraVerticalRotationState.CurrentDeltaAngle =
                math.lerp(CameraVerticalRotationState.CurrentDeltaAngle, targetVerticalAngle,
                    math.clamp(CameraVerticalRotationState.DampingSpeed * CameraMovementJobStateStruct.d, 0f, 1f));
            CameraMovementJobStateStruct.CameraVerticalRotationState = CameraVerticalRotationState;

            return quaternion.RotateX(math.radians(CameraVerticalRotationState.CurrentDeltaAngle));
        }
    }
}