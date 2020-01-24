using System;
using System.Collections.Generic;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using OdinSerializer;
using SequencedAction;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AIObjects
{
    [Serializable]
    public class RuntimePatrolGraphNodes : AIPatrolGraphBuilder
    {
        public List<PatrolGraphNode> PatrolGraphNodes;

        public override ASequencedAction[] GetPatrolGraphSequencedActions(CoreInteractiveObject InvolvedInteractiveObject, AIPatrolGraphRuntimeCallbacks AIPatrolGraphRuntimeCallbacks)
        {
            var startNode = this.PatrolGraphNodes[Random.Range(0, this.PatrolGraphNodes.Count - 1)];
            var startWarpNode = AIPatrolGraphV2.CreateAIWarpAction(InvolvedInteractiveObject, startNode.WorldPosition, startNode.WorldRotation);
            var actionInifineLoop = new BranchInfiniteLoopAction();

            ASequencedAction intitialMovementNode = null;
            ASequencedAction currentMovementNode = null;
            PatrolGraphNode currentPatrolGraphNode = startNode;

            var maxGraphWaypoints = Random.Range(4, 12);
            for (int i = 0; i < maxGraphWaypoints; i++)
            {
                var pickedLink = currentPatrolGraphNode.Links[Random.Range(0, currentPatrolGraphNode.Links.Count - 1)];
                PatrolGraphNode newPatrolGraphNode = pickedLink.PatrolGraphNode;
                if (intitialMovementNode == null)
                {
                    intitialMovementNode = BuildActionFromPatrolGraphNode(newPatrolGraphNode, pickedLink, AIPatrolGraphRuntimeCallbacks, out ASequencedAction lastActionInChain);
                    currentMovementNode = lastActionInChain;
                }
                else
                {
                    var newMovementNode = BuildActionFromPatrolGraphNode(newPatrolGraphNode, pickedLink, AIPatrolGraphRuntimeCallbacks, out ASequencedAction lastActionInChain);
                    currentMovementNode.Then(new ASequencedAction[] {newMovementNode});
                    currentMovementNode = lastActionInChain;
                }

                currentPatrolGraphNode = newPatrolGraphNode;
            }

            actionInifineLoop.SetNextContextAction(new ASequencedAction[] {intitialMovementNode});
            startWarpNode.Then(actionInifineLoop);

            return new ASequencedAction[]
            {
                startWarpNode
            };
        }

        private static ASequencedAction BuildActionFromPatrolGraphNode(PatrolGraphNode PatrolGraphNode, PatrolGraphNodeLink PatrolGraphNodeLink, AIPatrolGraphRuntimeCallbacks AIPatrolGraphRuntimeCallbacks,
            out ASequencedAction LastActionInChain)
        {
            Vector3? worldRotation = null;
            if (PatrolGraphNode.WorldRotationEnabled)
            {
                worldRotation = PatrolGraphNode.WorldRotation;
            }

            var movementAction = AIPatrolGraphV2.CreateAIMoveToActionV2(PatrolGraphNode.WorldPosition, worldRotation, PatrolGraphNodeLink.AIMovementSpeed, AIPatrolGraphRuntimeCallbacks);
            if (PatrolGraphNode.IsWaiting)
            {
                var waitAction = new WaitForSecondsAction(PatrolGraphNode.WaitTime);
                movementAction.SetNextContextAction(new ASequencedAction[] {waitAction});
                LastActionInChain = waitAction;
            }
            else
            {
                LastActionInChain = movementAction;
            }

            return movementAction;
        }
    }
}