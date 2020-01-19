using System;
using AnimatorPlayable;
using Input;
using InteractiveObjectAction;
using InteractiveObjects;
using ParticleObjects;
using PlayerObject_Interfaces;
using UnityEngine.Serialization;

namespace ProjectileDeflection
{
    /// <summary>
    /// In order to work properly, the associated <see cref="CoreInteractiveObject"/> must implements :
    ///  * <see cref="IEM_DeflectingProjectileAction_DataRetriever"/> to ensure that execution is allowed and getting a reference to <see cref="ProjectileDeflectionTrackingInteractiveObjectAction"/>.
    /// </summary>
    [Serializable]
    public class DeflectingProjectileInteractiveObjectActionInherentData : InteractiveObjectActionInherentData
    {
        [Inline(CreateAtSameLevelIfAbsent = true)]
        public A_AnimationPlayableDefinition ProjectileDeflectMovementAnimation;

        [FormerlySerializedAs("ParticleObjectDefinition")] [Inline(CreateAtSameLevelIfAbsent = true)]
        public ParticleObjectDefinition OnDeflectionParticles;

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

        /// <summary>
        /// This will usually be called from the SkillStytem.
        /// This is why we check that the projectile deflection is enabled <see cref="IEM_DeflectingProjectileAction_DataRetriever.ProjectileDeflectionEnabled"/>.  
        /// </summary>
        public override IInteractiveObjectActionInput BuildInputFromInteractiveObject(CoreInteractiveObject AssociatedInteractiveObject)
        {
            if (AssociatedInteractiveObject is IEM_DeflectingProjectileAction_DataRetriever IEM_DeflectingProjectileAction_DataRetriever)
            {
                if (IEM_DeflectingProjectileAction_DataRetriever.ProjectileDeflectionEnabled())
                {
                    return new DeflectingProjectileInteractiveObjectActionInput(AssociatedInteractiveObject, IEM_DeflectingProjectileAction_DataRetriever.GetPlayingProjectileDeflectionSystem(), this);
                }
            }

            return null;
        }
    }

    public interface ISkilkSlotLowHealthNotification
    {
        void OnLowHealthStarted(CoreInteractiveObject AssociatedInteractiveObject);
    }

    public interface IEM_DeflectingProjectileAction_DataRetriever
    {
        bool ProjectileDeflectionEnabled();
        ProjectileDeflectionTrackingInteractiveObjectAction GetPlayingProjectileDeflectionSystem();
    }
}