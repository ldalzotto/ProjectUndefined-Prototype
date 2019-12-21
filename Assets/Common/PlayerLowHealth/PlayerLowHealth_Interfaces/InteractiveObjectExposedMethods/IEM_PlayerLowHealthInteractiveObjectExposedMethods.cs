using System;

namespace ProjectileDeflection
{
    /// <summary>
    /// Methods exposed of CoreInteractiveObject for registering PlayerLowHealth start and end events.
    /// </summary>
    public interface IEM_PlayerLowHealthInteractiveObjectExposedMethods
    {
        void RegisterPlayerLowHealthStartedEvent(Action action);

        void UnRegisterPlayerLowHealthStartedEvent(Action action);

        void RegisterPlayerLowHealthEndedEvent(Action action);

        void UnRegisterPlayerLowHealthEndedEvent(Action action);
    }
}