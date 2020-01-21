
using InteractiveObjects;

namespace PlayerDash
{
    /// <summary>
    /// This interface must be implemented by a <see cref="CoreInteractiveObject">.
    /// </summary>
    public interface IEM_DashTeleportationAction
    {
        /// <summary>
        /// Notify that the <see cref="DashTeleportationAction"/> is trying to get executed.
        /// Also, returns a flag that indicates if the associated <see cref="CoreInteractiveObject"> is elligible to execute the <see cref="DashTeleportationAction"/>.
        /// </summary>
        bool TryingToExecuteDashTeleportationAction();
    }
}
