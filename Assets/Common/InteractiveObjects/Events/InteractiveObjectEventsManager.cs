using System;
using CoreGame;

namespace InteractiveObjects
{
    public interface IInteractiveObjectEventsManager
    {
        void RegisterOnAllInteractiveObjectCreatedEventListener(Action<CoreInteractiveObject> action);
        void UnRegisterOnAllInteractiveObjectCreatedEventListener(Action<CoreInteractiveObject> action);
    }

    public static class InteractiveObjectEventsManagerSingleton
    {
        public static IInteractiveObjectEventsManager Get()
        {
            return InteractiveObjectEventsManager.Get();
        }
    }

    public class InteractiveObjectEventsManager : GameSingleton<InteractiveObjectEventsManager>, IInteractiveObjectEventsManager
    {
        public void RegisterOnAllInteractiveObjectCreatedEventListener(Action<CoreInteractiveObject> action)
        {
            OnAllInteractiveObjectCreatedEvent += action;
        }

        public void UnRegisterOnAllInteractiveObjectCreatedEventListener(Action<CoreInteractiveObject> action)
        {
            OnAllInteractiveObjectCreatedEvent -= action;
        }
        
        private event Action<CoreInteractiveObject> OnAllInteractiveObjectCreatedEvent;

        public void OnInteractiveObjectCreated(CoreInteractiveObject CoreInteractiveObject)
        {
            if (OnAllInteractiveObjectCreatedEvent != null) OnAllInteractiveObjectCreatedEvent.Invoke(CoreInteractiveObject);
        }

    }
}