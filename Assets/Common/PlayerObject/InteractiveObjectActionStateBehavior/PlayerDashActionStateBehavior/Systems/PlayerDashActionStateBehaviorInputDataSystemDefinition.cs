using System;
using OdinSerializer;
using PlayerDash;

namespace PlayerObject
{
    [SceneHandleDraw]
    [Serializable]
    public class PlayerDashActionStateBehaviorInputDataSystemDefinition : SerializedScriptableObject
    {
        /// <summary>
        /// The definition of the <see cref="DashTeleportationDirectionAction"/> that will be executed in <see cref="PlayerDashDirectionActionStateManager"/>.
        /// </summary>
        [DrawNested]
        public PlayerDashTeleportationDirectionActionDefinition DashTeleportationDirectionActionDefinition;

        public PlayerDashActionStateBehaviorInputDataSystemDefinition(PlayerDashTeleportationDirectionActionDefinition dashTeleportationDirectionActionDefinition)
        {
            DashTeleportationDirectionActionDefinition = dashTeleportationDirectionActionDefinition;
        }
    }
}