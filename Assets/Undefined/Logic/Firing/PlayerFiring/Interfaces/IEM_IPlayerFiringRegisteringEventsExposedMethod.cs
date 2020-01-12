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
        void RegisterOnPlayerStartTargettingEvent(Action<InteractiveObjectActionInherentData> action);

        void UnRegisterOnPlayerStartTargettingEvent(Action<InteractiveObjectActionInherentData> action);
        
        void RegisterOnPlayerStoppedTargettingEvent(Action<InteractiveObjectActionInherentData> action);

        void UnRegisterOnPlayerStoppedTargettingEvent(Action<InteractiveObjectActionInherentData> action);
    }
}