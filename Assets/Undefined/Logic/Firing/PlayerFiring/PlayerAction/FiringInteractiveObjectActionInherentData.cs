﻿using System;
using AnimatorPlayable;
using InteractiveObjects;
using PlayerActions;
using PlayerObject_Interfaces;
using UnityEngine;
using UnityEngine.Serialization;

namespace Firing
{
    /// <summary>
    /// Creates a <see cref="FiringInteractiveObjectAction"/> that will trigger the Player targetting logic.
    /// </summary>
    [Serializable]
    [SceneHandleDraw]
    public class FiringInteractiveObjectActionInherentData : InteractiveObjectActionInherentData
    {
        public GameObject FiringHorizontalPlanePrefab;

        [FormerlySerializedAs("DottedVisualFeebackPrefab")]
        public GameObject GroundConeVisualFeedbackPrefab;

        [Inline] public A_AnimationPlayableDefinition FiringPoseAnimationV2;

        public override string InteractiveObjectActionUniqueID
        {
            get { return FiringInteractiveObjectAction.FiringPlayerActionUniqueID; }
        }

        /// <param name="interactiveObjectActionInput">The input must be of type <see cref="FiringInteractiveObjectActionInput"/> to create the InteractiveObjectAction.</param>
        public override InteractiveObjectAction BuildInteractiveObjectAction(IInteractiveObjectActionInput interactiveObjectActionInput, Action OnInteractiveObjectActionStartedCallback = null, Action OnInteractiveObjectActionEndCallback = null)
        {
            if (interactiveObjectActionInput is FiringInteractiveObjectActionInput FiringPlayerActionInput)
            {
                return new FiringInteractiveObjectAction(ref FiringPlayerActionInput, OnInteractiveObjectActionStartedCallback, OnInteractiveObjectActionEndCallback);
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
        public FiringInteractiveObjectActionInherentData FiringInteractiveObjectActionInherentData;
        public CoreInteractiveObject firingInteractiveObject;

        public FiringInteractiveObjectActionInput(FiringInteractiveObjectActionInherentData firingInteractiveObjectActionInherentData, CoreInteractiveObject firingInteractiveObject)
        {
            FiringInteractiveObjectActionInherentData = firingInteractiveObjectActionInherentData;
            this.firingInteractiveObject = firingInteractiveObject;
        }
    }
}