using CoreGame;
using Input;
using PlayerObject_Interfaces;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace CameraManagement
{
    public struct CameraMovementJobState
    {
        public float d;
        public RigidTransform CameraPivotPointTransform;
        public float CameraSize;

        public CameraFollowState CameraFollowState;
        public CameraOrientationState CameraOrientationState;
        public CameraZoomState CameraZoomState;
    }

    [BurstCompile]
    public struct CameraMovementJob : IJob
    {
        private NativeArray<CameraMovementJobState> CameraMovementJobState;

        public CameraMovementJob(NativeArray<CameraMovementJobState> cameraMovementJobState)
        {
            CameraMovementJobState = cameraMovementJobState;
        }

        public CameraMovementJobState GetCameraMovementJobState()
        {
            return this.CameraMovementJobState[0];
        }

        public void SetCameraMovementJobState(CameraMovementJobState CameraMovementJobState)
        {
            this.CameraMovementJobState[0] = CameraMovementJobState;
        }

        public void Execute()
        {
            var CameraMovementJobStateStruct = this.CameraMovementJobState[0];

            CameraMovementJobStateStruct = CameraFollowSystem.CameraFollowTargetMovement(CameraMovementJobStateStruct);
            CameraMovementJobStateStruct = CameraOrientationSystem.CameraRotation(CameraMovementJobStateStruct);
            CameraMovementJobStateStruct = CameraZoomSystem.CameraZoom(CameraMovementJobStateStruct);

            this.CameraMovementJobState[0] = CameraMovementJobStateStruct;
        }


        public void Dispose()
        {
            this.CameraMovementJobState.Dispose();
        }
    }

    public class CameraMovementJobManager : GameSingleton<CameraMovementJobManager>
    {
        private CameraMovementJob CameraMovementJob;
        private JobHandle CameraMovementJobHandle;

        private CameraFollowSystem _cameraFollowSystem;
        private CameraOrientationSystem _cameraOrientationSystem;
        private CameraZoomSystem _cameraZoomSystem;

        public CameraMovementJobManager()
        {
            this.CameraMovementJob = new CameraMovementJob(new NativeArray<CameraMovementJobState>(1, Allocator.Persistent));

            var CameraPivotPointTransform = GameObject.FindGameObjectWithTag(TagConstants.CAMERA_PIVOT_POINT_TAG).transform;
            this._cameraOrientationSystem = new CameraOrientationSystem(CameraPivotPointTransform, GameInputManager.Get());
            this._cameraZoomSystem = new CameraZoomSystem(Camera.main, GameInputManager.Get());

            /// Initialize state
            var CameraMovementJobStateStruct = this.CameraMovementJob.GetCameraMovementJobState();

            this._cameraOrientationSystem.InitState(ref CameraMovementJobStateStruct);
            this._cameraZoomSystem.InitState(ref CameraMovementJobStateStruct);

            this.CameraMovementJob.SetCameraMovementJobState(CameraMovementJobStateStruct);
        }

        public void InitializeEvents()
        {
            PlayerInteractiveObjectCreatedEvent.Get().RegisterPlayerInteractiveObjectCreatedEvent(this.OnPlayerInteractiveObjectCreated);
            PlayerInteractiveObjectDestroyedEvent.Get().RegisterPlayerInteractiveObjectDestroyedEvent(this.OnPlayerInteractiveObjectDestroyed);
        }

        public void SetupJob(float d)
        {
            var CameraMovementJobStateStruct = this.CameraMovementJob.GetCameraMovementJobState();
            CameraMovementJobStateStruct.d = d;

            this._cameraFollowSystem?.SetupJob(ref CameraMovementJobStateStruct);
            this._cameraOrientationSystem.SetupJob(ref CameraMovementJobStateStruct);
            this._cameraZoomSystem.SetupJob(ref CameraMovementJobStateStruct);

            this.CameraMovementJob.SetCameraMovementJobState(CameraMovementJobStateStruct);

            this.CameraMovementJobHandle = this.CameraMovementJob.Schedule();
        }

        public void Tick()
        {
            this.CameraMovementJobHandle.Complete();

            var CameraMovementJobStateStruct = this.CameraMovementJob.GetCameraMovementJobState();

            this._cameraFollowSystem?.Tick(CameraMovementJobStateStruct);
            this._cameraOrientationSystem.Tick(CameraMovementJobStateStruct);
            this._cameraZoomSystem.Tick(CameraMovementJobStateStruct);
        }

        /// <summary>
        /// Called from <see cref="PlayerInteractiveObjectCreatedEvent"/>.
        /// </summary
        private void OnPlayerInteractiveObjectCreated(IPlayerInteractiveObject IPlayerInteractiveObject)
        {
            var cameraPivotPoint = GameObject.FindGameObjectWithTag(TagConstants.CAMERA_PIVOT_POINT_TAG).transform;

            //set initial position
            cameraPivotPoint.position = IPlayerInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent.transform.position;

            this._cameraFollowSystem = new CameraFollowSystem(IPlayerInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent.transform,
                cameraPivotPoint.transform,
                CameraConfigurationGameObject.Get().CameraMovementConfiguration.CameraFollowManagerComponent);

            var CameraMovementJobStateStruct = this.CameraMovementJob.GetCameraMovementJobState();
            this._cameraFollowSystem.InitState(ref CameraMovementJobStateStruct);
            this.CameraMovementJob.SetCameraMovementJobState(CameraMovementJobStateStruct);
        }

        /// <summary>
        /// Called from <see cref="PlayerInteractiveObjectDestroyedEvent"/>
        /// </summary>
        private void OnPlayerInteractiveObjectDestroyed()
        {
            this._cameraFollowSystem = null;
        }

        #region Logical conditions

        /// <summary>
        /// Camera is considered rotating when the input <see cref="XInput.RotationCameraDH"/> is verified.
        /// /!\ It must be called after <see cref="Tick"/> for state to be updated for the current frame
        /// </summary>
        public bool IsCameraRotating()
        {
            return this.CameraMovementJob.GetCameraMovementJobState().CameraOrientationState.IsRotating;
        }

        #endregion

        public override void OnDestroy()
        {
            base.OnDestroy();
            this.CameraMovementJob.Dispose();
        }
    }
}