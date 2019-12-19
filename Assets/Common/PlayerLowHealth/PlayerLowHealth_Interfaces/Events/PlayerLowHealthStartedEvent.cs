using System;
using CoreGame;

namespace PlayerLowHealth_Interfaces
{
    public class PlayerLowHealthStartedEvent : GameSingleton<PlayerLowHealthStartedEvent>
    {
        private event Action OnPlayerLowHealthStartedEvent;

        public void RegisterPlayerLowHealthStartedEvent(Action action)
        {
            this.OnPlayerLowHealthStartedEvent += action;
        }

        public void UnRegisterPlayerLowHealthStartedEvent(Action action)
        {
            this.OnPlayerLowHealthStartedEvent -= action;
        }

        public void OnPlayerLowHealthStarted()
        {
            this.OnPlayerLowHealthStartedEvent?.Invoke();
        }
    }
}