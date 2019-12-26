using System;
using InteractiveObjects_Interfaces;
using UnityEngine.AI;

namespace SoliderAIBehavior
{
    public struct TrackUnknownStateManagerExternalCallbacks
    {
        public Func<IAgentMovementCalculationStrategy, NavMeshPathStatus> SetAIAgentDestinationAction;
        public Action<AIMovementSpeedAttenuationFactor> SetAIAgentSpeedAttenuationAction;
        public Action OnTrackUnknownStateManagerAskedToExit;

        public TrackUnknownStateManagerExternalCallbacks(Func<IAgentMovementCalculationStrategy, NavMeshPathStatus> SetAIAgentDestinationAction, 
            Action<AIMovementSpeedAttenuationFactor> SetAIAgentSpeedAttenuationAction, Action onTrackUnknownStateManagerAskedToExit)
        {
            this.SetAIAgentDestinationAction = SetAIAgentDestinationAction;
            this.SetAIAgentSpeedAttenuationAction = SetAIAgentSpeedAttenuationAction;
            OnTrackUnknownStateManagerAskedToExit = onTrackUnknownStateManagerAskedToExit;
        }

        public static implicit operator MoveTowardsInterestDirectionStateManagerExternalCallbacks(TrackUnknownStateManagerExternalCallbacks TrackUnknownStateManagerExternalCallbacks)
        {
            return new MoveTowardsInterestDirectionStateManagerExternalCallbacks(
                TrackUnknownStateManagerExternalCallbacks.SetAIAgentDestinationAction,
                TrackUnknownStateManagerExternalCallbacks.SetAIAgentSpeedAttenuationAction,
                TrackUnknownStateManagerExternalCallbacks.OnTrackUnknownStateManagerAskedToExit
            );
        }
    }
}