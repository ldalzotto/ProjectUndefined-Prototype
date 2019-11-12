using AIObjects;
using InteractiveObjects;
using UnityEngine;

namespace TrainingLevel
{
    /// <summary>
    /// The default State of <see cref="SoldierAIBehavior"/> (<see cref="SoldierAIStateEnum.PATROLLING"/>).
    /// It holds and execute a <see cref="AIPatrolSystem"/>.
    /// </summary>
    class PatrollingStateManager : SoldierStateManager
    {
        private SoldierAIBehavior SoldierAIBehavior;
        private AIPatrolSystem AIPatrolSystem;

        public PatrollingStateManager(SoldierAIBehavior SoldierAIBehavior, CoreInteractiveObject AssociatedInteractiveObject, AIPatrolSystemDefinition AIPatrolSystemDefinition)
        {
            this.SoldierAIBehavior = SoldierAIBehavior;
            this.AIPatrolSystem = new AIPatrolSystem(AssociatedInteractiveObject, AIPatrolSystemDefinition);
        }

        public override void Tick(float d)
        {
            this.AIPatrolSystem.Tick(d);
        }
        
        public override void OnPlayerObjectJustOnSight(CoreInteractiveObject InSightInteractiveObject)
        {
            Debug.Log(MyLog.Format("PatrollingStateManager to MOVE_TOWARDS_PLAYER"));
            this.SoldierAIBehavior.SetState(SoldierAIStateEnum.MOVE_TOWARDS_PLAYER);
        }

        public override void OnDestinationReached()
        {
            this.AIPatrolSystem.OnAIDestinationReached();
        }
    }
}