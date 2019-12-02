using CoreGame;
using Input;
using PlayerObject_Interfaces;
using Targetting;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

namespace CameraManagement
{
    public struct CameraMovementJobState
    {
        public float d;

        public CameraObject CameraObject;
        public TargetCursorComponent TargetCursorComponent;

        public CameraFollowState CameraFollowState;
        public CameraOrientationState CameraOrientationState;
        public CameraPanningState CameraPanningState;
        public CameraVerticalRotationState CameraVerticalRotationState;
        public CameraZoomState CameraZoomState;

        public void SetupForJob(float d, Camera camera)
        {
            this.d = d;
            this.CameraObject.SetupForJob(camera);
        }

        public void UpdateCameraFollowState(CameraFollowState CameraFollowState)
        {
            this.CameraFollowState = CameraFollowState;
        }
    }

    public struct TargetCursorComponent
    {
        [ReadOnly] public float2 TargetCursorScreenPosition;
    }

    public struct CameraObject
    {
        /// <summary>
        /// Ofset of <see cref="CameraPanningState.CameraPanningWorldOffset"/> are not taken into account here
        /// </summary>
        public RigidTransform CameraPivotPointTransformWithoutOffset;

        public float CameraSize;
        public float4x4 CameraProjectionMatrix;
        public float4x4 WorldToCameraMatrix;

        public void Initialize(Transform cameraPivotPoint, Camera camera)
        {
            this.CameraPivotPointTransformWithoutOffset.pos = cameraPivotPoint.transform.position;

            var cameraPivotPointRotationEuler = cameraPivotPoint.transform.rotation.eulerAngles;
            
            /// The vertical rotation is resetted to ensure that rotation delta are not accumulated. 
            cameraPivotPointRotationEuler.x = 0f;
            cameraPivotPoint.transform.rotation = Quaternion.Euler(cameraPivotPointRotationEuler);
            
            this.CameraPivotPointTransformWithoutOffset.rot = cameraPivotPoint.transform.rotation;

            this.CameraSize = camera.orthographicSize;
            this.WorldToCameraMatrix = camera.worldToCameraMatrix;
            this.CameraProjectionMatrix = camera.projectionMatrix;
        }

        public void SetupForJob(Camera camera)
        {
            this.WorldToCameraMatrix = camera.worldToCameraMatrix;
            this.CameraProjectionMatrix = camera.projectionMatrix;
        }
    }

    [BurstCompile]
    public struct CameraMovementJob : IJobParallelForTransform
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

        public void Execute(int index, TransformAccess transform)
        {
            var CameraMovementJobStateStruct = this.CameraMovementJobState[0];

            var cameraFollowPosition = CameraFollowSystem.CameraFollowTargetMovement(ref CameraMovementJobStateStruct);
            var cameraDeltaRotation = CameraOrientationSystem.CameraRotation(in CameraMovementJobStateStruct);
            var cameraPanningDeltaPosition = CameraPanningSystem.CameraPanningMovement(ref CameraMovementJobStateStruct);
            var cameraVerticalRotationDelta = CameraVerticalRotationSystem.CameraVerticalRotation(ref CameraMovementJobStateStruct);
            var cameraZoomValue = CameraZoomSystem.CameraZoom(ref CameraMovementJobStateStruct);

            var CameraObject = CameraMovementJobStateStruct.CameraObject;

            var rotation = math.mul(cameraDeltaRotation, CameraObject.CameraPivotPointTransformWithoutOffset.rot);

            CameraObject.CameraPivotPointTransformWithoutOffset = new RigidTransform(rotation, cameraFollowPosition);
            CameraObject.CameraSize = cameraZoomValue;

            CameraMovementJobStateStruct.CameraObject = CameraObject;

            this.CameraMovementJobState[0] = CameraMovementJobStateStruct;
            
            transform.position = cameraFollowPosition + cameraPanningDeltaPosition;
            transform.rotation = math.mul(rotation, cameraVerticalRotationDelta);
        }


        public void Dispose()
        {
            this.CameraMovementJobState.Dispose();
        }
    }

    public class CameraMovementJobManager : GameSingleton<CameraMovementJobManager>
    {
        #region External Dependencies

        private TargetCursorManager TargetCursorManager = TargetCursorManager.Get();

        #endregion

        private CameraMovementJob CameraMovementJob;
        private JobHandle CameraMovementJobHandle;

        private TransformAccessArray CameraPivotPointTransform;
        private Camera MainCamera;

        private CameraFollowSystem _cameraFollowSystem;
        private CameraOrientationSystem _cameraOrientationSystem;
        private CameraPanningSystem CameraPanningSystem;
        private CameraVerticalRotationSystem CameraVerticalRotationSystem;
        private CameraZoomSystem _cameraZoomSystem;


        public CameraMovementJobManager()
        {
            this.MainCamera = Camera.main;
            this.CameraPivotPointTransform = new TransformAccessArray(new Transform[] {GameObject.FindGameObjectWithTag(TagConstants.CAMERA_PIVOT_POINT_TAG).transform});

            this.CameraMovementJob = new CameraMovementJob(new NativeArray<CameraMovementJobState>(1, Allocator.Persistent));

            this._cameraOrientationSystem = new CameraOrientationSystem(GameInputManager.Get());
            this.CameraPanningSystem = new CameraPanningSystem(CameraConfigurationGameObject.Get().CameraMovementConfiguration);
            this.CameraVerticalRotationSystem = new CameraVerticalRotationSystem(CameraConfigurationGameObject.Get().CameraMovementConfiguration);
            this._cameraZoomSystem = new CameraZoomSystem(this.MainCamera, GameInputManager.Get());

            /// InitState state
            var CameraMovementJobStateStruct = this.CameraMovementJob.GetCameraMovementJobState();

            CameraMovementJobStateStruct.CameraObject.Initialize(this.CameraPivotPointTransform[0], this.MainCamera);
            this._cameraZoomSystem.InitState(ref CameraMovementJobStateStruct);
            this.CameraVerticalRotationSystem.InitState(ref CameraMovementJobStateStruct);

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

            CameraMovementJobStateStruct.CameraObject.SetupForJob(this.MainCamera);
            CameraMovementJobStateStruct.TargetCursorComponent.TargetCursorScreenPosition = TargetCursorManager.GetTargetCursorPositionAsDeltaFromCenter();


            this._cameraFollowSystem?.SetupJob(ref CameraMovementJobStateStruct);
            this._cameraOrientationSystem.SetupJob(ref CameraMovementJobStateStruct);
            this.CameraPanningSystem.SetupJob(ref CameraMovementJobStateStruct);
            this.CameraVerticalRotationSystem.SetupJob(ref CameraMovementJobStateStruct);
            this._cameraZoomSystem.SetupJob(ref CameraMovementJobStateStruct);

            this.CameraMovementJob.SetCameraMovementJobState(CameraMovementJobStateStruct);

            this.CameraMovementJobHandle = this.CameraMovementJob.Schedule(this.CameraPivotPointTransform);
        }

        public void Tick()
        {
            this.CameraMovementJobHandle.Complete();

            var CameraMovementJobStateStruct = this.CameraMovementJob.GetCameraMovementJobState();

            this.MainCamera.orthographicSize = CameraMovementJobStateStruct.CameraObject.CameraSize;
        }

        /// <summary>
        /// Called from <see cref="PlayerInteractiveObjectCreatedEvent"/>.
        /// </summary
        private void OnPlayerInteractiveObjectCreated(IPlayerInteractiveObject IPlayerInteractiveObject)
        {
            //set initial position
            this.CameraPivotPointTransform[0].position = IPlayerInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent.transform.position;

            this._cameraFollowSystem = new CameraFollowSystem(IPlayerInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent.transform,
                CameraConfigurationGameObject.Get().CameraMovementConfiguration.CameraFollowManagerComponent);

            var CameraMovementJobStateStruct = this.CameraMovementJob.GetCameraMovementJobState();

            CameraMovementJobStateStruct.CameraObject.Initialize(CameraPivotPointTransform[0], MainCamera);
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
            this.CameraPivotPointTransform.Dispose();
            this.CameraMovementJob.Dispose();
        }
    }
}