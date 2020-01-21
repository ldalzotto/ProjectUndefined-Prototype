using UnityEngine;
using PlayerDash;

namespace PlayerObject
{
    /// <summary>
    /// This system holds a reference to the <see cref="PlayerDashTargetPositionTrackerSystem.PlayerDashTargetWorldPosition"/>.
    /// This stored reference is needed because getting the value directly from <see cref="DashTeleportationDirectionAction"/> may lead to inacurrate result
    /// as the action may be destroyed before execution of <see cref="DashTeleportationAction"/>. This is prevention only.
    /// </summary>
    public class PlayerDashTargetPositionTrackerSystem
    {
        public Vector3? PlayerDashTargetWorldPosition;
    }
}