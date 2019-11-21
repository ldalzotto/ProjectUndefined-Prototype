using System;
using CoreGame;

namespace StartMenu
{
    public class StartMenuEscapePressedEvent : GameSingleton<StartMenuEscapePressedEvent>
    {
        private event Action OnStartMenuEscapePressedEvent;

        public void RegisterOnStartMenuEscapePressedEventAction(Action action)
        {
            this.OnStartMenuEscapePressedEvent += action;
        }

        public void UnRegisterOnStartMenuEscapePressedEventAction(Action action)
        {
            this.OnStartMenuEscapePressedEvent -= action;
        }

        public void OnStartMenuEscapePressed()
        {
            this.OnStartMenuEscapePressedEvent?.Invoke();
        }
    }
}