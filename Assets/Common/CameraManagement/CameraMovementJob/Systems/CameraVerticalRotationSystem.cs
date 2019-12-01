using System;
using Targetting;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace CameraManagement
{
    public struct CameraVerticalRotationState
    {
        [ReadOnly] public float2 TargetCursorScreenPosition;
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

        #region External Dependencies

        private TargetCursorManager TargetCursorManager = TargetCursorManager.Get();

        #endregion

        public CameraVerticalRotationSystem(CameraMovementConfiguration cameraMovementConfiguration)
        {
            CameraMovementConfiguration = cameraMovementConfiguration;
        }

        public void SetupJob(ref CameraMovementJobState CameraMovementJobState)
        {
            CameraMovementJobState.CameraVerticalRotationState.TargetCursorScreenPosition = this.TargetCursorManager.GetTargetCursorPositionAsDeltaFromCenter();
            CameraMovementJobState.CameraVerticalRotationState.MinAngle = this.CameraMovementConfiguration.CameraVerticalRotationComponent.MinAngle;
            CameraMovementJobState.CameraVerticalRotationState.MaxAngle = this.CameraMovementConfiguration.CameraVerticalRotationComponent.MaxAngle;
            CameraMovementJobState.CameraVerticalRotationState.DampingSpeed = this.CameraMovementConfiguration.CameraVerticalRotationComponent.DampingSpeed;
        }

        public static quaternion CameraVerticalRotation(ref CameraMovementJobState CameraMovementJobStateStruct)
        {
            var CameraObject = CameraMovementJobStateStruct.CameraObject;
            var CameraVerticalRotationState = CameraMovementJobStateStruct.CameraVerticalRotationState;

            var clamperYCursorScreenPosition = 1 - math.clamp(CameraVerticalRotationState.TargetCursorScreenPosition.y, 0, 1);

            var targetVerticalAngle = math.lerp(CameraVerticalRotationState.MinAngle, CameraVerticalRotationState.MaxAngle, clamperYCursorScreenPosition);

            CameraVerticalRotationState.CurrentDeltaAngle = math.lerp(CameraVerticalRotationState.CurrentDeltaAngle, targetVerticalAngle, CameraVerticalRotationState.DampingSpeed * CameraMovementJobStateStruct.d);
            CameraMovementJobStateStruct.CameraVerticalRotationState = CameraVerticalRotationState;
            
            return quaternion.RotateX(math.radians( CameraVerticalRotationState.CurrentDeltaAngle));
        }
    }
}