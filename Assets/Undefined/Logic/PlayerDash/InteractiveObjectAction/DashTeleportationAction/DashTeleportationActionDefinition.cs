using System;
using InteractiveObjectAction;
using InteractiveObjects;
using UnityEngine;

namespace PlayerDash
{
    [Serializable]
    public class DashTeleportationActionDefinition : InteractiveObjectActionInherentData
    {
        public override string InteractiveObjectActionUniqueID
        {
            get { return DashTeleportationAction.DashTeleportationActionUniqueID; }
        }

        public override AInteractiveObjectAction BuildInteractiveObjectAction(IInteractiveObjectActionInput interactiveObjectActionInput)
        {
            if (interactiveObjectActionInput is DashTeleportationActionDefinitionInput DashTeleportationActionDefinitionInput)
            {
                return new DashTeleportationAction(this.coreInteractiveObjectActionDefinition, DashTeleportationActionDefinitionInput);
            }

            return null;
        }

        public override IInteractiveObjectActionInput BuildInputFromInteractiveObject(CoreInteractiveObject AssociatedInteractiveObject)
        {
            if (AssociatedInteractiveObject is IEM_DashTeleportationAction IEM_DashTeleportationAction)
            {
                if (IEM_DashTeleportationAction.TryingToExecuteDashTeleportationAction())
                {
                    if (AssociatedInteractiveObject is IEM_DashTeleportationDirectionAction_DataRetriever IEM_DashTeleportationDirectionAction_DataRetriever)
                    {
                        return new DashTeleportationActionDefinitionInput(AssociatedInteractiveObject,
                            IEM_DashTeleportationDirectionAction_DataRetriever.GetTargetWorldPosition());
                    }
                }
            }


            return null;
        }
    }

    public struct DashTeleportationActionDefinitionInput : IInteractiveObjectActionInput
    {
        public CoreInteractiveObject AssociatedInteractiveObject;
        public Vector3 TargetWorldPoint;

        public DashTeleportationActionDefinitionInput(CoreInteractiveObject associatedInteractiveObject, Vector3 targetWorldPoint)
        {
            AssociatedInteractiveObject = associatedInteractiveObject;
            this.TargetWorldPoint = targetWorldPoint;
        }
    }

    public interface IEM_DashTeleportationAction
    {
        bool TryingToExecuteDashTeleportationAction();
    }
}