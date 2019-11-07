using InteractiveObjects;
using InteractiveObjects_Interfaces;

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
        public CoreInteractiveObject TargetLook;

        public LookingAtAgentMovementCalculationStrategy(AIDestination aiDestination, CoreInteractiveObject targetLook)
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