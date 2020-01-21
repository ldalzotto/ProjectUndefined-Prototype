using UnityEngine;
using System.Collections;
using System;

namespace PlayerDash
{
    public interface IEM_IPlayerDashDirectionActionRegisteringEventsExposedMethod
    {
        void RegisterOnPlayerDashDirectionActionStartedEvent(Action action);
        void UnRegisterOnPlayerDashDirectionActionStartedEvent(Action action);
        void RegisterOnPlayerDashDirectionActionEndedEvent(Action action);
        void UnRegisterOnPlayerDashDirectionActionEndedEvent(Action action);
        
        
    }
}