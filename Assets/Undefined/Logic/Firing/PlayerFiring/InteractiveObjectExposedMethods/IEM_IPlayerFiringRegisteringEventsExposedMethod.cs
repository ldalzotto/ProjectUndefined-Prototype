using System;
using AnimatorPlayable;
using PlayerActions;

namespace Firing
{
    /// <summary>
    /// Methods exposed of CoreInteractiveObject for registering <see cref="FiringPlayerAction"/> workflow related events.
    /// </summary>
    public interface IEM_IPlayerFiringRegisteringEventsExposedMethod
    {
        void RegisterOnPlayerStartTargettingEvent(Action<PlayerActionInherentData> action);

        void UnRegisterOnPlayerStartTargettingEvent(Action<PlayerActionInherentData> action);
        
        void RegisterOnPlayerStoppedTargettingEvent(Action<PlayerActionInherentData> action);

        void UnRegisterOnPlayerStoppedTargettingEvent(Action<PlayerActionInherentData> action);
    }
}