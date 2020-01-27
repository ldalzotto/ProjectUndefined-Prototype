using System;
using AIObjects;
using CoreGame;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace SoliderAIBehavior
{
    /// <summary>
    /// The <see cref="SoldierAIBehavior"/> moves towards <see cref="TrackUnknownInterestDirectionSystem"/> interest direction to potentially
    /// see a point of intereset (sight events will be triggered by <see cref="SoldierAIBehavior.PlayerObjectStateDataSystem"/>).
    /// </summary>
    public class MoveTowardsInterestDirectionStateManager : SoldierStateManager
    {
        private CoreInteractiveObject AssociatedInteractiveObject;
        private TrackUnknownInterestDirectionSystem TrackUnknownInterestDirectionSystem;
        private SoldierAIBehaviorDefinition SoldierAIBehaviorDefinition;

        private ISetAIAgentDestinationActionCallback ISetAIAgentDestinationActionCallback;
        private Action OnTrackUnknownStateManagerAskedToExit;
        
        public MoveTowardsInterestDirectionStateManager(
            CoreInteractiveObject AssociatedInteractiveObject,
            TrackUnknownInterestDirectionSystem TrackUnknownInterestDirectionSystem,
            SoldierAIBehaviorDefinition SoldierAIBehaviorDefinition,
            ISetAIAgentDestinationActionCallback ISetAIAgentDestinationActionCallback, Action OnTrackUnknownStateManagerAskedToExit)
        {
            this.AssociatedInteractiveObject = AssociatedInteractiveObject;
            this.TrackUnknownInterestDirectionSystem = TrackUnknownInterestDirectionSystem;
            this.SoldierAIBehaviorDefinition = SoldierAIBehaviorDefinition;
            this.ISetAIAgentDestinationActionCallback = ISetAIAgentDestinationActionCallback;
            this.OnTrackUnknownStateManagerAskedToExit = OnTrackUnknownStateManagerAskedToExit;
        }

        public override void DamageDealt(CoreInteractiveObject damageDealerInteractiveObject)
        {
            base.DamageDealt(damageDealerInteractiveObject);

            var TargetWorldPositionNavMehshit = this.GetTargetWorldPositionNavMehshit();
            this.ISetAIAgentDestinationActionCallback.SetAIAgentSpeedAttenuationAction.Invoke(AIMovementSpeedAttenuationFactor.RUN);
            this.ISetAIAgentDestinationActionCallback.SetAIAgentDestinationAction.Invoke(new ForwardAgentMovementCalculationStrategy(new AIDestination(TargetWorldPositionNavMehshit.position, null)));
        }

        /// <summary>
        /// If this event is called, this means that nothing has happened.
        /// </summary>
        public override void OnDestinationReached()
        {
            this.OnTrackUnknownStateManagerAskedToExit.Invoke();
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