using System;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using UnityEngine;
using UnityEngine.AI;
using Weapon;

namespace SoliderAIBehavior
{
 public struct TrackAndKillPlayerStateManagerExternalCallbacks
    {
        public Func<IAgentMovementCalculationStrategy, NavMeshPathStatus> SetAIAgentDestinationAction;
        public Action<AIMovementSpeedAttenuationFactor> SetAIAgentSpeedAttenuationAction;
        public Action ClearAIAgentPathAction;
        public Action<CoreInteractiveObject> AskToFireAFiredProjectile_WithTargetPosition_Action;
        public Func<WeaponHandlingFirePointOriginLocalDefinition> GetWeaponFirePointOriginLocalDefinitionAction;

        /// <summary>
        /// As <see cref="TrackAndKillPlayerStateManager"/> is itself a <see cref="TrackAndKillPlayerStateManager.TrackAndKillPlayerBehavior"/>,
        /// this callback must be called when parent state manager must exit <see cref="TrackAndKillPlayerStateManager"/> state.
        /// </summary>
        public Action AskedToExitTrackAndKillPlayerBehaviorAction;
        
        public Action OnShootingAtPlayerStartAction;
        public Action OnShootingAtPlayerEndAction;

        public TrackAndKillPlayerStateManagerExternalCallbacks(Func<IAgentMovementCalculationStrategy, NavMeshPathStatus> SetAIAgentDestinationAction, 
            Action<AIMovementSpeedAttenuationFactor> SetAIAgentSpeedAttenuationAction,
            Action clearAiAgentPathAction, Action<CoreInteractiveObject> askToFireAFiredProjectileWithTargetPositionAction, Func<WeaponHandlingFirePointOriginLocalDefinition> weaponFirePointOriginLocalDefinitionAction,
            Action askedToExitTrackAndKillPlayerBehaviorAction, Action onShootingAtPlayerStartAction, Action onShootingAtPlayerEndAction)
        {
            this.SetAIAgentDestinationAction = SetAIAgentDestinationAction;
            this.SetAIAgentSpeedAttenuationAction = SetAIAgentSpeedAttenuationAction;
            ClearAIAgentPathAction = clearAiAgentPathAction;
            AskToFireAFiredProjectile_WithTargetPosition_Action = askToFireAFiredProjectileWithTargetPositionAction;
            GetWeaponFirePointOriginLocalDefinitionAction = weaponFirePointOriginLocalDefinitionAction;
            AskedToExitTrackAndKillPlayerBehaviorAction = askedToExitTrackAndKillPlayerBehaviorAction;
            OnShootingAtPlayerStartAction = onShootingAtPlayerStartAction;
            OnShootingAtPlayerEndAction = onShootingAtPlayerEndAction;
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
                TrackAndKillPlayerStateManagerExternalCallbacks.SetAIAgentDestinationAction,
                TrackAndKillPlayerStateManagerExternalCallbacks.SetAIAgentSpeedAttenuationAction
            );
        }

        public static implicit operator ShootingAtPlayerStateManagerExternalCallbacks(TrackAndKillPlayerStateManagerExternalCallbacks TrackAndKillPlayerStateManagerExternalCallbacks)
        {
            return new ShootingAtPlayerStateManagerExternalCallbacks(
                TrackAndKillPlayerStateManagerExternalCallbacks.ClearAIAgentPathAction,
                TrackAndKillPlayerStateManagerExternalCallbacks.AskToFireAFiredProjectile_WithTargetPosition_Action,
                TrackAndKillPlayerStateManagerExternalCallbacks.OnShootingAtPlayerStartAction,
                TrackAndKillPlayerStateManagerExternalCallbacks.OnShootingAtPlayerEndAction
            );
        }

        public static implicit operator MoveAroundPlayerStateManagerExternalCallbacks(TrackAndKillPlayerStateManagerExternalCallbacks TrackAndKillPlayerStateManagerExternalCallbacks)
        {
            return new MoveAroundPlayerStateManagerExternalCallbacks(
                TrackAndKillPlayerStateManagerExternalCallbacks.SetAIAgentDestinationAction,
                TrackAndKillPlayerStateManagerExternalCallbacks.SetAIAgentSpeedAttenuationAction
            );
        }

        public static implicit operator MoveToLastSeenPlayerPositionStateManagerExternalCallbacks(TrackAndKillPlayerStateManagerExternalCallbacks TrackAndKillPlayerStateManagerExternalCallbacks)
        {
            return new MoveToLastSeenPlayerPositionStateManagerExternalCallbacks(
                TrackAndKillPlayerStateManagerExternalCallbacks.SetAIAgentDestinationAction,
                TrackAndKillPlayerStateManagerExternalCallbacks.SetAIAgentSpeedAttenuationAction,
                TrackAndKillPlayerStateManagerExternalCallbacks.AskedToExitTrackAndKillPlayerBehaviorAction);
        }
    }
}