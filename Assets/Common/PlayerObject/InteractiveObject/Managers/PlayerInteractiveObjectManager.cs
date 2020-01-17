using CoreGame;
using PlayerObject_Interfaces;

namespace PlayerObject
{
    public class PlayerInteractiveObjectManager : GameSingleton<PlayerInteractiveObjectManager>
    {
        public PlayerInteractiveObject PlayerInteractiveObject { get; private set; }

        public void InitializeEvents()
        {
            PlayerInteractiveObjectCreatedEvent.Get().RegisterPlayerInteractiveObjectCreatedEvent(this.OnPlayerInteractiveObjectCreated);
            PlayerInteractiveObjectDestroyedEvent.Get().RegisterPlayerInteractiveObjectDestroyedEvent(this.OnPlayerInteractiveObjectDestroyed);
        }

        public void FixedTick(float d)
        {
            PlayerInteractiveObject?.FixedTick(d);
        }

        public void FixedTickTimeFrozen(float d)
        {
            PlayerInteractiveObject?.FixedTickTimeFrozen(d);
        }
        
        public void Tick(float d)
        {
            PlayerInteractiveObject?.Tick(d);
        }

        public void TickTimeFrozen(float d)
        {
            PlayerInteractiveObject?.TickTimeFrozen(d);
        }

        public void AfterTicks(float d)
        {
            PlayerInteractiveObject?.AfterTicks(d);
        }

        public void LateTick(float d)
        {
            PlayerInteractiveObject?.LateTick(d);
        }
        
        #region External Events

        public void OnPlayerInteractiveObjectCreated(IPlayerInteractiveObject PlayerInteractiveObject)
        {
            this.PlayerInteractiveObject = (PlayerInteractiveObject)PlayerInteractiveObject;
        }
        public void OnPlayerInteractiveObjectDestroyed()
        {
            this.PlayerInteractiveObject = null;
        }
        #endregion
    }
}