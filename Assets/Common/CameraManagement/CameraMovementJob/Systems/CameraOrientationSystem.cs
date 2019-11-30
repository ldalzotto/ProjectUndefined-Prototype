using Input;
using Unity.Mathematics;
using UnityEngine;

namespace CameraManagement
{
    public struct CameraOrientationState
    {
        public bool IsRotating;
        public float LeftRotationFromInput;
        public float RightRotationFromInput;
    }

    public class CameraOrientationSystem
    {
        private Transform CameraPivotPointTransform;
        private GameInputManager gameInputManager;

        public CameraOrientationSystem(Transform CameraPivotPointTransform, GameInputManager gameInputManager)
        {
            this.CameraPivotPointTransform = CameraPivotPointTransform;
            this.gameInputManager = gameInputManager;
        }

        public void InitState(ref CameraMovementJobState CameraMovementJobState)
        {
            CameraMovementJobState.CameraPivotPointTransform.rot = this.CameraPivotPointTransform.transform.rotation;
        }

        public void SetupJob(ref CameraMovementJobState CameraMovementJobState)
        {
            CameraMovementJobState.CameraOrientationState.IsRotating = this.gameInputManager.CurrentInput.RotationCameraDH();
            CameraMovementJobState.CameraOrientationState.LeftRotationFromInput = gameInputManager.CurrentInput.LeftRotationCamera();
            CameraMovementJobState.CameraOrientationState.RightRotationFromInput = gameInputManager.CurrentInput.RightRotationCamera();
        }

        public void Tick(CameraMovementJobState CameraMovementJobState)
        {
            this.CameraPivotPointTransform.transform.rotation = CameraMovementJobState.CameraPivotPointTransform.rot;
        }

        public static CameraMovementJobState CameraRotation(CameraMovementJobState CameraMovementJobStateStruct)
        {
            var CameraOrientationState = CameraMovementJobStateStruct.CameraOrientationState;

            if (CameraOrientationState.IsRotating)
            {
                var deltaRotation = quaternion.AxisAngle(new float3(0, 1, 0), math.radians((CameraOrientationState.LeftRotationFromInput - CameraOrientationState.RightRotationFromInput)) * CameraMovementJobStateStruct.d);
                CameraMovementJobStateStruct.CameraPivotPointTransform.rot = math.mul(deltaRotation, CameraMovementJobStateStruct.CameraPivotPointTransform.rot);
            }

            return CameraMovementJobStateStruct;
        }
    }
}