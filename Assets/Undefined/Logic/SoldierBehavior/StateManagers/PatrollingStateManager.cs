using AIObjects;
using InteractiveObjects;
using UnityEngine;

namespace SoliderAIBehavior
{
    /// <summary>
    /// The default State of <see cref="SoldierAIBehavior"/> (<see cref="SoldierAIStateEnum.PATROLLING"/>).
    /// It holds and execute a <see cref="AIPatrolSystem"/>.
    /// </summary>
    class PatrollingStateManager : SoldierStateManager
    {
        private AIPatrolSystem AIPatrolSystem;

        public PatrollingStateManager(CoreInteractiveObject AssociatedInteractiveObject, AIPatrolSystemDefinition AIPatrolSystemDefinition)
        {
            this.AIPatrolSystem = new AIPatrolSystem(AssociatedInteractiveObject, AIPatrolSystemDefinition);
        }

        public override void Tick(float d)
        {
            this.AIPatrolSystem.Tick(d);
        }

        public override void OnDestinationReached()
        {
            this.AIPatrolSystem.OnAIDestinationReached();
        }
    }
}