using System;
using System.Collections.Generic;
using AIObjects;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using UnityEngine;
using UnityEngine.AI;
using Weapon;

namespace SoliderAIBehavior
{
    public enum TrackAndKillPlayerStateEnum
    {
        /// <summary>
        /// /!\ It is often not ecouraged to use this state. The state <see cref="TrackAndKillPlayerStateEnum.MOVE_TO_LAST_SEEN_PLAYER_POSITION"/>
        /// is always more appropriate and will automatically switch to <see cref="TrackAndKillPlayerStateEnum.MOVE_TOWARDS_PLAYER"/> if
        /// the player is in sight.
        /// This state must be used only when the player is in sight
        /// </summary>
        MOVE_TOWARDS_PLAYER = 0,
        GO_ROUND_PLAYER = 1,

        /// <summary>
        /// /!\ Switching to this state is only allowed if <see cref="SoldierAIBehaviorUtil.IsAllowToMoveToShootingAtPlayerState"/> conditions are fulfilled.
        /// </summary>
        SHOOTING_AT_PLAYER = 2,
        MOVE_TO_LAST_SEEN_PLAYER_POSITION = 3
    }

    public class TrackAndKillPlayerStateManager : SoldierStateManager
    {
        private TrackAndKillPlayerBehavior TrackAndKillPlayerBehavior;

        public TrackAndKillPlayerStateManager(CoreInteractiveObject AssociatedInteractiveObject, SoldierAIBehaviorDefinition SoldierAIBehaviorDefinition,
            PlayerObjectStateDataSystem PlayerObjectStateDataSystem,
            Func<IAgentMovementCalculationStrategy, AIMovementSpeedDefinition, NavMeshPathStatus> destinationAction, Action ClearpathAction, Action<Vector3> AskToFireAFiredProjectileAction_WithTargetPosition,
            Func<WeaponHandlingFirePointOriginLocalDefinition> GetWeaponFirePointOriginLocalDefinitionAction,
            Action AskedToExitTrackAndKillPlayerBehaviorAction)
        {
            this.TrackAndKillPlayerBehavior = new TrackAndKillPlayerBehavior(AssociatedInteractiveObject, SoldierAIBehaviorDefinition,
                PlayerObjectStateDataSystem,
                destinationAction, ClearpathAction, AskToFireAFiredProjectileAction_WithTargetPosition,
                GetWeaponFirePointOriginLocalDefinitionAction,
                AskedToExitTrackAndKillPlayerBehaviorAction);
        }

        public override void Tick(float d)
        {
            base.Tick(d);
            this.TrackAndKillPlayerBehavior.Tick(d);
        }

        /// <summary>
        /// Resetting <see cref="TrackAndKillPlayerBehavior"/> to it's starting state
        /// </summary>
        public override void OnStateEnter()
        {
            this.TrackAndKillPlayerBehavior.SetState(TrackAndKillPlayerStateEnum.MOVE_TO_LAST_SEEN_PLAYER_POSITION);
        }

        /// <summary>
        /// Resetting <see cref="TrackAndKillPlayerBehavior"/> to it's starting state
        /// </summary>
        public override void OnStateExit()
        {
            this.TrackAndKillPlayerBehavior.SetState(TrackAndKillPlayerStateEnum.MOVE_TO_LAST_SEEN_PLAYER_POSITION);
        }
        
                
        #region External Agent Events

        public override void OnDestinationReached()
        {
            this.TrackAndKillPlayerBehavior.GetCurrentStateManager().OnDestinationReached();
        }

        #endregion
    }

    /// <summary>
    /// Handles the AI behavior when the Player has been seen. The solider try to look for player is sight is lost <see cref="TrackAndKillPlayerStateEnum.GO_ROUND_PLAYER"/> and
    /// <see cref="TrackAndKillPlayerStateEnum.MOVE_TO_LAST_SEEN_PLAYER_POSITION"/>.
    /// And shoot to it when in range <see cref="TrackAndKillPlayerStateEnum.SHOOTING_AT_PLAYER"/>.
    /// </summary>
    public class TrackAndKillPlayerBehavior : AIBehavior<TrackAndKillPlayerStateEnum, SoldierStateManager>
    {
        private WeaponFiringAreaSystem WeaponFiringAreaSystem;

        public TrackAndKillPlayerBehavior(CoreInteractiveObject AssociatedInteractiveObject, SoldierAIBehaviorDefinition SoldierAIBehaviorDefinition,
            PlayerObjectStateDataSystem PlayerObjectStateDataSystem,
            Func<IAgentMovementCalculationStrategy, AIMovementSpeedDefinition, NavMeshPathStatus> destinationAction, Action ClearpathAction, Action<Vector3> AskToFireAFiredProjectileAction_WithTargetPosition,
            Func<WeaponHandlingFirePointOriginLocalDefinition> GetWeaponFirePointOriginLocalDefinitionAction,
            Action AskedToExitTrackAndKillPlayerBehaviorAction) : base(TrackAndKillPlayerStateEnum.MOVE_TO_LAST_SEEN_PLAYER_POSITION)
        {
            this.WeaponFiringAreaSystem = new WeaponFiringAreaSystem(AssociatedInteractiveObject, PlayerObjectStateDataSystem, GetWeaponFirePointOriginLocalDefinitionAction);

            this.StateManagersLookup = new Dictionary<TrackAndKillPlayerStateEnum, SoldierStateManager>()
            {
                {TrackAndKillPlayerStateEnum.MOVE_TOWARDS_PLAYER, new MoveTowardsPlayerStateManager(this, SoldierAIBehaviorDefinition, AssociatedInteractiveObject, PlayerObjectStateDataSystem, this.WeaponFiringAreaSystem, destinationAction)},
                {TrackAndKillPlayerStateEnum.SHOOTING_AT_PLAYER, new ShootingAtPlayerStateManager(this, PlayerObjectStateDataSystem, AssociatedInteractiveObject, ClearpathAction, AskToFireAFiredProjectileAction_WithTargetPosition)},
                {TrackAndKillPlayerStateEnum.GO_ROUND_PLAYER, new MoveAroundPlayerStateManager(this, PlayerObjectStateDataSystem, AssociatedInteractiveObject, this.WeaponFiringAreaSystem, destinationAction)},
                {TrackAndKillPlayerStateEnum.MOVE_TO_LAST_SEEN_PLAYER_POSITION, new MoveToLastSeenPlayerPositionStateManager(this, PlayerObjectStateDataSystem, destinationAction, AskedToExitTrackAndKillPlayerBehaviorAction)}
            };
        }

        public override void Tick(float d)
        {
            this.WeaponFiringAreaSystem.Tick(d);
            base.Tick(d);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            
            foreach (var stateManager in this.StateManagersLookup.Values)
            {
                stateManager.OnDestroy();
            }

            this.WeaponFiringAreaSystem.OnDestroy();
        }
        
    }
}