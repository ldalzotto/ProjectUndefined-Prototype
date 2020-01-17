using Input;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using PlayerObject_Interfaces;
using UnityEngine;
using UnityEngine.Jobs;

namespace PlayerObject
{
    /// <summary>
    /// Process Input to calculate <see cref="PlayerInteractiveObject"/> <see cref="Rigidbody"/> rotation and velocity.
    /// /!\ As the player transform is physics based, all updates are done inside the <see cref="FixedTick"/>.
    /// </summary>
    public class PlayerRigidBodyMoveManager : APlayerMoveManager
    {
        private Rigidbody PlayerRigidBody;
        private Transform CameraPivotPoint;
        private GameInputManager GameInputManager;

        private ObjectMovementSpeedSystem ObjectMovementSpeedSystemRef;

        public PlayerRigidBodyMoveManager(CoreInteractiveObject PlayerInteractiveObject, TransformMoveManagerComponentV3 TransformMoveManagerComponentV3,
            ObjectMovementSpeedSystem ObjectMovementSpeedSystemRef,
            Transform cameraPivotPoint)
        {
            PlayerRigidBody = PlayerInteractiveObject.InteractiveGameObject.PhysicsRigidbody;
            this.GameInputManager = GameInputManager.Get();
            this.ObjectMovementSpeedSystemRef = ObjectMovementSpeedSystemRef;
            this.CameraPivotPoint = cameraPivotPoint;
            this.CurrentConstraint = new NoConstraint();
        }

        private bool physicsUpdated = false;

        public override void FixedTick(float d)
        {
            this.DoCalculations();
        }

        public override void LateTick(float d)
        {
            this.physicsUpdated = false;
        }

        /// /!\ As the player transform is physics based, all updates are done inside the <see cref="FixedTick"/>.
        private void DoCalculations()
        {
            if (!this.physicsUpdated)
            {
                this.physicsUpdated = true;

                var inputDisplacementVector = UpdateDisplacementFromInput(out Vector3 playerMovementOrientation);

                ApplyPlayerMovementConstraints();

                this.ObjectMovementSpeedSystemRef.ManualCalculation(playerMovementOrientation, inputDisplacementVector.sqrMagnitude, this.PlayerRigidBody.velocity.magnitude);

                if (Time.inFixedTimeStep)
                {
                    //move rigid body
                    PlayerRigidBody.velocity = this.ObjectMovementSpeedSystemRef.GetDesiredVelocity_Scaled_Attenuated();
                    /// Debug.Log(PlayerRigidBody.velocity.ToString("F4"));
                }
            }
        }

        private void ApplyPlayerMovementConstraints()
        {
            this.CurrentConstraint.ApplyConstraint(this.PlayerRigidBody.transform);

            /// Constraints are consumed every frame.
            this.CurrentConstraint = new NoConstraint();
        }

        private Vector3 UpdateDisplacementFromInput(out Vector3 playerMovementOrientation)
        {
            var currentCameraAngle = CameraPivotPoint.transform.eulerAngles.y;

            var inputDisplacementVector = GameInputManager.CurrentInput.LocomotionAxis();
            var projectedDisplacement = Quaternion.Euler(0, currentCameraAngle, 0) * inputDisplacementVector;

            playerMovementOrientation = projectedDisplacement.normalized;


            //move rigid body rotation
            if (Mathf.Abs(projectedDisplacement.magnitude) >= .05)
            {
                PlayerRigidBody.transform.rotation = Quaternion.LookRotation(playerMovementOrientation);
                //rotation will take place at the end of physics step https://docs.unity3d.com/ScriptReference/Rigidbody-rotation.html
            }

            return inputDisplacementVector;
        }
    }
}