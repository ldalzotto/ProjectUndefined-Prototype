using System;
using CoreGame;

namespace PlayerObject_Interfaces
{
    public class PlayerInteractiveObjectCreatedEvent : GameSingleton<PlayerInteractiveObjectCreatedEvent>
    {
        private event Action<IPlayerInteractiveObject> OnPlayerInteractiveObjectCreatedEvent;

        public void RegisterPlayerInteractiveObjectCreatedEvent(Action<IPlayerInteractiveObject> action)
        {
            this.OnPlayerInteractiveObjectCreatedEvent += action;
        }

        public void OnPlayerInteractiveObjectCreated(IPlayerInteractiveObject IPlayerInteractiveObject)
        {
            this.OnPlayerInteractiveObjectCreatedEvent?.Invoke(IPlayerInteractiveObject);
        }
    }
}