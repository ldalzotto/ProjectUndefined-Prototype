using System;
using AnimatorPlayable;
using InteractiveObjects;
using PlayerActions;
using PlayerObject_Interfaces;
using UnityEngine;
using UnityEngine.Serialization;

namespace Firing
{
    /// <summary>
    /// Creates a <see cref="FiringPlayerAction"/> that will trigger the Player targetting logic.
    /// </summary>
    [Serializable]
    [SceneHandleDraw]
    public class FiringPlayerActionInherentData : PlayerActionInherentData
    {
        public GameObject FiringHorizontalPlanePrefab;

        [FormerlySerializedAs("DottedVisualFeebackPrefab")]
        public GameObject GroundConeVisualFeedbackPrefab;

        [Inline] public A_AnimationPlayableDefinition FiringPoseAnimationV2;

        public override string PlayerActionUniqueID
        {
            get { return FiringPlayerAction.FiringPlayerActionUniqueID; }
        }

        /// <param name="IPlayerActionInput">The input must be of type <see cref="FiringPlayerActionInput"/> to create the PlayerAction.</param>
        public override PlayerAction BuildPlayerAction(IPlayerActionInput IPlayerActionInput, Action OnPlayerActionStartedCallback = null, Action OnPlayerActionEndCallback = null)
        {
            if (IPlayerActionInput is FiringPlayerActionInput FiringPlayerActionInput)
            {
                return new FiringPlayerAction(ref FiringPlayerActionInput, OnPlayerActionStartedCallback, OnPlayerActionEndCallback);
            }

            return default;
        }

        public override IPlayerActionInput BuildInputFromInteractiveObject(CoreInteractiveObject AssociatedInteractiveObject)
        {
            return new FiringPlayerActionInput(this, AssociatedInteractiveObject);
        }
    }

    public struct FiringPlayerActionInput : IPlayerActionInput
    {
        public FiringPlayerActionInherentData FiringPlayerActionInherentData;
        public CoreInteractiveObject firingInteractiveObject;

        public FiringPlayerActionInput(FiringPlayerActionInherentData firingPlayerActionInherentData, CoreInteractiveObject firingInteractiveObject)
        {
            FiringPlayerActionInherentData = firingPlayerActionInherentData;
            this.firingInteractiveObject = firingInteractiveObject;
        }
    }
}