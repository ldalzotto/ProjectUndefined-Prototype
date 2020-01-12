using System;
using AnimatorPlayable;
using InteractiveObjectAction;
using InteractiveObjects;
using UnityEngine;
using UnityEngine.Serialization;

namespace PlayerAim
{
    /// <summary>
    /// Creates a <see cref="PlayerAimingInteractiveObjectAction"/> that will trigger the Player targetting logic.
    /// </summary>
    [Serializable]
    [SceneHandleDraw]
    public class PlayerAimingInteractiveObjectActionInherentData : InteractiveObjectActionInherentData
    {
        public GameObject FiringHorizontalPlanePrefab;

        [FormerlySerializedAs("DottedVisualFeebackPrefab")]
        public GameObject GroundConeVisualFeedbackPrefab;

        [Inline] public A_AnimationPlayableDefinition FiringPoseAnimationV2;

        public override string InteractiveObjectActionUniqueID
        {
            get { return PlayerAimingInteractiveObjectAction.PlayerAimingInteractiveObjectActionUniqueID; }
        }

        /// <param name="interactiveObjectActionInput">The input must be of type <see cref="FiringInteractiveObjectActionInput"/> to create the AInteractiveObjectAction.</param>
        public override InteractiveObjectAction.AInteractiveObjectAction BuildInteractiveObjectAction(IInteractiveObjectActionInput interactiveObjectActionInput)
        {
            if (interactiveObjectActionInput is FiringInteractiveObjectActionInput FiringPlayerActionInput)
            {
                return new PlayerAimingInteractiveObjectAction(ref FiringPlayerActionInput);
            }

            return default;
        }

        public override IInteractiveObjectActionInput BuildInputFromInteractiveObject(CoreInteractiveObject AssociatedInteractiveObject)
        {
            return new FiringInteractiveObjectActionInput(this, AssociatedInteractiveObject);
        }
    }

    public struct FiringInteractiveObjectActionInput : IInteractiveObjectActionInput
    {
        public PlayerAimingInteractiveObjectActionInherentData PlayerAimingInteractiveObjectActionInherentData;
        public CoreInteractiveObject firingInteractiveObject;

        public FiringInteractiveObjectActionInput(PlayerAimingInteractiveObjectActionInherentData playerAimingInteractiveObjectActionInherentData, CoreInteractiveObject firingInteractiveObject)
        {
            PlayerAimingInteractiveObjectActionInherentData = playerAimingInteractiveObjectActionInherentData;
            this.firingInteractiveObject = firingInteractiveObject;
        }
    }
}