using System;
using AnimatorPlayable;
using InteractiveObjectAction;
using InteractiveObjects;

namespace ProjectileDeflection
{
    [Serializable]
    public class DeflectingProjectileInteractiveObjectActionInherentData : InteractiveObjectActionInherentData
    {
        [Inline(CreateAtSameLevelIfAbsent = true)]
        public A_AnimationPlayableDefinition ProjectileDeflectMovementAnimation;

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
                if (IEM_DeflectingProjectileAction_DataRetriever.ProjectileDeflectionEnabled())
                {
                    return new DeflectingProjectileInteractiveObjectActionInput(AssociatedInteractiveObject, IEM_DeflectingProjectileAction_DataRetriever.ProjectileDeflectionSystem, this);
                }
            }

            return null;
        }
    }

    public interface IEM_DeflectingProjectileAction_DataRetriever
    {
        bool ProjectileDeflectionEnabled();
        ProjectileDeflectionSystem ProjectileDeflectionSystem { get; }
    }
}