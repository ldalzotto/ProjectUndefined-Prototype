using System;
using CoreGame;

namespace PlayerActions_Interfaces
{
    public class PlayerActionsEventListenerManager : GameSingleton<PlayerActionsEventListenerManager>
    {
        private event Action OnPlayerActionSelectionWheelAwakeEvent;

        public void RegisterOnPlayerActionSelectionWheelAwakeEventListener(Action action)
        {
            this.OnPlayerActionSelectionWheelAwakeEvent += action;
        }

        public void UnRegisterOnPlayerActionSelectionWheelAwakeEventListener(Action action)
        {
            this.OnPlayerActionSelectionWheelAwakeEvent -= action;
        }

        public void OnPlayerActionSelectionWheelAwake()
        {
            if (this.OnPlayerActionSelectionWheelAwakeEvent != null)
            {
                this.OnPlayerActionSelectionWheelAwakeEvent.Invoke();
            }
        }
    }
}