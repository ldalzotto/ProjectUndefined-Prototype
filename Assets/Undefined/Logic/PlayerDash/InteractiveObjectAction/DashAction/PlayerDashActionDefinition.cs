using System;
using InteractiveObjectAction;
using InteractiveObjects;
using UnityEngine;


namespace PlayerDash
{
    [Serializable]
    public class PlayerDashActionDefinition : InteractiveObjectActionInherentData
    {
        public override string InteractiveObjectActionUniqueID
        {
            get { return PlayerDashAction.DashTeleportationActionUniqueID; }
        }

        public override AInteractiveObjectAction BuildInteractiveObjectAction(IInteractiveObjectActionInput interactiveObjectActionInput)
        {
            if (interactiveObjectActionInput is DashTeleportationActionDefinitionInput DashTeleportationActionDefinitionInput)
            {
                return new PlayerDashAction(this.coreInteractiveObjectActionDefinition, DashTeleportationActionDefinitionInput);
            }

            return null;
        }

        /// <summary>
        /// To build the <see cref="DashTeleportationActionDefinitionInput"/>, the <see cref="CoreInteractiveObject"/> must implement : 
        ///     - <see cref="IEM_DashTeleportationAction"/> that will call the <see cref="IEM_DashTeleportationAction.TryingToExecuteDashTeleportationAction"/> events.
        ///     - <see cref="IEM_DashTeleportationDirectionAction_DataRetriever"/> that will retrieve the target world position from the <see cref="DashTeleportationDirectionAction"/>.
        /// </summary>
        public override IInteractiveObjectActionInput BuildInputFromInteractiveObject(CoreInteractiveObject AssociatedInteractiveObject)
        {
            if (AssociatedInteractiveObject is IEM_PlayerDashAction IEM_DashTeleportationAction)
            {
                if (IEM_DashTeleportationAction.TryingToExecuteDashTeleportationAction())
                {
                    if (AssociatedInteractiveObject is IEM_DashTeleportationDirectionAction_DataRetriever IEM_DashTeleportationDirectionAction_DataRetriever)
                    {
                        var targetWorldPosition = IEM_DashTeleportationDirectionAction_DataRetriever.GetTargetWorldPosition();

                        /// A targetWorldPosition has ne value when the calculation has not been successful this frame.
                        if (targetWorldPosition.HasValue)
                        {
                            return new DashTeleportationActionDefinitionInput(AssociatedInteractiveObject, targetWorldPosition.Value);
                        }
                    }
                }
            }

            return null;
        }
    }

    public struct DashTeleportationActionDefinitionInput : IInteractiveObjectActionInput
    {
        public CoreInteractiveObject AssociatedInteractiveObject;

        /// <summary>
        /// The <see cref="TargetWorldPoint"> is the world position point where the <see cref="AssociatedInteractiveObject"/> will be
        /// teleported.
        /// </summary>
        public Vector3 TargetWorldPoint;

        public DashTeleportationActionDefinitionInput(CoreInteractiveObject associatedInteractiveObject, Vector3 targetWorldPoint)
        {
            AssociatedInteractiveObject = associatedInteractiveObject;
            this.TargetWorldPoint = targetWorldPoint;
        }
    }

}