using System;
using AnimatorPlayable;
using PlayerActions;
using PlayerObject_Interfaces;
using UnityEngine;

namespace Firing
{
    [Serializable]
    [SceneHandleDraw]
    public class FiringPlayerActionInherentData : PlayerActionInherentData
    {
        public GameObject FiringHorizontalPlanePrefab;
        public GameObject DottedVisualFeebackPrefab;
        [Inline]
        public A_AnimationPlayableDefinition FiringPoseAnimationV2;
        public override PlayerAction BuildPlayerAction(IPlayerInteractiveObject PlayerInteractiveObject, Action OnPlayerActionStartedCallback = null, Action OnPlayerActionEndCallback = null)
        {
            return new FiringPlayerAction(this, PlayerInteractiveObject, OnPlayerActionStartedCallback, OnPlayerActionEndCallback);
        }
    }
}