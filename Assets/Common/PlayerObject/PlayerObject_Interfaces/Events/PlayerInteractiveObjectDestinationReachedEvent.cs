using System;
using CoreGame;

namespace PlayerObject_Interfaces
{
    public class PlayerInteractiveObjectDestinationReachedEvent : GameSingleton<PlayerInteractiveObjectDestinationReachedEvent>
    {
        private event Action OnPlayerInteractiveObjectDestinationReachedEvent;

        public void RegisterOnPlayerInteractiveObjectDestinationReachedEventListener(Action action)
        {
            this.OnPlayerInteractiveObjectDestinationReachedEvent += action;
        }

        public void OnPlayerInteractiveObjectDestinationReached()
        {
            this.OnPlayerInteractiveObjectDestinationReachedEvent?.Invoke();
        }
    }
}