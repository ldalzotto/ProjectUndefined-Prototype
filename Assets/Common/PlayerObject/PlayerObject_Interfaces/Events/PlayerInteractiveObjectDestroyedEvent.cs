using System;
using CoreGame;

namespace PlayerObject_Interfaces
{
    public class PlayerInteractiveObjectDestroyedEvent : GameSingleton<PlayerInteractiveObjectDestroyedEvent>
    {
        private event Action OnPlayerInteractiveObjectDestroyedEvent;

        public void RegisterPlayerInteractiveObjectDestroyedEvent(Action action)
        {
            this.OnPlayerInteractiveObjectDestroyedEvent += action;
        }
        public void UnRegisterPlayerInteractiveObjectDestroyedEvent(Action action)
        {
            this.OnPlayerInteractiveObjectDestroyedEvent -= action;
        }

        public void OnPlayerInteractiveObjectDestroyed()
        {
            this.OnPlayerInteractiveObjectDestroyedEvent?.Invoke();
        }
    }
}