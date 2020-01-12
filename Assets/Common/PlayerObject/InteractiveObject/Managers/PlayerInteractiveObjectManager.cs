using CoreGame;
using PlayerObject_Interfaces;

namespace PlayerObject
{
    public class PlayerInteractiveObjectManager : GameSingleton<PlayerInteractiveObjectManager>
    {
        public PlayerAimingInteractiveObject PlayerAimingInteractiveObject { get; private set; }

        public void InitializeEvents()
        {
            PlayerInteractiveObjectCreatedEvent.Get().RegisterPlayerInteractiveObjectCreatedEvent(this.OnPlayerInteractiveObjectCreated);
            PlayerInteractiveObjectDestroyedEvent.Get().RegisterPlayerInteractiveObjectDestroyedEvent(this.OnPlayerInteractiveObjectDestroyed);
        }

        public void FixedTick(float d)
        {
            PlayerAimingInteractiveObject?.FixedTick(d);
        }

        public void Tick(float d)
        {
            PlayerAimingInteractiveObject?.Tick(d);
        }

        public void TickTimeFrozen(float d)
        {
            PlayerAimingInteractiveObject?.TickTimeFrozen(d);
        }

        public void AfterTicks(float d)
        {
            PlayerAimingInteractiveObject?.AfterTicks(d);
        }

        public void LateTick(float d)
        {
            PlayerAimingInteractiveObject?.LateTick(d);
        }
        
        #region External Events

        public void OnPlayerInteractiveObjectCreated(IPlayerInteractiveObject PlayerInteractiveObject)
        {
            this.PlayerAimingInteractiveObject = (PlayerAimingInteractiveObject)PlayerInteractiveObject;
        }
        public void OnPlayerInteractiveObjectDestroyed()
        {
            this.PlayerAimingInteractiveObject = null;
        }
        #endregion
    }
}