using CoreGame;
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
                //if target is too close, we look to destination
                var distanceToDestination = Vector3.Distance(objectAgent.nextPosition, objectAgent.destination);

                if (objectAgent.nextPosition != objectAgent.destination && distanceToDestination <= 5f)
                    this.CurrentLookingTargetRotation = Quaternion.LookRotation(objectAgent.destination - objectAgent.nextPosition, Vector3.up);
                else
                    this.CurrentLookingTargetRotation = Quaternion.LookRotation((objectAgent.path.corners[1] - objectAgent.path.corners[0]).normalized, Vector3.up);

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
                this.CurrentLookingTargetRotation = Quaternion.LookRotation((this.TargetLook.position - this.objectAgent.transform.position).normalized, Vector3.up);
                objectAgent.transform.rotation = Quaternion.Slerp(objectAgent.transform.rotation, this.CurrentLookingTargetRotation, this.AITransformMoveManagerComponentV3.RotationSpeed * d);
            }
        }
    }
}