using System;
using CoreGame;

namespace StartMenu
{
    public class ControlsWindowExitedEvent : GameSingleton<ControlsWindowExitedEvent>
    {
        private event Action OnControlsWindowExitedEvent;

        public void RegisterControlsWindowExitedEventAction(Action action)
        {
            this.OnControlsWindowExitedEvent += action;
        }

        public void OnControlsWindowExited()
        {
            this.OnControlsWindowExitedEvent?.Invoke();
        }
    }
}