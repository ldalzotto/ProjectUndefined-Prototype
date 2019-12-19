using System;
using CoreGame;

namespace PlayerLowHealth_Interfaces
{
    public class PlayerLowHealthEndedEvent : GameSingleton<PlayerLowHealthEndedEvent>
    {
        private event Action OnPlayerLowHealthEndedEvent;

        public void RegisterPlayerLowHealthEndedEvent(Action action)
        {
            this.OnPlayerLowHealthEndedEvent += action;
        }

        public void UnRegisterPlayerLowHealthEndedEvent(Action action)
        {
            this.OnPlayerLowHealthEndedEvent -= action;
        }

        public void OnPlayerLowHealthEnded()
        {
            this.OnPlayerLowHealthEndedEvent?.Invoke();
        }
    }
}