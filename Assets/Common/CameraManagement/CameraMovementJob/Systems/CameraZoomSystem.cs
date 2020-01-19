using Input;
using Unity.Mathematics;
using UnityEngine;

namespace CameraManagement
{
    public struct CameraZoomState
    {
        public float TargetSize;
        public float DelteZoomFromInput;

        public void SetTargetSize(float targetSize)
        {
            this.TargetSize = targetSize;
        }
    }

    public class CameraZoomSystem
    {
        public const float MinCameraSize = 35f;
        public const float MaxCameraSize = 70f;

        private GameInputManager gameInputManager;
        private Camera MainCamera;

        public CameraZoomSystem(Camera MainCamera, GameInputManager gameInputManager)
        {
            this.MainCamera = MainCamera;
            this.gameInputManager = gameInputManager;
        }

        public void InitState(ref CameraMovementJobState CameraMovementJobState)
        {
            CameraMovementJobState.CameraZoomState.TargetSize = MainCamera.orthographicSize;
        }

        public void SetupJob(ref CameraMovementJobState CameraMovementJobState)
        {
            CameraMovementJobState.CameraZoomState.DelteZoomFromInput = this.gameInputManager.CurrentInput.CameraZoom();
        }

        public static float CameraZoom(ref CameraMovementJobState CameraMovementJobStateStruct)
        {
            var CameraObject = CameraMovementJobStateStruct.CameraObject;
            var CameraZoomState = CameraMovementJobStateStruct.CameraZoomState;

            var zoomDelta = -1 * CameraZoomState.DelteZoomFromInput * CameraMovementJobStateStruct.d;
            if (zoomDelta != 0f)
            {
                CameraZoomState.TargetSize = math.clamp(CameraZoomState.TargetSize + zoomDelta, MinCameraSize, MaxCameraSize);
            }

            CameraMovementJobStateStruct.CameraZoomState = CameraZoomState;

            return math.lerp(CameraObject.CameraSize, CameraZoomState.TargetSize, CameraMovementJobStateStruct.d * 4f);
        }
    }
}