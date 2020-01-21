namespace PlayerObject
{
    public class ListeningAimingInteractiveObjectActionStateManager : AAimingInteractiveObjectActionStateManager
    {
        private FiringInteractiveObjectActionStateBehavior FiringInteractiveObjectActionStateBehavior;
        private AimingInteractiveObjectActionStateBehaviorInputDataSystem _aimingInteractiveObjectActionStateBehaviorInputDataSystemRef;

        public ListeningAimingInteractiveObjectActionStateManager(FiringInteractiveObjectActionStateBehavior FiringInteractiveObjectActionStateBehaviorRef,
            ref AimingInteractiveObjectActionStateBehaviorInputDataSystem aimingInteractiveObjectActionStateBehaviorInputDataSystemRef)
        {
            this.FiringInteractiveObjectActionStateBehavior = FiringInteractiveObjectActionStateBehaviorRef;
            this._aimingInteractiveObjectActionStateBehaviorInputDataSystemRef = aimingInteractiveObjectActionStateBehaviorInputDataSystemRef;
        }

        public override void Tick(float d)
        {
            base.Tick(d);
            this.PlayerActionTriggering();
        }

        /// <summary>
        /// We want to be able to move to <see cref="AimingInteractiveObjectActionState.AIMING"/> even if time is frozen.
        /// This is to allow the Player to execute accurate shots without getting rushed by time. 
        /// </summary>
        public override void TickTimeFrozen(float d)
        {
            base.TickTimeFrozen(d);
            this.Tick(d);
        }

        /// <summary>
        /// Starts a new <see cref="AInteractiveObjectAction"/> if input condition and player inherent conditions are met.
        /// </summary>
        private void PlayerActionTriggering()
        {
            if (this._aimingInteractiveObjectActionStateBehaviorInputDataSystemRef.GameInputManager.CurrentInput.FiringActionDown())
            {
                this._aimingInteractiveObjectActionStateBehaviorInputDataSystemRef.InteractiveObjectActionPlayerSystem.ExecuteActionV2
                    (this._aimingInteractiveObjectActionStateBehaviorInputDataSystemRef.firingInteractiveObjectActionInherentData);
                this.FiringInteractiveObjectActionStateBehavior.SetState(AimingInteractiveObjectActionState.AIMING);
            }
        }
    }
}