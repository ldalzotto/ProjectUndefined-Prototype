using System;
using AnimatorPlayable;
using InteractiveObjectAction;

namespace Firing
{
    /// <summary>
    /// Methods exposed of CoreInteractiveObject for registering <see cref="FiringPlayerAction"/> workflow related events.
    /// </summary>
    public interface IEM_IPlayerFiringRegisteringEventsExposedMethod
    {
        void RegisterOnPlayerStartTargettingEvent(Action<FiringInteractiveObjectActionInherentData> action);

        void UnRegisterOnPlayerStartTargettingEvent(Action<FiringInteractiveObjectActionInherentData> action);
        
        void RegisterOnPlayerStoppedTargettingEvent(Action<FiringInteractiveObjectActionInherentData> action);

        void UnRegisterOnPlayerStoppedTargettingEvent(Action<FiringInteractiveObjectActionInherentData> action);
    }
}