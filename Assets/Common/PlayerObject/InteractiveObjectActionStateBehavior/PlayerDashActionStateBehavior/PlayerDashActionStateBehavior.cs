using System.Collections.Generic;
using InteractiveObjectAction;
using UnityEngine;

namespace PlayerObject
{
    public enum PlayerDashActionState
    {
        LISTENING,
        DASH_DIRECTION,
        DASH_WARP
    }

    public abstract class APlayerDashActionStateManager : StateManager
    {
    }

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

        public bool TryingToExecuteDashTeleportationAction()
        {
            if (this.GetCurrentState() == PlayerDashActionState.LISTENING)
            {
                this.SetState(PlayerDashActionState.DASH_DIRECTION);
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

        public Vector3 GetTargetPointWorldPosition()
        {
            return this.PlayerDashTargetPositionTrackerSystem.PlayerDashTargetWorldPosition;
        }

        #endregion
    }
}