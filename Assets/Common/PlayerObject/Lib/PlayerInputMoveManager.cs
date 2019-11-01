using Input;
using UnityEngine;

namespace PlayerObject
{
    #region Player Movement

    public abstract class PlayerMoveManager
    {
        private float SpeedMultiplicationFactor;
        protected Rigidbody PlayerRigidBody;
        protected float playerSpeedMagnitude;
        protected PlayerSpeedProcessingInput playerSpeedProcessingInput;

        protected abstract PlayerSpeedProcessingInput ComputePlayerSpeedProcessingInput();

        public PlayerMoveManager(float SpeedMultiplicationFactor, Rigidbody playerRigidBody)
        {
            this.SpeedMultiplicationFactor = SpeedMultiplicationFactor;
            PlayerRigidBody = playerRigidBody;
        }

        public float PlayerSpeedMagnitude
        {
            get => playerSpeedProcessingInput.PlayerSpeedMagnitude;
        }

        public void Tick(float d)
        {
            this.playerSpeedProcessingInput = ComputePlayerSpeedProcessingInput();
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

    public class PlayerInputMoveManager : PlayerMoveManager
    {
        private Transform CameraPivotPoint;
        private GameInputManager GameInputManager;

        public PlayerInputMoveManager(float SpeedMultiplicationFactor, Transform cameraPivotPoint, GameInputManager gameInputManager, Rigidbody playerRigidBody)
            : base(SpeedMultiplicationFactor, playerRigidBody)
        {
            CameraPivotPoint = cameraPivotPoint;
            GameInputManager = gameInputManager;
            this.playerSpeedProcessingInput = ComputePlayerSpeedProcessingInput();
        }

        protected override PlayerSpeedProcessingInput ComputePlayerSpeedProcessingInput()
        {
            var currentCameraAngle = CameraPivotPoint.transform.eulerAngles.y;

            var inputDisplacementVector = GameInputManager.CurrentInput.LocomotionAxis();
            var projectedDisplacement = Quaternion.Euler(0, currentCameraAngle, 0) * inputDisplacementVector;

            var playerMovementOrientation = projectedDisplacement.normalized;

            return new PlayerSpeedProcessingInput(playerMovementOrientation, inputDisplacementVector.sqrMagnitude);
        }

        public void ResetSpeed()
        {
            this.playerSpeedMagnitude = 0;
            base.playerSpeedProcessingInput.PlayerSpeedMagnitude = 0;
            base.playerSpeedProcessingInput.PlayerMovementOrientation = Vector3.zero;
        }
    }

    public class PlayerSpeedProcessingInput
    {
        private Vector3 playerMovementOrientation;
        private float playerSpeedMagnitude;

        public PlayerSpeedProcessingInput(Vector3 playerMovementOrientation, float playerSpeedMagnitude)
        {
            this.playerMovementOrientation = playerMovementOrientation;
            this.playerSpeedMagnitude = playerSpeedMagnitude;
        }

        public Vector3 PlayerMovementOrientation
        {
            get => playerMovementOrientation;
            set => playerMovementOrientation = value;
        }

        public float PlayerSpeedMagnitude
        {
            get => playerSpeedMagnitude;
            set => playerSpeedMagnitude = value;
        }
    }

    #endregion
}