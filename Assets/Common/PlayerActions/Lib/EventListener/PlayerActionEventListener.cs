using System;

namespace PlayerActions
{
    /// <summary>
    /// <see cref="PlayerActionEventListener"/> is a convenient way of having Start and Stopped events for any <see cref="PlayerAction"/>.
    /// </summary>
    public struct PlayerActionEventListener
    {
        private PlayerActionInherentData PlayerActionInherentData;

        #region Events callback

        private event Action<PlayerActionInherentData> OnPlayerActionStartEvent;
        private event Action<PlayerActionInherentData> OnPlayerActionStoppedEvent;

        #endregion

        public PlayerActionEventListener(PlayerActionInherentData playerActionInherentData)
        {
            this.PlayerActionInherentData = playerActionInherentData;
            OnPlayerActionStartEvent = null;
            OnPlayerActionStoppedEvent = null;
        }
        
        public void OnPlayerActionStart()
        {
            this.OnPlayerActionStartEvent?.Invoke(this.PlayerActionInherentData);
        }

        public void OnPlayerActionStopped()
        {
            this.OnPlayerActionStoppedEvent?.Invoke(this.PlayerActionInherentData);
        }

        #region Events Register

        public void RegisterOnPlayerActionStartEvent(Action<PlayerActionInherentData> action)
        {
            this.OnPlayerActionStartEvent += action;
        }

        public void UnRegisterOnPlayerActionStartEvent(Action<PlayerActionInherentData> action)
        {
            this.OnPlayerActionStartEvent -= action;
        }

        public void RegisterOnPlayerActionStopEvent(Action<PlayerActionInherentData> action)
        {
            this.OnPlayerActionStoppedEvent += action;
        }

        public void UnRegisterOnPlayerActionStopEvent(Action<PlayerActionInherentData> action)
        {
            this.OnPlayerActionStoppedEvent -= action;
        }
        #endregion
    }
}