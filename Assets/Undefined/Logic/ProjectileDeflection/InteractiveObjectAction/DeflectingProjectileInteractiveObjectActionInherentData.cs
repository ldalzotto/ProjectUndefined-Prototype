using System;
using InteractiveObjectAction;
using InteractiveObjects;

namespace ProjectileDeflection
{
    [Serializable]
    public class DeflectingProjectileInteractiveObjectActionInherentData : InteractiveObjectActionInherentData
    {
        public override string InteractiveObjectActionUniqueID
        {
            get { return DeflectingProjectileInteractiveObjectAction.DeflectingProjectileInteractiveObjectActionUniqueID; }
        }

        public override AInteractiveObjectAction BuildInteractiveObjectAction(IInteractiveObjectActionInput interactiveObjectActionInput)
        {
            if (interactiveObjectActionInput is DeflectingProjectileInteractiveObjectActionInput DeflectingProjectileInteractiveObjectActionInput)
            {
                return new DeflectingProjectileInteractiveObjectAction(DeflectingProjectileInteractiveObjectActionInput, this.coreInteractiveObjectActionDefinition);
            }

            return null;
        }

        public override IInteractiveObjectActionInput BuildInputFromInteractiveObject(CoreInteractiveObject AssociatedInteractiveObject)
        {
            if (AssociatedInteractiveObject is IEM_DeflectingProjectileAction_DataRetriever IEM_DeflectingProjectileAction_DataRetriever)
            {
                return new DeflectingProjectileInteractiveObjectActionInput(AssociatedInteractiveObject, IEM_DeflectingProjectileAction_DataRetriever.ProjectileDeflectionSystem);
            }

            return null;
        }
    }

    public interface IEM_DeflectingProjectileAction_DataRetriever
    {
        ProjectileDeflectionSystem ProjectileDeflectionSystem { get; }
    }
}