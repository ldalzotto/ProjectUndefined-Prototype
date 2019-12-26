using System;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using SequencedAction;

namespace AIObjects
{
    public class AIPatrolSystem : AInteractiveObjectSystem
    {
        private SequencedActionPlayer SequencedActionPlayer;

        public AIPatrolSystem(CoreInteractiveObject AssociatedInteractiveObject,
            AIPatrolSystemDefinition AIPatrolSystemDefinition,
            Action<IAgentMovementCalculationStrategy> coreInteractiveObjectDestinationCallback = null, Action<AIMovementSpeedAttenuationFactor> aiSpeedAttenuationFactorCallback = null)
        {
            SequencedActionPlayer = new SequencedActionPlayer(AIPatrolSystemDefinition.AIPatrolGraph.AIPatrolGraphActions(
                AssociatedInteractiveObject, new AIPatrolGraphRuntimeCallbacks(coreInteractiveObjectDestinationCallback, aiSpeedAttenuationFactorCallback)),
                null, null);
            SequencedActionPlayer.Play();
        }

        public override void Tick(float d)
        {
            SequencedActionPlayer.Tick(d);
        }

        public void OnAIDestinationReached()
        {
            IActionAbortedOnDestinationReachedHelper.ProcessOnDestinationReachedEvent(this.SequencedActionPlayer);
        }
    }
}