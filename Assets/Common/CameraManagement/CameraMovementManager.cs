using System;
using CoreGame;
using Input;
using PlayerObject_Interfaces;
using UnityEngine;

namespace CameraManagement
{
    public class CameraMovementManager : GameSingleton<CameraMovementManager>
    {
        private CameraFollowManager CameraFollowManager;
        private CameraOrientationManager CameraOrientationManager;
        private CameraZoomManager CameraZoomManager;

        public void InitializeEvents()
        {
            PlayerInteractiveObjectCreatedEvent.Get().RegisterPlayerInteractiveObjectCreatedEvent(this.OnPlayerInteractiveObjectCreated);
            PlayerInteractiveObjectDestroyedEvent.Get().RegisterPlayerInteractiveObjectDestroyedEvent(this.OnPlayerInteractiveObjectDestroyed);
        }
        
        public void Init()
        {
            var cameraPivotPoint = GameObject.FindGameObjectWithTag(TagConstants.CAMERA_PIVOT_POINT_TAG).transform;
            this.CameraOrientationManager = new CameraOrientationManager(cameraPivotPoint, GameInputManager.Get(), InputConfigurationGameObject.Get().CoreInputConfiguration);
            this.CameraZoomManager = new CameraZoomManager(Camera.main, GameInputManager.Get());
        }

        public void Tick(float unscaled)
        {
            this.CameraFollowManager?.Tick(unscaled);
            this.CameraOrientationManager.Tick(unscaled);
            this.CameraZoomManager.Tick(unscaled);
        }

        #region External Events
        
        /// <summary>
        /// Called from <see cref="PlayerInteractiveObjectCreatedEvent"/>.
        /// </summary
        private void OnPlayerInteractiveObjectCreated(IPlayerInteractiveObject IPlayerInteractiveObject)
        {
            var cameraPivotPoint = GameObject.FindGameObjectWithTag(TagConstants.CAMERA_PIVOT_POINT_TAG).transform;
            this.CameraFollowManager = new CameraFollowManager(IPlayerInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent.transform, cameraPivotPoint,
                CameraConfigurationGameObject.Get().CameraMovementConfiguration.CameraFollowManagerComponent);
        }

        /// <summary>
        /// Called from <see cref="PlayerInteractiveObjectDestroyedEvent"/>
        /// </summary>
        private void OnPlayerInteractiveObjectDestroyed()
        {
            this.CameraFollowManager = null;
        }

        #endregion

        #region Logical conditions

        /// <summary>
        /// Camera is considered rotating when the input <see cref="XInput.RotationCameraDH"/> is verified.
        /// </summary>
        public bool IsCameraRotating()
        {
            return this.CameraOrientationManager.IsRotating;
        }

        #endregion
    }

    [Serializable]
    public class CameraFollowManagerComponent
    {
        public float DampTime;
    }

    public class CameraFollowManager
    {
        private CameraFollowManagerComponent CameraFollowManagerComponent;

        private Transform targetTrasform;
        private Transform cameraPivotPoint;

        private Vector3 currentVelocity;

        public CameraFollowManager(Transform targetTransform, Transform cameraPivotPoint, CameraFollowManagerComponent CameraFollowManagerComponent)
        {
            this.targetTrasform = targetTransform;
            this.cameraPivotPoint = cameraPivotPoint;
            this.CameraFollowManagerComponent = CameraFollowManagerComponent;
            //set initial position
            this.cameraPivotPoint.position = this.targetTrasform.position;
        }

        public void Tick(float d)
        {
            cameraPivotPoint.position = Vector3.SmoothDamp(cameraPivotPoint.position, targetTrasform.position, ref currentVelocity, this.CameraFollowManagerComponent.DampTime);
        }

        #region Data Retrieval

        public Transform GetCameraPivotPointTransform()
        {
            return this.cameraPivotPoint;
        }

        #endregion
    }

    public class CameraOrientationManager
    {
        private CoreInputConfiguration CoreInputConfiguration;
        private Transform cameraPivotPoint;
        private GameInputManager gameInputManager;

        private float targetAngle;

        #region State

        public bool IsRotating { get; private set; } = false;
        private bool isRotatingTowardsAtarget = false;
        private bool inputEnabled = true;

        #endregion

        public CameraOrientationManager(Transform cameraPivotPoint, GameInputManager gameInputManager, CoreInputConfiguration CoreInputConfiguration)
        {
            this.cameraPivotPoint = cameraPivotPoint;
            this.gameInputManager = gameInputManager;
            this.CoreInputConfiguration = CoreInputConfiguration;
        }

        public void Tick(float d)
        {
            this.IsRotating = this.gameInputManager.CurrentInput.RotationCameraDH();

            if (this.IsRotating)
            {
                Vector3 rotationVector = Vector3.zero;
                if (this.isRotatingTowardsAtarget)
                {
                    float initialY = this.cameraPivotPoint.transform.rotation.eulerAngles.y;
                    float deltaAngle = Mathf.Lerp(initialY, this.targetAngle, 0.1f) - initialY;
                    rotationVector = new Vector3(0, Mathf.Abs(deltaAngle) * Mathf.Sign(this.targetAngle - initialY), 0);
                }
                else if (this.inputEnabled)
                {
                    rotationVector = new Vector3(0, (gameInputManager.CurrentInput.LeftRotationCamera() - gameInputManager.CurrentInput.RightRotationCamera()) * d, 0);
                }

                if (Mathf.Abs(rotationVector.y) <= 0.001)
                {
                    if (this.isRotatingTowardsAtarget)
                    {
                        cameraPivotPoint.eulerAngles = new Vector3(0, this.targetAngle, 0);
                    }

                    this.isRotatingTowardsAtarget = false;
                }
                else
                {
                    cameraPivotPoint.eulerAngles += rotationVector;
                }
            }
          
        }

    }

    public class CameraZoomManager
    {
        private Camera camera;
        private GameInputManager IGameInputManager;

        private float TargetSize;

        private float MinCameraSize = 35f;
        private float MaxCameraSize = 70f;

        public CameraZoomManager(Camera camera, GameInputManager gameInputManager)
        {
            this.camera = camera;
            this.IGameInputManager = gameInputManager;
            this.TargetSize = camera.orthographicSize;
        }

        public void Tick(float d)
        {
            var zoomDelta = -1 * this.IGameInputManager.CurrentInput.CameraZoom() * d;
            if (zoomDelta != 0f)
            {
                this.TargetSize = Mathf.Clamp(this.TargetSize + zoomDelta, this.MinCameraSize, this.MaxCameraSize);
            }

            if (this.TargetSize != this.camera.orthographicSize)
            {
                this.camera.orthographicSize = Mathf.Lerp(this.camera.orthographicSize, this.TargetSize, d * 4f);
            }
            else
            {
                this.camera.orthographicSize = this.TargetSize;
            }
        }
    }
}