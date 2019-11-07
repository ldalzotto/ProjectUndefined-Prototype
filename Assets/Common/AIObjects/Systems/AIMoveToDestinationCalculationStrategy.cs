using InteractiveObjects_Interfaces;
using UnityEngine;

namespace AIObjects
{
    public struct ForwardAgentMovementCalculationStrategy : IAgentMovementCalculationStrategy
    {
        public AIDestination AiDestination;

        public ForwardAgentMovementCalculationStrategy(AIDestination aiDestination)
        {
            AiDestination = aiDestination;
        }

        public AIDestination GetAIDestination()
        {
            return this.AiDestination;
        }
    }

    public struct LookingAtAgentMovementCalculationStrategy : IAgentMovementCalculationStrategy
    {
        public AIDestination AiDestination;
        public Transform TargetLook;

        public LookingAtAgentMovementCalculationStrategy(AIDestination aiDestination, Transform targetLook)
        {
            AiDestination = aiDestination;
            TargetLook = targetLook;
        }

        public AIDestination GetAIDestination()
        {
            return this.AiDestination;
        }
    }
}