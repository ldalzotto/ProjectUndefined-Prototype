using System;

namespace PlayerActions
{
    /// <summary>
    /// <see cref="InteractiveObjectActionEventListener"/> is a convenient way of having Start and Stopped events for any <see cref="PlayerAction"/>.
    /// </summary>
    public struct InteractiveObjectActionEventListener
    {
        private InteractiveObjectActionInherentData _interactiveObjectActionInherentData;

        #region Events callback

        private event Action<InteractiveObjectActionInherentData> OnInteractiveObjectActionStartEvent;
        private event Action<InteractiveObjectActionInherentData> OnInteractiveObjectActionStoppedEvent;

        #endregion

        public InteractiveObjectActionEventListener(InteractiveObjectActionInherentData interactiveObjectActionInherentData)
        {
            this._interactiveObjectActionInherentData = interactiveObjectActionInherentData;
            OnInteractiveObjectActionStartEvent = null;
            OnInteractiveObjectActionStoppedEvent = null;
        }
        
        public void OnInteractiveObjectActionStart()
        {
            this.OnInteractiveObjectActionStartEvent?.Invoke(this._interactiveObjectActionInherentData);
        }

        public void OnInteractiveObjectActionStopped()
        {
            this.OnInteractiveObjectActionStoppedEvent?.Invoke(this._interactiveObjectActionInherentData);
        }

        #region Events Register

        public void RegisterOnInteractiveObjectActionStartEvent(Action<InteractiveObjectActionInherentData> action)
        {
            this.OnInteractiveObjectActionStartEvent += action;
        }

        public void UnRegisterOnInteractiveObjectActionStartEvent(Action<InteractiveObjectActionInherentData> action)
        {
            this.OnInteractiveObjectActionStartEvent -= action;
        }

        public void RegisterOnPlayerActionStopEvent(Action<InteractiveObjectActionInherentData> action)
        {
            this.OnInteractiveObjectActionStoppedEvent += action;
        }

        public void UnRegisterOnInteractiveObjectActionStopEvent(Action<InteractiveObjectActionInherentData> action)
        {
            this.OnInteractiveObjectActionStoppedEvent -= action;
        }
        #endregion
    }
}