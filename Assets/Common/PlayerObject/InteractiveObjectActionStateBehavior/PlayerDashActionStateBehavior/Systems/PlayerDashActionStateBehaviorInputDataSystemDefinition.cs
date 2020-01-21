using System;
using OdinSerializer;
using PlayerDash;

namespace PlayerObject
{
    [Serializable]
    public class PlayerDashActionStateBehaviorInputDataSystemDefinition : SerializedScriptableObject
    {
        /// <summary>
        /// The definition of the <see cref="DashTeleportationDirectionAction"/> that will be executed in <see cref="PlayerDashDirectionActionStateManager"/>.
        /// </summary>
        public DashTeleportationDirectionActionDefinition DashTeleportationDirectionActionDefinition;

        public PlayerDashActionStateBehaviorInputDataSystemDefinition(DashTeleportationDirectionActionDefinition dashTeleportationDirectionActionDefinition)
        {
            DashTeleportationDirectionActionDefinition = dashTeleportationDirectionActionDefinition;
        }
    }
}