namespace PlayerObject
{
    public class PlayerDashActionListeningStateManager : APlayerDashActionStateManager
    {
        private PlayerDashActionStateBehavior PlayerDashActionStateBehavior;

        public PlayerDashActionListeningStateManager(PlayerDashActionStateBehavior playerDashActionStateBehavior)
        {
            PlayerDashActionStateBehavior = playerDashActionStateBehavior;
        }

    }
}