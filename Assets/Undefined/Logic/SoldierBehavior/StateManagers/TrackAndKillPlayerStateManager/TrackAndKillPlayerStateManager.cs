using System;
using System.Collections.Generic;
using InteractiveObjects;

namespace SoliderAIBehavior
{
    public enum TrackAndKillPlayerStateEnum
    {
        LISTENING = 0,

        /// <summary>
        /// /!\ It is often not ecouraged to use this state. The state <see cref="TrackAndKillPlayerStateEnum.MOVE_TO_LAST_SEEN_PLAYER_POSITION"/>
        /// is always more appropriate and will automatically switch to <see cref="TrackAndKillPlayerStateEnum.MOVE_TOWARDS_PLAYER"/> if
        /// the player is in sight.
        /// This state must be used only when the player is in sight
        /// </summary>
        MOVE_TOWARDS_PLAYER = 1,
        GO_ROUND_PLAYER = 2,

        /// <summary>
        /// /!\ Switching to this state is only allowed if <see cref="SoldierAIBehaviorUtil.IsAllowToMoveToShootingAtPlayerState"/> conditions are fulfilled.
        /// </summary>
        SHOOTING_AT_PLAYER = 3,
        MOVE_TO_LAST_SEEN_PLAYER_POSITION = 4
    }


    public class TrackAndKillPlayerStateManager : SoldierStateManager
    {
        [VE_Nested] private TrackAndKillPlayerStateBehavior _trackAndKillPlayerStateBehavior;

        public TrackAndKillPlayerStateManager(CoreInteractiveObject AssociatedInteractiveObject, SoldierAIBehaviorDefinition SoldierAIBehaviorDefinition,
            PlayerObjectStateDataSystem PlayerObjectStateDataSystem,
            SoldierAIBehaviorExternalCallbacksV2 SoldierAIBehaviorExternalCallbacksV2,
            Action AskTrackAndKillPlayerStateBehaviorExitAction)
        {
            this._trackAndKillPlayerStateBehavior = new TrackAndKillPlayerStateBehavior();
            this._trackAndKillPlayerStateBehavior.Init(AssociatedInteractiveObject, SoldierAIBehaviorDefinition, PlayerObjectStateDataSystem, SoldierAIBehaviorExternalCallbacksV2, AskTrackAndKillPlayerStateBehaviorExitAction);
        }

        public override void Tick(float d)
        {
            base.Tick(d);
            this._trackAndKillPlayerStateBehavior.Tick(d);
        }

        /// <summary>
        /// Incrementing the <see cref="_trackAndKillPlayerStateBehavior"/> to it's first state
        /// </summary>
        public override void OnStateEnter()
        {
            this._trackAndKillPlayerStateBehavior.OnParentStateManagerEnter();
            this._trackAndKillPlayerStateBehavior.SetState(TrackAndKillPlayerStateEnum.MOVE_TO_LAST_SEEN_PLAYER_POSITION);
        }

        /// <summary>
        /// Resetting <see cref="_trackAndKillPlayerStateBehavior"/> to it's starting state
        /// </summary>
        public override void OnStateExit()
        {
            this._trackAndKillPlayerStateBehavior.OnParentStateManagerExit();
            this._trackAndKillPlayerStateBehavior.SetState(TrackAndKillPlayerStateEnum.LISTENING);
        }

        #region Internal Sight Events

        public override void OnPlayerObjectJustOnSight(CoreInteractiveObject InSightInteractiveObject)
        {
            this._trackAndKillPlayerStateBehavior.GetCurrentStateManager().OnPlayerObjectJustOnSight(InSightInteractiveObject);
        }

        public override void OnPlayerObjectJustOutOfSight(CoreInteractiveObject NotInSightInteractiveObject)
        {
            this._trackAndKillPlayerStateBehavior.GetCurrentStateManager().OnPlayerObjectJustOutOfSight(NotInSightInteractiveObject);
        }

        #endregion

        #region External Agent Events

        public override void OnDestinationReached()
        {
            this._trackAndKillPlayerStateBehavior.GetCurrentStateManager().OnDestinationReached();
        }

        #endregion
    }

    /// <summary>
    /// Handles the AI behavior when the Player has been seen. The solider try to look for player is sight is lost <see cref="TrackAndKillPlayerStateEnum.GO_ROUND_PLAYER"/> and
    /// <see cref="TrackAndKillPlayerStateEnum.MOVE_TO_LAST_SEEN_PLAYER_POSITION"/>.
    /// And shoot to it when in range <see cref="TrackAndKillPlayerStateEnum.SHOOTING_AT_PLAYER"/>.
    /// </summary>
    public class TrackAndKillPlayerStateBehavior : StateBehavior<TrackAndKillPlayerStateEnum, SoldierStateManager>
    {
        private WeaponFiringAreaSystem WeaponFiringAreaSystem;

        public void Init(CoreInteractiveObject AssociatedInteractiveObject, SoldierAIBehaviorDefinition SoldierAIBehaviorDefinition,
            PlayerObjectStateDataSystem PlayerObjectStateDataSystem, SoldierAIBehaviorExternalCallbacksV2 SoldierAIBehaviorExternalCallbacksV2,
            Action AskTrackAndKillPlayerStateBehaviorExitAction)
        {
            this.WeaponFiringAreaSystem = new WeaponFiringAreaSystem(AssociatedInteractiveObject, PlayerObjectStateDataSystem, SoldierAIBehaviorExternalCallbacksV2, SoldierAIBehaviorExternalCallbacksV2);
            this.StateManagersLookup = new Dictionary<TrackAndKillPlayerStateEnum, SoldierStateManager>()
            {
                {TrackAndKillPlayerStateEnum.LISTENING, new ListeningStateManager()},
                {TrackAndKillPlayerStateEnum.MOVE_TOWARDS_PLAYER, new MoveTowardsPlayerStateManager(this, SoldierAIBehaviorDefinition, AssociatedInteractiveObject, PlayerObjectStateDataSystem, this.WeaponFiringAreaSystem, SoldierAIBehaviorExternalCallbacksV2)},
                {TrackAndKillPlayerStateEnum.SHOOTING_AT_PLAYER, new ShootingAtPlayerStateManager(this, PlayerObjectStateDataSystem, AssociatedInteractiveObject, SoldierAIBehaviorDefinition, this.WeaponFiringAreaSystem, SoldierAIBehaviorExternalCallbacksV2, SoldierAIBehaviorExternalCallbacksV2, SoldierAIBehaviorExternalCallbacksV2)},
                {TrackAndKillPlayerStateEnum.GO_ROUND_PLAYER, new MoveAroundPlayerStateManager(this, PlayerObjectStateDataSystem, AssociatedInteractiveObject, this.WeaponFiringAreaSystem, SoldierAIBehaviorExternalCallbacksV2)},
                {TrackAndKillPlayerStateEnum.MOVE_TO_LAST_SEEN_PLAYER_POSITION, new MoveToLastSeenPlayerPositionStateManager(this, PlayerObjectStateDataSystem, SoldierAIBehaviorExternalCallbacksV2, AskTrackAndKillPlayerStateBehaviorExitAction)}
            };
            base.Init(TrackAndKillPlayerStateEnum.LISTENING);
        }

        public override void Tick(float d)
        {
            this.WeaponFiringAreaSystem.Tick(d);
            base.Tick(d);
        }

        public void OnParentStateManagerEnter()
        {
            this.WeaponFiringAreaSystem.Enable();
        }

        public void OnParentStateManagerExit()
        {
            this.WeaponFiringAreaSystem.Disable();
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