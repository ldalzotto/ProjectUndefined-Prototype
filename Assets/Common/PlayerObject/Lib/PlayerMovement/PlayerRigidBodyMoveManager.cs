using Input;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using UnityEngine;
using UnityEngine.Jobs;

namespace PlayerObject
{
    /// <summary>
    /// Process Input to calculate <see cref="PlayerInteractiveObject"/> <see cref="Rigidbody"/> rotation and velocity.
    /// </summary>
    public class PlayerRigidBodyMoveManager : APlayerMoveManager
    {
        private Rigidbody PlayerRigidBody;
        private Transform CameraPivotPoint;
        private GameInputManager GameInputManager;

        public PlayerRigidBodyMoveManager(CoreInteractiveObject PlayerInteractiveObject, TransformMoveManagerComponentV3 TransformMoveManagerComponentV3,
            Transform cameraPivotPoint)
            : base(PlayerInteractiveObject, TransformMoveManagerComponentV3)
        {
            PlayerRigidBody = PlayerInteractiveObject.InteractiveGameObject.PhysicsRigidbody;
            this.GameInputManager = GameInputManager.Get();
            this.CameraPivotPoint = cameraPivotPoint;
        }

        public override void Tick(float d)
        {
            base.Tick(d);
            this.ComputePlayerSpeed();
        }

        private void ComputePlayerSpeed()
        {
            var currentCameraAngle = CameraPivotPoint.transform.eulerAngles.y;

            var inputDisplacementVector = GameInputManager.CurrentInput.LocomotionAxis();
            var projectedDisplacement = Quaternion.Euler(0, currentCameraAngle, 0) * inputDisplacementVector;

            var playerMovementOrientation = projectedDisplacement.normalized;

            this.ObjectMovementSpeedSystem.ManualCalculation(playerMovementOrientation, inputDisplacementVector.sqrMagnitude);
        }


        public override void FixedTick(float d)
        {
            //move rigid body rotation
            if (Mathf.Abs(this.ObjectMovementSpeedSystem.GetSpeedMagnitude()) >= .05)
            {
                PlayerRigidBody.rotation = Quaternion.LookRotation(this.ObjectMovementSpeedSystem.GetWorldDirection());
                //rotation will take place at the end of physics step https://docs.unity3d.com/ScriptReference/Rigidbody-rotation.html
            }

            //move rigid body by taking account speed attenuation factor
            PlayerRigidBody.velocity = this.ObjectMovementSpeedSystem.GetVelocity();
        }
    }
}