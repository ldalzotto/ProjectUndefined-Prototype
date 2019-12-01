using System;
using CoreGame;
using Unity.Mathematics;
using UnityEngine;

namespace CameraManagement
{
    public struct CameraFollowState
    {
        public float3 CameraFollowTargetPosition;
        public float DampingTime;
        public float3 CurrentVelocity;
    }

    public class CameraFollowSystem
    {
        private CameraFollowManagerComponent CameraFollowManagerComponent;

        private Transform targetTrasform;

        public CameraFollowSystem(Transform targetTransform, CameraFollowManagerComponent CameraFollowManagerComponent)
        {
            this.targetTrasform = targetTransform;
            this.CameraFollowManagerComponent = CameraFollowManagerComponent;
        }

        public void InitState(ref CameraMovementJobState CameraMovementJobState)
        {
            PopulateCameraMovementState(ref CameraMovementJobState);
        }

        public void SetupJob(ref CameraMovementJobState CameraMovementJobState)
        {
            PopulateCameraMovementState(ref CameraMovementJobState);
        }

        private void PopulateCameraMovementState(ref CameraMovementJobState CameraMovementJobState)
        {
            CameraMovementJobState.CameraFollowState.DampingTime = this.CameraFollowManagerComponent.DampTime;
            CameraMovementJobState.CameraFollowState.CameraFollowTargetPosition = targetTrasform.position;
        }

        public static float3 CameraFollowTargetMovement(ref CameraMovementJobState CameraMovementJobStateStruct)
        {
            var CameraObject = CameraMovementJobStateStruct.CameraObject;
            var CameraFollowState = CameraMovementJobStateStruct.CameraFollowState;

            var smoothedPivotPointPosition = float3SmoothDamp.SmoothDamp(CameraObject.CameraPivotPointTransformWithoutOffset.pos,
                CameraFollowState.CameraFollowTargetPosition, ref CameraFollowState.CurrentVelocity, CameraFollowState.DampingTime, float.PositiveInfinity,
                CameraMovementJobStateStruct.d);

            CameraMovementJobStateStruct.UpdateCameraFollowState(CameraFollowState);

            return smoothedPivotPointPosition;
        }
    }
}