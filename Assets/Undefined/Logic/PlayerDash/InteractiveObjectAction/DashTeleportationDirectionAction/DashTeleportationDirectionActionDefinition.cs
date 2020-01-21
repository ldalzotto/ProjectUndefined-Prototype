using System;
using InteractiveObjectAction;
using InteractiveObjects;
using OdinSerializer;

namespace PlayerDash
{
    public struct DashTeleportationDirectionActionDefinitionInput : IInteractiveObjectActionInput
    {
        public CoreInteractiveObject AssociatedInteractiveObject;

        public DashTeleportationDirectionActionDefinitionInput(CoreInteractiveObject associatedInteractiveObject)
        {
            AssociatedInteractiveObject = associatedInteractiveObject;
        }
    }

    [Serializable]
    public class DashTeleportationDirectionActionDefinition : InteractiveObjectActionInherentData
    {
        public override string InteractiveObjectActionUniqueID
        {
            get { return DashTeleportationDirectionAction.DashTeleportationDirectionActionUniqueID; }
        }

        /// <summary>
        /// The initial max distance from which the player is allowed to dash.
        /// </summary>
        public float MaxDashDistance;

        public override AInteractiveObjectAction BuildInteractiveObjectAction(IInteractiveObjectActionInput interactiveObjectActionInput)
        {
            if (interactiveObjectActionInput is DashTeleportationDirectionActionDefinitionInput DashTeleportationDirectionActionDefinitionInput)
            {
                return new DashTeleportationDirectionAction(DashTeleportationDirectionActionDefinitionInput.AssociatedInteractiveObject,
                    this, this.coreInteractiveObjectActionDefinition);
            }

            return null;
        }

        public override IInteractiveObjectActionInput BuildInputFromInteractiveObject(CoreInteractiveObject AssociatedInteractiveObject)
        {
            return new DashTeleportationDirectionActionDefinitionInput(AssociatedInteractiveObject);
        }
    }
}