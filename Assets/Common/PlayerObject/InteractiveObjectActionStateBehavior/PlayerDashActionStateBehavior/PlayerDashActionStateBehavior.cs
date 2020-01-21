using System.Collections.Generic;
using InteractiveObjectAction;
using UnityEngine;
using PlayerDash;

namespace PlayerObject
{
    public enum PlayerDashActionState
    {

        LISTENING,
        /// <summary>
        /// <see cref="PlayerDashActionState.DASH_DIRECTION"/> is a state that correspond to the execution of <see cref="DashTeleportationDirectionAction"/>.
        /// </summary>
        DASH_DIRECTION
    }

    public abstract class APlayerDashActionStateManager : StateManager
    {
    }

    /// <summary>
    /// Manages conditions on execution of <see cref="PlayerDashAction"/> and <see cref="DashTeleportationDirectionAction"/>.
    /// </summary>
    public class PlayerDashActionStateBehavior : StateBehavior<PlayerDashActionState, APlayerDashActionStateManager>
    {
        private PlayerDashActionStateBehaviorInputDataSystemDefinition _playerDashActionStateBehaviorInputDataSystemDefinition;

        private PlayerDashTargetPositionTrackerSystem PlayerDashTargetPositionTrackerSystem;

        public PlayerDashActionStateBehavior(PlayerDashActionStateBehaviorInputDataSystemDefinition PlayerDashActionStateBehaviorInputDataSystemDefinition,
            InteractiveObjectActionPlayerSystem InteractiveObjectActionPlayerSystem)
        {
            this._playerDashActionStateBehaviorInputDataSystemDefinition = PlayerDashActionStateBehaviorInputDataSystemDefinition;
            this.PlayerDashTargetPositionTrackerSystem = new PlayerDashTargetPositionTrackerSystem();

            base.StateManagersLookup = new Dictionary<PlayerDashActionState, APlayerDashActionStateManager>()
            {
                {PlayerDashActionState.LISTENING, new PlayerDashActionListeningStateManager(this)},
                {
                    PlayerDashActionState.DASH_DIRECTION, new PlayerDashDirectionActionStateManager(this, InteractiveObjectActionPlayerSystem,
                        this._playerDashActionStateBehaviorInputDataSystemDefinition, ref PlayerDashTargetPositionTrackerSystem)
                }
            };

            base.Init(PlayerDashActionState.LISTENING);
        }

        #region External Events

        /// <summary>
        /// This method is called when the <see cref="PlayerDashAction"/> is trying to get executed. (usually called from a <see cref="Skill.SkillSlot"/>).
        /// For having the <see cref="PlayerDashAction"/> getting the correct inputs, the <see cref="DashTeleportationDirectionAction"/> must be executed first.
        /// The execution of <see cref="DashTeleportationDirectionAction"/> is translate by getting in the <see cref="PlayerDashActionState.DASH_DIRECTION"/> state.
        /// </summary>
        public bool TryingToExecuteDashTeleportationAction()
        {
            /// <see cref="DashTeleportationDirectionAction"/> is not executed.
            if (this.GetCurrentState() == PlayerDashActionState.LISTENING)
            {
                /// We execute it
                this.SetState(PlayerDashActionState.DASH_DIRECTION);
                /// And prevent <see cref="PlayerDashAction"/> execution.</summary>
                return false;
            }
            else if (this.GetCurrentState() == PlayerDashActionState.DASH_DIRECTION)
            {
                this.SetState(PlayerDashActionState.LISTENING);
                return true;
            }

            return false;
        }

        #endregion

        #region Data retrieval

        public Vector3? GetTargetPointWorldPosition()
        {
            return this.PlayerDashTargetPositionTrackerSystem.PlayerDashTargetWorldPosition;
        }

        #endregion
    }
}