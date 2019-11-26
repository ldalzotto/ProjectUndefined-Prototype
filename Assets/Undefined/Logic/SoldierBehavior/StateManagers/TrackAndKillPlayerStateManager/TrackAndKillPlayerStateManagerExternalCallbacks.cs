using System;
using InteractiveObjects_Interfaces;
using UnityEngine;
using UnityEngine.AI;
using Weapon;

namespace SoliderAIBehavior
{
 public struct TrackAndKillPlayerStateManagerExternalCallbacks
    {
        public Func<IAgentMovementCalculationStrategy, AIMovementSpeedAttenuationFactor, NavMeshPathStatus> SetAIAgentDestinationAction;
        public Action ClearAIAgentPathAction;
        public Action<Vector3> AskToFireAFiredProjectile_WithTargetPosition_Action;
        public Func<WeaponHandlingFirePointOriginLocalDefinition> GetWeaponFirePointOriginLocalDefinitionAction;

        /// <summary>
        /// As <see cref="TrackAndKillPlayerStateManager"/> is itself a <see cref="TrackAndKillPlayerStateManager.TrackAndKillPlayerBehavior"/>,
        /// this callback must be called when parent state manager must exit <see cref="TrackAndKillPlayerStateManager"/> state.
        /// </summary>
        public Action AskedToExitTrackAndKillPlayerBehaviorAction;

        public TrackAndKillPlayerStateManagerExternalCallbacks(Func<IAgentMovementCalculationStrategy, AIMovementSpeedAttenuationFactor, NavMeshPathStatus> aiAgentDestinationAction,
            Action clearAiAgentPathAction, Action<Vector3> askToFireAFiredProjectileWithTargetPositionAction, Func<WeaponHandlingFirePointOriginLocalDefinition> weaponFirePointOriginLocalDefinitionAction,
            Action askedToExitTrackAndKillPlayerBehaviorAction)
        {
            SetAIAgentDestinationAction = aiAgentDestinationAction;
            ClearAIAgentPathAction = clearAiAgentPathAction;
            AskToFireAFiredProjectile_WithTargetPosition_Action = askToFireAFiredProjectileWithTargetPositionAction;
            GetWeaponFirePointOriginLocalDefinitionAction = weaponFirePointOriginLocalDefinitionAction;
            AskedToExitTrackAndKillPlayerBehaviorAction = askedToExitTrackAndKillPlayerBehaviorAction;
        }

        public static implicit operator WeaponFiringAreaSystemExternalCallbacks(TrackAndKillPlayerStateManagerExternalCallbacks TrackAndKillPlayerStateManagerExternalCallbacks)
        {
            return new WeaponFiringAreaSystemExternalCallbacks(
                TrackAndKillPlayerStateManagerExternalCallbacks.GetWeaponFirePointOriginLocalDefinitionAction
            );
        }

        public static implicit operator MoveTowardsPlayerStateManagerExternalCallbacks(TrackAndKillPlayerStateManagerExternalCallbacks TrackAndKillPlayerStateManagerExternalCallbacks)
        {
            return new MoveTowardsPlayerStateManagerExternalCallbacks(
                TrackAndKillPlayerStateManagerExternalCallbacks.SetAIAgentDestinationAction
            );
        }

        public static implicit operator ShootingAtPlayerStateManagerExternalCallbacks(TrackAndKillPlayerStateManagerExternalCallbacks TrackAndKillPlayerStateManagerExternalCallbacks)
        {
            return new ShootingAtPlayerStateManagerExternalCallbacks(
                TrackAndKillPlayerStateManagerExternalCallbacks.ClearAIAgentPathAction,
                TrackAndKillPlayerStateManagerExternalCallbacks.AskToFireAFiredProjectile_WithTargetPosition_Action
            );
        }

        public static implicit operator MoveAroundPlayerStateManagerExternalCallbacks(TrackAndKillPlayerStateManagerExternalCallbacks TrackAndKillPlayerStateManagerExternalCallbacks)
        {
            return new MoveAroundPlayerStateManagerExternalCallbacks(
                TrackAndKillPlayerStateManagerExternalCallbacks.SetAIAgentDestinationAction
            );
        }

        public static implicit operator MoveToLastSeenPlayerPositionStateManagerExternalCallbacks(TrackAndKillPlayerStateManagerExternalCallbacks TrackAndKillPlayerStateManagerExternalCallbacks)
        {
            return new MoveToLastSeenPlayerPositionStateManagerExternalCallbacks(
                TrackAndKillPlayerStateManagerExternalCallbacks.SetAIAgentDestinationAction,
                TrackAndKillPlayerStateManagerExternalCallbacks.ClearAIAgentPathAction);
        }
    }
}