using System;
using AnimatorPlayable;
using InteractiveObjectAction;

namespace PlayerAim
{
    /// <summary>
    /// Methods exposed of CoreInteractiveObject for registering <see cref="FiringPlayerAction"/> workflow related events.
    /// </summary>
    public interface IEM_IPlayerAimingFiringRegisteringEventsExposedMethod
    {
        void RegisterOnPlayerStartTargettingEvent(Action<PlayerAimingInteractiveObjectActionInherentData> action);

        void UnRegisterOnPlayerStartTargettingEvent(Action<PlayerAimingInteractiveObjectActionInherentData> action);
        
        void RegisterOnPlayerStoppedTargettingEvent(Action<PlayerAimingInteractiveObjectActionInherentData> action);

        void UnRegisterOnPlayerStoppedTargettingEvent(Action<PlayerAimingInteractiveObjectActionInherentData> action);
    }
}