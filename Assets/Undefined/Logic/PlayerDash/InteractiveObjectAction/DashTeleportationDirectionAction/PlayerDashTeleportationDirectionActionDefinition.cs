using System;
using InteractiveObjectAction;
using InteractiveObjects;
using OdinSerializer;

namespace PlayerDash
{
    public struct PlayerDashDirectionActionDefinitionInput : IInteractiveObjectActionInput
    {
        public CoreInteractiveObject AssociatedInteractiveObject;

        public PlayerDashDirectionActionDefinitionInput(CoreInteractiveObject associatedInteractiveObject)
        {
            AssociatedInteractiveObject = associatedInteractiveObject;
        }
    }

    [SceneHandleDraw]
    [Serializable]
    public class PlayerDashTeleportationDirectionActionDefinition : InteractiveObjectActionInherentData
    {
        public override string InteractiveObjectActionUniqueID
        {
            get { return PlayerDashDirectionAction.DashTeleportationDirectionActionUniqueID; }
        }

        /// <summary>
        /// The initial max distance from which the player is allowed to dash.
        /// </summary>
        [WireCircle()]
        public float MaxDashDistance;

        public override AInteractiveObjectAction BuildInteractiveObjectAction(IInteractiveObjectActionInput interactiveObjectActionInput)
        {
            if (interactiveObjectActionInput is PlayerDashDirectionActionDefinitionInput DashTeleportationDirectionActionDefinitionInput)
            {
                return new PlayerDashDirectionAction(DashTeleportationDirectionActionDefinitionInput.AssociatedInteractiveObject,
                    this, this.coreInteractiveObjectActionDefinition);
            }

            return null;
        }

        public override IInteractiveObjectActionInput BuildInputFromInteractiveObject(CoreInteractiveObject AssociatedInteractiveObject)
        {
            return new PlayerDashDirectionActionDefinitionInput(AssociatedInteractiveObject);
        }
    }
}