using CoreGame;
using Input;
using UnityEngine;

namespace PlayerObject
{
    #region Player Movement

    /// <summary>
    /// Process Input to calculate <see cref="PlayerInteractiveObject"/> <see cref="Rigidbody"/> rotation and velocity.
    /// </summary>
    public class PlayerRigidBodyMoveManager
    {
        private float SpeedMultiplicationFactor;
        protected Rigidbody PlayerRigidBody;
        private Transform CameraPivotPoint;
        private GameInputManager GameInputManager;
        protected PlayerSpeedProcessingInput playerSpeedProcessingInput;
        
        public PlayerRigidBodyMoveManager(float SpeedMultiplicationFactor, Rigidbody playerRigidBody, Transform cameraPivotPoint)
        {
            this.SpeedMultiplicationFactor = SpeedMultiplicationFactor;
            PlayerRigidBody = playerRigidBody;
            this.GameInputManager = GameInputManager.Get();
            this.CameraPivotPoint = cameraPivotPoint;
        }

        public float PlayerSpeedMagnitude
        {
            get => playerSpeedProcessingInput.PlayerSpeedMagnitude;
        }

        public void Tick(float d)
        {
            this.playerSpeedProcessingInput = ComputePlayerSpeedProcessingInput();
        }
        
        
        private PlayerSpeedProcessingInput ComputePlayerSpeedProcessingInput()
        {
            var currentCameraAngle = CameraPivotPoint.transform.eulerAngles.y;

            var inputDisplacementVector = GameInputManager.CurrentInput.LocomotionAxis();
            var projectedDisplacement = Quaternion.Euler(0, currentCameraAngle, 0) * inputDisplacementVector;

            var playerMovementOrientation = projectedDisplacement.normalized;

            return new PlayerSpeedProcessingInput(playerMovementOrientation, inputDisplacementVector.sqrMagnitude);
        }

        public void ResetSpeed()
        {
            this.playerSpeedProcessingInput = new PlayerSpeedProcessingInput(Vector3.zero, 0f);
        }

        public void FixedTick(float d)
        {
            //move rigid body rotation
            if (playerSpeedProcessingInput.PlayerMovementOrientation.sqrMagnitude > .05)
            {
                PlayerRigidBody.rotation = Quaternion.LookRotation(playerSpeedProcessingInput.PlayerMovementOrientation);
                //rotation will take place at the end of physics step https://docs.unity3d.com/ScriptReference/Rigidbody-rotation.html
            }

            //move rigid body
            PlayerRigidBody.velocity = playerSpeedProcessingInput.PlayerMovementOrientation * playerSpeedProcessingInput.PlayerSpeedMagnitude * this.SpeedMultiplicationFactor;
        }
    }

    public struct PlayerSpeedProcessingInput
    {
        public Vector3 PlayerMovementOrientation;
        public float PlayerSpeedMagnitude;

        public PlayerSpeedProcessingInput(Vector3 playerMovementOrientation, float playerSpeedMagnitude)
        {
            this.PlayerMovementOrientation = playerMovementOrientation;
            this.PlayerSpeedMagnitude = playerSpeedMagnitude;
        }

    }

    #endregion
}