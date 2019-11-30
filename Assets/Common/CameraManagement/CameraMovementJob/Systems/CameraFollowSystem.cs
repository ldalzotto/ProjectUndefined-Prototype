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

        private Transform CameraPivotPointTransform;
        private Transform targetTrasform;
        private Vector3 currentVelocity;

        public CameraFollowSystem(Transform targetTransform, Transform CameraPivotPointTransform, CameraFollowManagerComponent CameraFollowManagerComponent)
        {
            this.targetTrasform = targetTransform;
            this.CameraPivotPointTransform = CameraPivotPointTransform;
            this.CameraFollowManagerComponent = CameraFollowManagerComponent;
        }

        public void InitState(ref CameraMovementJobState CameraMovementJobState)
        {
            CameraMovementJobState.CameraPivotPointTransform.pos = this.CameraPivotPointTransform.transform.position;
        }
        
        public void SetupJob(ref CameraMovementJobState CameraMovementJobState)
        {
            CameraMovementJobState.CameraFollowState.DampingTime = this.CameraFollowManagerComponent.DampTime;
            CameraMovementJobState.CameraFollowState.CameraFollowTargetPosition = targetTrasform.position;
        }

        public void Tick(CameraMovementJobState CameraMovementJobState)
        {
            this.CameraPivotPointTransform.transform.position = CameraMovementJobState.CameraPivotPointTransform.pos;
        }
        
        public static CameraMovementJobState CameraFollowTargetMovement(CameraMovementJobState CameraMovementJobStateStruct)
        {
            var CameraFollowState = CameraMovementJobStateStruct.CameraFollowState;
            CameraMovementJobStateStruct.CameraPivotPointTransform.pos = float3SmoothDamp.SmoothDamp(CameraMovementJobStateStruct.CameraPivotPointTransform.pos,
                CameraFollowState.CameraFollowTargetPosition, ref CameraFollowState.CurrentVelocity, CameraFollowState.DampingTime, float.PositiveInfinity,
                CameraMovementJobStateStruct.d);

            CameraMovementJobStateStruct.CameraFollowState = CameraFollowState;
            return CameraMovementJobStateStruct;
        }
    }
}