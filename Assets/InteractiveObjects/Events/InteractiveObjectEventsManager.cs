using System;
using CoreGame;

namespace InteractiveObjects
{
    public interface IInteractiveObjectEventsManager
    {
        void RegisterOnInteractiveObjectCreatedEventListener(Action<CoreInteractiveObject> action);
        void UpRegisterOnInteractiveObjectCreatedEventListener(Action<CoreInteractiveObject> action);
        void RegisterOnInteractiveObjectDestroyedEventListener(Action<CoreInteractiveObject> action);
        void UnRegisterOnInteractiveObjectDestroyedEventListener(Action<CoreInteractiveObject> action);
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
        public void RegisterOnInteractiveObjectCreatedEventListener(Action<CoreInteractiveObject> action)
        {
            OnInteractiveObjectCreatedEvent += action;
        }

        public void UpRegisterOnInteractiveObjectCreatedEventListener(Action<CoreInteractiveObject> action)
        {
            OnInteractiveObjectCreatedEvent -= action;
        }

        public void RegisterOnInteractiveObjectDestroyedEventListener(Action<CoreInteractiveObject> action)
        {
            OnInteractiveObjectDestroyedEvent += action;
        }

        public void UnRegisterOnInteractiveObjectDestroyedEventListener(Action<CoreInteractiveObject> action)
        {
            OnInteractiveObjectDestroyedEvent -= action;
        }

        private event Action<CoreInteractiveObject> OnInteractiveObjectCreatedEvent;
        private event Action<CoreInteractiveObject> OnInteractiveObjectDestroyedEvent;

        public void OnInteractiveObjectCreated(CoreInteractiveObject CoreInteractiveObject)
        {
            if (OnInteractiveObjectCreatedEvent != null) OnInteractiveObjectCreatedEvent.Invoke(CoreInteractiveObject);
        }

        public void OnInteractiveObjectDestroyed(CoreInteractiveObject CoreInteractiveObject)
        {
            if (OnInteractiveObjectDestroyedEvent != null) OnInteractiveObjectDestroyedEvent.Invoke(CoreInteractiveObject);
        }
    }
}