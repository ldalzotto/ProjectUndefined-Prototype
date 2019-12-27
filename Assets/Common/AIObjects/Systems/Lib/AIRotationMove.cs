using InteractiveObjects_Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace AIObjects
{
    internal abstract class A_AIRotationMoveManager
    {
        public abstract void UpdateAgentRotation(float d);

        public Quaternion CurrentLookingTargetRotation { get; protected set; }
    }

    internal class AIRotationMoveManager : A_AIRotationMoveManager
    {
        private NavMeshAgent objectAgent;
        private AIDestinationManager AIDestinationManagerRef;
        private TransformMoveManagerComponentV3 AITransformMoveManagerComponentV3;

        public AIRotationMoveManager(NavMeshAgent objectAgent, TransformMoveManagerComponentV3 AITransformMoveManagerComponentV3, AIDestinationManager AIDestinationManagerRef)
        {
            this.objectAgent = objectAgent;
            this.AITransformMoveManagerComponentV3 = AITransformMoveManagerComponentV3;
            this.AIDestinationManagerRef = AIDestinationManagerRef;
        }

        public override void UpdateAgentRotation(float d)
        {
            if (objectAgent.hasPath && !objectAgent.isStopped)
            {
                if (objectAgent.desiredVelocity != Vector3.zero)
                {
                    this.CurrentLookingTargetRotation = Quaternion.LookRotation(objectAgent.desiredVelocity.normalized, Vector3.up);
                    Debug.DrawRay(this.objectAgent.transform.position, this.CurrentLookingTargetRotation * Vector3.forward * 10, Color.red);

                    objectAgent.transform.rotation = Quaternion.Slerp(objectAgent.transform.rotation, this.CurrentLookingTargetRotation, this.AITransformMoveManagerComponentV3.RotationSpeed * d);
                }
                else
                {
                    /// If desired velocity is zero, we also set the CurrentLookingTargetRotation to the actual rotation of the Agent.
                    /// This is to ensure that the CurrentLookingTargetRotation keeps synching with the agent value.
                    this.CurrentLookingTargetRotation = objectAgent.transform.rotation;
                }
            }
            else
            {
                //If the agent has no path, this could be a warp
                if (this.AIDestinationManagerRef.CurrentDestination.HasValue && this.AIDestinationManagerRef.CurrentDestination.Value.Rotation.HasValue)
                {
                    var targetRotation = this.AIDestinationManagerRef.CurrentDestination.Value.Rotation.Value;

                    Debug.DrawRay(this.objectAgent.transform.position, targetRotation * Vector3.forward * 10, Color.blue);
                    objectAgent.transform.rotation = Quaternion.Slerp(objectAgent.transform.rotation, targetRotation, this.AITransformMoveManagerComponentV3.RotationSpeed * d);
                }
            }
        }
    }

    internal class AIRotationFacingMoveManager : A_AIRotationMoveManager
    {
        private NavMeshAgent objectAgent;
        private TransformMoveManagerComponentV3 AITransformMoveManagerComponentV3;
        private Transform TargetLook;

        public AIRotationFacingMoveManager(NavMeshAgent objectAgent, TransformMoveManagerComponentV3 aiTransformMoveManagerComponentV3)
        {
            this.objectAgent = objectAgent;
            AITransformMoveManagerComponentV3 = aiTransformMoveManagerComponentV3;
        }

        public void Init(Transform targetLook)
        {
            this.TargetLook = targetLook;
        }

        public override void UpdateAgentRotation(float d)
        {
            if (this.objectAgent.hasPath && !this.objectAgent.isStopped)
            {
                this.CurrentLookingTargetRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane((this.TargetLook.position - this.objectAgent.transform.position), this.objectAgent.transform.up).normalized, Vector3.up);
                objectAgent.transform.rotation = Quaternion.Slerp(objectAgent.transform.rotation, this.CurrentLookingTargetRotation, this.AITransformMoveManagerComponentV3.RotationSpeed * d);

                Debug.DrawRay(this.objectAgent.transform.position, this.CurrentLookingTargetRotation * Vector3.forward * 10, Color.green);
            }
        }
    }
}