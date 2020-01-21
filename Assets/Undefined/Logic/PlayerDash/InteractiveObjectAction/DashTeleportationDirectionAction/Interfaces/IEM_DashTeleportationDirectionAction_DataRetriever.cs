
namespace PlayerDash
{
    using UnityEngine;

    /// <summary>
    /// In order for the <see cref="DashTeleportationAction"/> to be executable, the <see cref="CoreInteractiveObject"/> must implements this interface.
    /// It provides the target world position where the associated interactive object must be warped to.
    /// </summary>
    public interface IEM_DashTeleportationDirectionAction_DataRetriever
    {
        Vector3? GetTargetWorldPosition();
    }
}