using System;
using InteractiveObjects;
using OdinSerializer;
using SequencedAction;
using UnityEngine;

namespace AIObjects
{
    public abstract class AIPatrolGraphBuilder : SerializedScriptableObject
    {
        public abstract ASequencedAction[] GetPatrolGraphSequencedActions(CoreInteractiveObject InvolvedInteractiveObject, AIPatrolGraphRuntimeCallbacks AIPatrolGraphRuntimeCallbacks);
    }
    
    [Serializable]
    public class AIPatrolSystemDefinition : AIPatrolGraphBuilder
    {
        [Inline(CreateAtSameLevelIfAbsent = true)]
        public AIPatrolGraphV2 AIPatrolGraph;

        public override ASequencedAction[] GetPatrolGraphSequencedActions(CoreInteractiveObject InvolvedInteractiveObject, AIPatrolGraphRuntimeCallbacks AIPatrolGraphRuntimeCallbacks)
        {
            return this.AIPatrolGraph.AIPatrolGraphActions(InvolvedInteractiveObject, AIPatrolGraphRuntimeCallbacks);
        }
    }
}