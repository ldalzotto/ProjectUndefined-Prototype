using Input;
using UnityEngine;

namespace PlayerObject
{
    /// <summary>
    /// Process Input to calculate <see cref="PlayerInteractiveObject"/> <see cref="Rigidbody"/> rotation and velocity.
    /// </summary>
    public class PlayerRigidBodyMoveManager : APlayerMoveManager
    {
        private float SpeedMultiplicationFactor;
        private Rigidbody PlayerRigidBody;
        private Transform CameraPivotPoint;
        private GameInputManager GameInputManager;
        private PlayerSpeedProcessingInput playerSpeedProcessingInput;

        public PlayerRigidBodyMoveManager(float SpeedMultiplicationFactor, Rigidbody playerRigidBody, Transform cameraPivotPoint)
        {
            this.SpeedMultiplicationFactor = SpeedMultiplicationFactor;
            PlayerRigidBody = playerRigidBody;
            this.GameInputManager = GameInputManager.Get();
            this.CameraPivotPoint = cameraPivotPoint;
        }

        public override float GetPlayerSpeedMagnitude()
        {
            return this.playerSpeedProcessingInput.PlayerSpeedMagnitude;
        }
        
        public override void Tick(float d)
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

        public override void ResetSpeed()
        {
            this.playerSpeedProcessingInput = new PlayerSpeedProcessingInput(Vector3.zero, 0f);
        }

        public override void FixedTick(float d)
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
}