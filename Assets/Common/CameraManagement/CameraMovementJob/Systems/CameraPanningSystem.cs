using System;
using System.ComponentModel;
using CoreGame;
using Targetting;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace CameraManagement
{
    public struct CameraPanningState
    {
        [Unity.Collections.ReadOnly] public float2 TargetCursorScreenPosition;
        [Unity.Collections.ReadOnly] public float ScreenMovementIntensity;
        [Unity.Collections.ReadOnly] public float DampingSpeed;

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
        #region External Dependencies

        private TargetCursorManager TargetCursorManager = TargetCursorManager.Get();

        #endregion

        private CameraMovementConfiguration CameraMovementConfiguration;

        public CameraPanningSystem(CameraMovementConfiguration CameraMovementConfiguration)
        {
            this.CameraMovementConfiguration = CameraMovementConfiguration;
        }

        public void SetupJob(ref CameraMovementJobState CameraMovementJobState)
        {
            CameraMovementJobState.CameraPanningState.TargetCursorScreenPosition = TargetCursorManager.GetTargetCursorPositionAsDeltaFromCenter();
            CameraMovementJobState.CameraPanningState.ScreenMovementIntensity = this.CameraMovementConfiguration.CameraPanningComponent.ScreenMovementIntensity;
            CameraMovementJobState.CameraPanningState.DampingSpeed = this.CameraMovementConfiguration.CameraPanningComponent.DampingSpeed;
        }

        public static float3 CameraPanningMovement(ref CameraMovementJobState CameraMovementJobStateStruct)
        {
            var CameraObject = CameraMovementJobStateStruct.CameraObject;
            var CameraPanningState = CameraMovementJobStateStruct.CameraPanningState;

            CameraPanningState.TargetWorldOffset =
                math.inverse(math.mul(CameraObject.CameraProjectionMatrix, CameraObject.WorldToCameraMatrix)).MultiplyVector(
                    new float3(CameraPanningState.TargetCursorScreenPosition, 0f)) * CameraPanningState.ScreenMovementIntensity;
            CameraPanningState.CurrentWorldOffset =
                math.lerp(CameraPanningState.CurrentWorldOffset, CameraPanningState.TargetWorldOffset, CameraPanningState.DampingSpeed * CameraMovementJobStateStruct.d);

            CameraMovementJobStateStruct.CameraPanningState = CameraPanningState;

            return CameraPanningState.CurrentWorldOffset;
        }
    }
}