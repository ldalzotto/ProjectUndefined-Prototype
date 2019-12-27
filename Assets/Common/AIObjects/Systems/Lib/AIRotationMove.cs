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
                }

                objectAgent.transform.rotation = Quaternion.Slerp(objectAgent.transform.rotation, this.CurrentLookingTargetRotation, this.AITransformMoveManagerComponentV3.RotationSpeed * d);
            }
            else
            {
                //If the agent has no path, this could be a warp
                if (this.AIDestinationManagerRef.CurrentDestination.HasValue && this.AIDestinationManagerRef.CurrentDestination.Value.Rotation.HasValue)
                {
                    var targetRotation = this.AIDestinationManagerRef.CurrentDestination.Value.Rotation.Value;
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
            }
        }
    }
}