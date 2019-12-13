using UnityEngine;

namespace PlayerObject_Interfaces
{
    /// <summary>
    /// The <see cref="PlayerMovementConstraint"/> purpose is to centralize centralize the PlayerObject position and rotation modifications.
    /// To be sure that nothig else changes it's position, we introduce constraints that are applied during transform calcualtion of PlayerMoveManager.
    /// This avoids position and rotation were updated at different point of the application and inconsistently across physics and tick udpates.
    /// /!\ In order for constraint to be taken into account frame perfectly, constraint must be applied before <see cref="PlayerMoveManager"/> is updated.
    /// </summary>
    public interface PlayerMovementConstraint
    {
        void ApplyConstraint(ref Rigidbody playerInteractiveRigidbody);
    }

    public struct NoConstraint : PlayerMovementConstraint
    {
        public void ApplyConstraint(ref Rigidbody playerInteractiveRigidbody)
        {
        }
    }

    public struct LookDirectionConstraint : PlayerMovementConstraint
    {
        private Quaternion LookQuaternion;

        public LookDirectionConstraint(Quaternion lookQuaternion)
        {
            LookQuaternion = lookQuaternion;
        }

        public void ApplyConstraint(ref Rigidbody playerInteractiveRigidbody)
        {
            playerInteractiveRigidbody.transform.rotation = this.LookQuaternion;
        }
    }
}