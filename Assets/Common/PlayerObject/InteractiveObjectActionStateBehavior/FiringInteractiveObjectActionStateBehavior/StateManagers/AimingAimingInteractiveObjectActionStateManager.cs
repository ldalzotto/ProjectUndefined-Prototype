namespace PlayerObject
{
    public class AimingAimingInteractiveObjectActionStateManager : AAimingInteractiveObjectActionStateManager
    {
        private FiringInteractiveObjectActionStateBehavior FiringInteractiveObjectActionStateBehavior;

        public AimingAimingInteractiveObjectActionStateManager(FiringInteractiveObjectActionStateBehavior firingInteractiveObjectActionStateBehavior)
        {
            FiringInteractiveObjectActionStateBehavior = firingInteractiveObjectActionStateBehavior;
        }

        public override void StopTargetting()
        {
            this.FiringInteractiveObjectActionStateBehavior.SetState(AimingInteractiveObjectActionState.LISTENING);
        }
    }
}