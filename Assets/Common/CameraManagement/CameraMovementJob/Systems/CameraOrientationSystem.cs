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
        private GameInputManager gameInputManager;

        public CameraOrientationSystem(GameInputManager gameInputManager)
        {
            this.gameInputManager = gameInputManager;
        }

        public void SetupJob(ref CameraMovementJobState CameraMovementJobState)
        {
            CameraMovementJobState.CameraOrientationState.IsRotating = this.gameInputManager.CurrentInput.RotationCameraDH();
            CameraMovementJobState.CameraOrientationState.LeftRotationFromInput = gameInputManager.CurrentInput.LeftRotationCamera();
            CameraMovementJobState.CameraOrientationState.RightRotationFromInput = gameInputManager.CurrentInput.RightRotationCamera();
        }

        public static quaternion CameraRotation(in CameraMovementJobState CameraMovementJobStateStruct)
        {
            var CameraOrientationState = CameraMovementJobStateStruct.CameraOrientationState;
            var CameraObject = CameraMovementJobStateStruct.CameraObject;

            if (CameraOrientationState.IsRotating)
            {
                return quaternion.AxisAngle(new float3(0, 1, 0), math.radians((CameraOrientationState.LeftRotationFromInput - CameraOrientationState.RightRotationFromInput)) * CameraMovementJobStateStruct.d);
            }
            else
            {
                return quaternion.identity;
            }
        }
    }
}