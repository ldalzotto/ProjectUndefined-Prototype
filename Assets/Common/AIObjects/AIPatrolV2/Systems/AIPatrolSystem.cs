using CoreGame;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using SequencedAction;

namespace AIObjects
{
    public class AIPatrolSystem : AInteractiveObjectSystem
    {
        private SequencedActionPlayer SequencedActionPlayer;

        public AIPatrolSystem(CoreInteractiveObject AssociatedCoreInteractiveObject, AIPatrolSystemDefinition AIPatrolSystemDefinition)
        {
            SequencedActionPlayer = new SequencedActionPlayer(AIPatrolSystemDefinition.AIPatrolGraph.AIPatrolGraphActions(AssociatedCoreInteractiveObject), null, null);
            SequencedActionPlayer.Play();
        }

        public override void Tick(float d)
        {
            SequencedActionPlayer.Tick(d);
        }

        public void OnAIDestinationReached()
        {
            foreach (var currentAction in SequencedActionPlayer.GetCurrentActions(true))
            {
                var destinationReachedListeningNode = currentAction as IActionAbortedOnDestinationReached;
                if (destinationReachedListeningNode != null) destinationReachedListeningNode.OnDestinationReached();
            }
        }
    }
}