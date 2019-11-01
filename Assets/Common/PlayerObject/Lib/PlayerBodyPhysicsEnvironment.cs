using CoreGame;
using UnityEngine;

namespace PlayerObject
{
    public class PlayerBodyPhysicsEnvironment
    {
        private GroundRayCaster GroundRayCaster;
        private StickGroundBodyPositioner StickGroundBodyPositioner;
        private SlopeVelocityAdjuster SlopeVelocityAdjuster;

        public PlayerBodyPhysicsEnvironment(Rigidbody rigidbody, Collider collider, float MinimumDistanceToStick)
        {
            GroundRayCaster = new GroundRayCaster(rigidbody, collider);
            StickGroundBodyPositioner = new StickGroundBodyPositioner(MinimumDistanceToStick, rigidbody);
            SlopeVelocityAdjuster = new SlopeVelocityAdjuster(rigidbody);
        }

        public void FixedTick(float d)
        {
            GroundRayCaster.FixedTick(d);

            if (GroundRayCaster.HasHitted())
            {
                //slope velocity adjusted
                SlopeVelocityAdjuster.FixedTick(GroundRayCaster.GetHitNormal());

                //stick to ground
                StickGroundBodyPositioner.FixedTick(GroundRayCaster.GetHitPosition());
            }
        }
    }

    class GroundRayCaster
    {
        private Rigidbody rigidbody;
        private Collider collider;
        private RaycastHit hit;

        public GroundRayCaster(Rigidbody rigidbody, Collider collider)
        {
            this.rigidbody = rigidbody;
            this.collider = collider;
        }

        public void FixedTick(float d)
        {
            PhysicsHelper.RaycastToDownVertically(collider, rigidbody, 1 << LayerMask.NameToLayer(LayerConstants.PUZZLE_GROUND_LAYER), out hit);
        }

        public bool HasHitted()
        {
            return hit.collider != null;
        }

        public Vector3 GetHitPosition()
        {
            return hit.point;
        }

        public Vector3 GetHitNormal()
        {
            return hit.normal;
        }
    }


    class StickGroundBodyPositioner
    {
        private Rigidbody rigidbody;
        private float MinimumDistanceToStick;

        public StickGroundBodyPositioner(float MinimumDistanceToStick, Rigidbody rigidbody)
        {
            this.rigidbody = rigidbody;
            this.MinimumDistanceToStick = MinimumDistanceToStick;
        }

        public void FixedTick(Vector3 hitPosition)
        {
            if (Vector3.Distance(rigidbody.position, hitPosition) > this.MinimumDistanceToStick)
            {
                rigidbody.position = hitPosition;
            }
        }
    }

    class SlopeVelocityAdjuster
    {
        private Rigidbody rigidbody;

        public SlopeVelocityAdjuster(Rigidbody rigidbody)
        {
            this.rigidbody = rigidbody;
        }

        public void FixedTick(Vector3 hitNormal)
        {
            // (1) - We project the normal in the current direction of rigid body -> this allow to take player orientation while getting on slope.
            var rigidBodyProjectedNormal = Vector3.ProjectOnPlane(hitNormal, rigidbody.transform.right);
            var slopeQuaternion = Quaternion.FromToRotation(rigidbody.transform.up, rigidBodyProjectedNormal);
            // velocity vector is rotated
            rigidbody.velocity = slopeQuaternion * rigidbody.velocity;
            Debug.DrawRay(rigidbody.position, rigidBodyProjectedNormal * 10, Color.green);
            Debug.DrawRay(rigidbody.position, rigidbody.velocity * 10, Color.red);
        }

        internal void ResetSpeed()
        {
            this.rigidbody.velocity = Vector3.zero;
        }
    }
}