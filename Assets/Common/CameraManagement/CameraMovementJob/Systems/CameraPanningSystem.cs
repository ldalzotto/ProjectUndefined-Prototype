using System;
using CoreGame;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace CameraManagement
{
    public struct CameraPanningState
    {
        [ReadOnly] public float ScreenMovementIntensity;
        [ReadOnly] public float DampingSpeed;

        public float3 CurrentWorldOffset;
        public float3 TargetWorldOffset;
    }

    [Serializable]
    public class CameraPanningComponent
    {
        [Range(0.0f, 1.0f)] public float ScreenMovementIntensity;
        public float DampingSpeed;
    }

    public class CameraPanningSystem
    {
        private CameraMovementConfiguration CameraMovementConfiguration;

        public CameraPanningSystem(CameraMovementConfiguration CameraMovementConfiguration)
        {
            this.CameraMovementConfiguration = CameraMovementConfiguration;
        }

        public void SetupJob(ref CameraMovementJobState CameraMovementJobState)
        {
            CameraMovementJobState.CameraPanningState.ScreenMovementIntensity = this.CameraMovementConfiguration.CameraPanningComponent.ScreenMovementIntensity;
            CameraMovementJobState.CameraPanningState.DampingSpeed = this.CameraMovementConfiguration.CameraPanningComponent.DampingSpeed;
        }

        public static float3 CameraPanningMovement(ref CameraMovementJobState CameraMovementJobStateStruct)
        {
            var TargetCursorComponent = CameraMovementJobStateStruct.TargetCursorComponent;
            var CameraObject = CameraMovementJobStateStruct.CameraObject;
            var CameraPanningState = CameraMovementJobStateStruct.CameraPanningState;

            CameraPanningState.TargetWorldOffset =
                math.inverse(math.mul(CameraObject.CameraProjectionMatrix, CameraObject.WorldToCameraMatrix)).MultiplyVector(
                    new float3(TargetCursorComponent.TargetCursorScreenPosition, 0f)) * CameraPanningState.ScreenMovementIntensity;
            CameraPanningState.CurrentWorldOffset =
                math.lerp(CameraPanningState.CurrentWorldOffset, CameraPanningState.TargetWorldOffset,
                    math.clamp(CameraPanningState.DampingSpeed * CameraMovementJobStateStruct.d, 0f, 1f));

            CameraMovementJobStateStruct.CameraPanningState = CameraPanningState;

            return CameraPanningState.CurrentWorldOffset;
        }
    }
}