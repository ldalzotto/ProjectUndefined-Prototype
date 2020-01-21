using Input;
using InteractiveObjectAction;
using PlayerDash;

namespace PlayerObject
{

    /// <summary>
    /// Responsible of executing the <see cref="DashTeleportationDirectionAction"/> and storing the target value in <see cref="PlayerDashTargetPositionTrackerSystem"/>.
    /// </summary>
    public class PlayerDashDirectionActionStateManager : APlayerDashActionStateManager
    {
        private InteractiveObjectActionPlayerSystem InteractiveObjectActionPlayerSystem;
        private PlayerDashActionStateBehaviorInputDataSystemDefinition _playerDashActionStateBehaviorInputDataSystemDefinitionRef;
        private PlayerDashTargetPositionTrackerSystem PlayerDashTargetPositionTrackerSystemRef;
        private GameInputManager GameInputManager = GameInputManager.Get();
        private PlayerDashActionStateBehavior PlayerDashActionStateBehavior;

        public PlayerDashDirectionActionStateManager(
            PlayerDashActionStateBehavior PlayerDashActionStateBehavior,
            InteractiveObjectActionPlayerSystem interactiveObjectActionPlayerSystem,
            PlayerDashActionStateBehaviorInputDataSystemDefinition playerDashActionStateBehaviorInputDataSystemDefinitionRef,
            ref PlayerDashTargetPositionTrackerSystem PlayerDashTargetPositionTrackerSystemRef)
        {
            this.PlayerDashActionStateBehavior = PlayerDashActionStateBehavior;
            InteractiveObjectActionPlayerSystem = interactiveObjectActionPlayerSystem;
            this._playerDashActionStateBehaviorInputDataSystemDefinitionRef = playerDashActionStateBehaviorInputDataSystemDefinitionRef;
            this.PlayerDashTargetPositionTrackerSystemRef = PlayerDashTargetPositionTrackerSystemRef;
        }

        /// <summary>
        /// On state enter, we execute the  <see cref="DashTeleportationDirectionAction"/>.
        /// </summary>
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            this.InteractiveObjectActionPlayerSystem.ExecuteActionV2(this._playerDashActionStateBehaviorInputDataSystemDefinitionRef.DashTeleportationDirectionActionDefinition);
            this.Tick(0f);
        }

        /// <summary>
        /// On state exit, the <see cref="DashTeleportationDirectionAction"/> is stopped.
        /// The last value of <see cref="PlayerDashTargetPositionTrackerSystem"/> is persisted before destroying just in case.
        /// </summary>
        public override void OnStateExit()
        {
            base.OnStateExit();
            this.Tick(0f);
            this.InteractiveObjectActionPlayerSystem.StopAction(DashTeleportationDirectionAction.DashTeleportationDirectionActionUniqueID);
        }

        public override void Tick(float d)
        {
            base.Tick(d);
        
            if (this.GameInputManager.CurrentInput.CancelButtonD())
            {
                this.PlayerDashActionStateBehavior.SetState(PlayerDashActionState.LISTENING);
            }
            else
            {
                if (this.InteractiveObjectActionPlayerSystem.GetPlayingPlayerActionReference(DashTeleportationDirectionAction.DashTeleportationDirectionActionUniqueID) is DashTeleportationDirectionAction dashTeleportationDirectionAction)
                {
                    this.PlayerDashTargetPositionTrackerSystemRef.PlayerDashTargetWorldPosition = dashTeleportationDirectionAction.GetTargetWorldPosition();
                }
            }
        }

        public override void TickTimeFrozen(float d)
        {
            base.TickTimeFrozen(d);
            this.Tick(d);
        }
    }
}