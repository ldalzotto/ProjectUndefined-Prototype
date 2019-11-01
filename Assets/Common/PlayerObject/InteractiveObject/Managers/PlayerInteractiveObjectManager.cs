using CoreGame;

namespace PlayerObject
{
    public class PlayerInteractiveObjectManager : GameSingleton<PlayerInteractiveObjectManager>
    {
        public PlayerInteractiveObject PlayerInteractiveObject { get; private set; }

        public void Init(PlayerInteractiveObject PlayerInteractiveObject)
        {
            this.PlayerInteractiveObject = PlayerInteractiveObject;
        }

        public void FixedTick(float d)
        {
            PlayerInteractiveObject.FixedTick(d);
        }

        public void Tick(float d)
        {
            PlayerInteractiveObject.Tick(d);
        }

        public void AfterTicks(float d)
        {
            this.PlayerInteractiveObject.AfterTicks(d);
        }

        public void LateTick(float d)
        {
            PlayerInteractiveObject.LateTick(d);
        }
    }
}