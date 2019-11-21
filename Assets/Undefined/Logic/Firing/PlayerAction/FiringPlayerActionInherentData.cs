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
        public SequencedAnimationInput FiringPoseAnimation;
        public override PlayerAction BuildPlayerAction(IPlayerInteractiveObject PlayerInteractiveObject)
        {
            return new FiringPlayerAction(this, PlayerInteractiveObject);
        }
    }
}