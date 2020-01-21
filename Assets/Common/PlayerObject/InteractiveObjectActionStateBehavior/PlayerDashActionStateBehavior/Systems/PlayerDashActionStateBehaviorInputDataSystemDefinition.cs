using System;
using OdinSerializer;
using PlayerDash;

namespace PlayerObject
{
    [Serializable]
    public class PlayerDashActionStateBehaviorInputDataSystemDefinition : SerializedScriptableObject
    {
        public DashTeleportationDirectionActionDefinition DashTeleportationDirectionActionDefinition;

        public PlayerDashActionStateBehaviorInputDataSystemDefinition(DashTeleportationDirectionActionDefinition dashTeleportationDirectionActionDefinition)
        {
            DashTeleportationDirectionActionDefinition = dashTeleportationDirectionActionDefinition;
        }
    }
}