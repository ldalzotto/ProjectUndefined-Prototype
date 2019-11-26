using System;
using InteractiveObjects_Interfaces;
using UnityEngine.AI;

namespace SoliderAIBehavior
{
    public struct TrackUnknownStateManagerExternalCallbacks
    {
        public Func<IAgentMovementCalculationStrategy, AIMovementSpeedAttenuationFactor, NavMeshPathStatus> SetAIAgentDestinationAction;
        public Action OnTrackUnknownStateManagerAskedToExit;

        public TrackUnknownStateManagerExternalCallbacks(Func<IAgentMovementCalculationStrategy, AIMovementSpeedAttenuationFactor, NavMeshPathStatus> aiAgentDestinationAction, Action onTrackUnknownStateManagerAskedToExit)
        {
            SetAIAgentDestinationAction = aiAgentDestinationAction;
            OnTrackUnknownStateManagerAskedToExit = onTrackUnknownStateManagerAskedToExit;
        }

        public static implicit operator MoveTowardsInterestDirectionStateManagerExternalCallbacks(TrackUnknownStateManagerExternalCallbacks TrackUnknownStateManagerExternalCallbacks)
        {
            return new MoveTowardsInterestDirectionStateManagerExternalCallbacks(
                TrackUnknownStateManagerExternalCallbacks.SetAIAgentDestinationAction,
                TrackUnknownStateManagerExternalCallbacks.OnTrackUnknownStateManagerAskedToExit
            );
        }
    }
}