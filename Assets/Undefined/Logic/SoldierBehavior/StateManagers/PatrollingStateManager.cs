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
        private SoldierAIBehavior SoldierAIBehavior;
        private PlayerObjectStateDataSystem PlayerObjectStateDataSystem;
        private AIPatrolSystem AIPatrolSystem;

        public PatrollingStateManager(SoldierAIBehavior SoldierAIBehavior, CoreInteractiveObject AssociatedInteractiveObject,
            PlayerObjectStateDataSystem PlayerObjectStateDataSystem, AIPatrolSystemDefinition AIPatrolSystemDefinition)
        {
            this.SoldierAIBehavior = SoldierAIBehavior;
            this.PlayerObjectStateDataSystem = PlayerObjectStateDataSystem;
            this.AIPatrolSystem = new AIPatrolSystem(AssociatedInteractiveObject, AIPatrolSystemDefinition);
        }

        public override void Tick(float d)
        {
            if (this.PlayerObjectStateDataSystem.IsPlayerInSight)
            {
                Debug.Log(MyLog.Format("PatrollingStateManager to MOVE_TO_LAST_SEEN_PLAYER_POSITION"));
                this.SoldierAIBehavior.SetState(SoldierAIStateEnum.MOVE_TO_LAST_SEEN_PLAYER_POSITION);
            }
            else
            {
                this.AIPatrolSystem.Tick(d);
            }
        }

        public override void OnDestinationReached()
        {
            if (!this.PlayerObjectStateDataSystem.IsPlayerInSight)
            {
                this.AIPatrolSystem.OnAIDestinationReached();
            }
        }
    }
}