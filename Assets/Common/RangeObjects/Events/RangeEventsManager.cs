using System;
using CoreGame;

namespace RangeObjects
{
    public class RangeEventsManager : GameSingleton<RangeEventsManager>
    {
        private event Action<RangeObjectV2> OnRangeObjectCreatedEvent;
        private event Action<RangeObjectV2> OnRangeObjectDestroyedEvent;

        public void RegisterOnRangeObjectCreatedEventListener(Action<RangeObjectV2> action)
        {
            this.OnRangeObjectCreatedEvent += action;
        }

        public void RegisterOnRangeObjectDestroyedEventListener(Action<RangeObjectV2> action)
        {
            this.OnRangeObjectDestroyedEvent += action;
        }

        public void OnRangeObjectCreated(RangeObjectV2 RangeObjectV2)
        {
            if (this.OnRangeObjectCreatedEvent != null)
            {
                this.OnRangeObjectCreatedEvent.Invoke(RangeObjectV2);
            }
        }

        public void OnRangeObjectDestroyed(RangeObjectV2 RangeObjectV2)
        {
            if (this.OnRangeObjectDestroyedEvent != null)
            {
                this.OnRangeObjectDestroyedEvent.Invoke(RangeObjectV2);
            }
        }
    }
}