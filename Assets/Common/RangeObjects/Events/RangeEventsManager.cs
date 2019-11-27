using System;
using CoreGame;

namespace RangeObjects
{
    public class RangeEventsManager : GameSingleton<RangeEventsManager>
    {
        private event Action<RangeObjectV2> OnRangeObjectAllCreatedEvent;

        public void RegisterOnRangeObjectCreatedEventListener(Action<RangeObjectV2> action)
        {
            this.OnRangeObjectAllCreatedEvent += action;
        }

        public void OnRangeObjectCreated(RangeObjectV2 RangeObjectV2)
        {
            if (this.OnRangeObjectAllCreatedEvent != null)
            {
                this.OnRangeObjectAllCreatedEvent.Invoke(RangeObjectV2);
            }
        }
    }
}