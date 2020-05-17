﻿using System;
using CoreGame;
using Input;
using UnityEngine;

namespace CameraManagement
{
    public class CameraMovementManager : GameSingleton<CameraMovementManager>
    {
        private CameraFollowManager CameraFollowManager;
        private CameraOrientationManager CameraOrientationManager;
        private CameraZoomManager CameraZoomManager;

        public void Init()
        {
            var playerPosition = GameObject.FindGameObjectWithTag(TagConstants.PLAYER_TAG).transform;
            var cameraPivotPoint = GameObject.FindGameObjectWithTag(TagConstants.CAMERA_PIVOT_POINT_TAG).transform;

            this.CameraFollowManager = new CameraFollowManager(playerPosition, cameraPivotPoint, CameraConfigurationGameObject.Get().CameraMovementConfiguration.CameraFollowManagerComponent);
            this.CameraOrientationManager = new CameraOrientationManager(cameraPivotPoint, GameInputManager.Get(), InputConfigurationGameObject.Get().CoreInputConfiguration);
            this.CameraZoomManager = new CameraZoomManager(Camera.main, GameInputManager.Get());
        }

        public void Tick(float d)
        {
            this.CameraFollowManager.Tick(d);
            this.CameraOrientationManager.Tick(d);
            this.CameraZoomManager.Tick(d);
        }

        #region External Events

        public void SetCameraFollowTarget(Transform followTarget)
        {
            this.CameraFollowManager.SetCameraFollowTarget(followTarget);
        }

        public void SetTargetAngle(float targetAngle)
        {
            this.CameraOrientationManager.SetTargetAngle(targetAngle);
        }

        public void DisableInput()
        {
            this.CameraOrientationManager.DisableInput();
        }

        public void EnableInput()
        {
            this.CameraOrientationManager.EnableInput();
        }

        #endregion

        #region Logical conditions

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

        public void SetCameraFollowTarget(Transform followTarget)
        {
            this.targetTrasform = followTarget;
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

        private bool isRotating = false;
        private bool isRotatingTowardsAtarget = false;
        private bool inputEnabled = true;

        #endregion

        public CameraOrientationManager(Transform cameraPivotPoint, GameInputManager gameInputManager, CoreInputConfiguration CoreInputConfiguration)
        {
            this.cameraPivotPoint = cameraPivotPoint;
            this.gameInputManager = gameInputManager;
            this.CoreInputConfiguration = CoreInputConfiguration;
        }

        public bool IsRotating
        {
            get => isRotating;
        }

        public void Tick(float d)
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
                rotationVector = new Vector3(0, (gameInputManager.CurrentInput.LeftRotationCameraDH() - gameInputManager.CurrentInput.RightRotationCameraDH()) * d, 0);
            }

            if (Mathf.Abs(rotationVector.y) <= 0.001)
            {
                if (this.isRotatingTowardsAtarget)
                {
                    cameraPivotPoint.eulerAngles = new Vector3(0, this.targetAngle, 0);
                }

                this.isRotatingTowardsAtarget = false;
                this.isRotating = false;
            }
            else
            {
                this.isRotating = true;
                cameraPivotPoint.eulerAngles += rotationVector;
            }
        }

        public void SetTargetAngle(float targetAngle)
        {
            this.targetAngle = targetAngle;
            this.isRotatingTowardsAtarget = true;
        }

        public void DisableInput()
        {
            this.inputEnabled = false;
        }

        public void EnableInput()
        {
            this.inputEnabled = true;
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