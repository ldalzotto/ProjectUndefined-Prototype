using Input;
using Unity.Mathematics;
using UnityEngine;

namespace CameraManagement
{
    public struct CameraZoomState
    {
        public float TargetSize;
        public float DelteZoomFromInput;
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
            CameraMovementJobState.CameraSize = MainCamera.orthographicSize;
            CameraMovementJobState.CameraZoomState.TargetSize = MainCamera.orthographicSize;
        }

        public void SetupJob(ref CameraMovementJobState CameraMovementJobState)
        {
            CameraMovementJobState.CameraZoomState.DelteZoomFromInput = this.gameInputManager.CurrentInput.CameraZoom();
        }

        public void Tick(CameraMovementJobState CameraMovementJobState)
        {
            this.MainCamera.orthographicSize = CameraMovementJobState.CameraSize;
        }
        
        public static CameraMovementJobState CameraZoom(CameraMovementJobState CameraMovementJobStateStruct)
        {
            var CameraZoomState = CameraMovementJobStateStruct.CameraZoomState;

            var zoomDelta = -1 * CameraZoomState.DelteZoomFromInput * CameraMovementJobStateStruct.d;
            if (zoomDelta != 0f)
            {
                CameraZoomState.TargetSize = math.clamp(CameraZoomState.TargetSize + zoomDelta, MinCameraSize, MaxCameraSize);
            }

            CameraMovementJobStateStruct.CameraSize = math.lerp(CameraMovementJobStateStruct.CameraSize, CameraZoomState.TargetSize, CameraMovementJobStateStruct.d * 4f);

            CameraMovementJobStateStruct.CameraZoomState = CameraZoomState;

            return CameraMovementJobStateStruct;
        }
    }
}