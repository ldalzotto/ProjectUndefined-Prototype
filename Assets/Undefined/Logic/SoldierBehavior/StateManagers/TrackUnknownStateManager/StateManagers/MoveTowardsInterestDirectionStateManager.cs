using System;
using AIObjects;
using CoreGame;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace SoliderAIBehavior
{
    public struct MoveTowardsInterestDirectionStateManagerExternalCallbacks
    {
        public Func<IAgentMovementCalculationStrategy, AIMovementSpeedAttenuationFactor, NavMeshPathStatus> SetAIAgentDestinationAction;
        public Action OnTrackUnknownStateManagerAskedToExit;

        public MoveTowardsInterestDirectionStateManagerExternalCallbacks(Func<IAgentMovementCalculationStrategy, AIMovementSpeedAttenuationFactor, NavMeshPathStatus> aiAgentDestinationAction, Action onTrackUnknownStateManagerAskedToExit)
        {
            SetAIAgentDestinationAction = aiAgentDestinationAction;
            OnTrackUnknownStateManagerAskedToExit = onTrackUnknownStateManagerAskedToExit;
        }
    }

    /// <summary>
    /// The <see cref="SoldierAIBehavior"/> moves towards <see cref="TrackUnknownInterestDirectionSystem"/> interest direction to potentially
    /// see a point of intereset (sight events will be triggered by <see cref="SoldierAIBehavior.PlayerObjectStateDataSystem"/>).
    /// </summary>
    public class MoveTowardsInterestDirectionStateManager : SoldierStateManager
    {
        private CoreInteractiveObject AssociatedInteractiveObject;
        private TrackUnknownInterestDirectionSystem TrackUnknownInterestDirectionSystem;
        private SoldierAIBehaviorDefinition SoldierAIBehaviorDefinition;

        private MoveTowardsInterestDirectionStateManagerExternalCallbacks MoveTowardsInterestDirectionStateManagerExternalCallbacks;
        public MoveTowardsInterestDirectionStateManager(
            CoreInteractiveObject AssociatedInteractiveObject,
            TrackUnknownInterestDirectionSystem TrackUnknownInterestDirectionSystem,
            SoldierAIBehaviorDefinition SoldierAIBehaviorDefinition,
            MoveTowardsInterestDirectionStateManagerExternalCallbacks MoveTowardsInterestDirectionStateManagerExternalCallbacks)
        {
            this.AssociatedInteractiveObject = AssociatedInteractiveObject;
            this.TrackUnknownInterestDirectionSystem = TrackUnknownInterestDirectionSystem;
            this.SoldierAIBehaviorDefinition = SoldierAIBehaviorDefinition;
            this.MoveTowardsInterestDirectionStateManagerExternalCallbacks = MoveTowardsInterestDirectionStateManagerExternalCallbacks;
        }

        public override void DamageDealt(CoreInteractiveObject damageDealerInteractiveObject)
        {
            base.DamageDealt(damageDealerInteractiveObject);

            var TargetWorldPositionNavMehshit = this.GetTargetWorldPositionNavMehshit();
            this.MoveTowardsInterestDirectionStateManagerExternalCallbacks.SetAIAgentDestinationAction.Invoke(new ForwardAgentMovementCalculationStrategy(new AIDestination(TargetWorldPositionNavMehshit.position, null)), AIMovementSpeedAttenuationFactor.RUN);
        }

        /// <summary>
        /// If this event is called, this means that nothing has happened.
        /// </summary>
        public override void OnDestinationReached()
        {
            this.MoveTowardsInterestDirectionStateManagerExternalCallbacks.OnTrackUnknownStateManagerAskedToExit.Invoke();
        }

        private NavMeshHit GetTargetWorldPositionNavMehshit()
        {
            Vector3 targetposition = this.AssociatedInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition
                                     + (this.TrackUnknownInterestDirectionSystem.WorldDirectionInterest * this.SoldierAIBehaviorDefinition.MoveTowardsInterestDirectionDistance);
            NavMesh.Raycast(this.AssociatedInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition,
                targetposition, out NavMeshHit hit, 1 << NavMesh.GetAreaFromName(NavMeshConstants.WALKABLE_LAYER));
            return hit;
        }
    }
}