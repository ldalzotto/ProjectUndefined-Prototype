using System.Collections;
using System.Collections.Generic;
using AnimatorPlayable;
using InteractiveObject_Animation;
using UnityEngine;

namespace SoldierAnimation
{
    public class SoldierAnimationSystem
    {
        private SoldierAnimationSystemDefinition SoldierAnimationSystemDefinition;
        private AnimationController AnimationController;

        public SoldierAnimationSystem(SoldierAnimationSystemDefinition SoldierAnimationSystemDefinition, AnimationController AnimationController)
        {
            this.SoldierAnimationSystemDefinition = SoldierAnimationSystemDefinition;
            this.AnimationController = AnimationController;
        }

        public void OnShootingAtPlayerStart()
        {
            this.AnimationController.PlayLocomotionAnimationOverride(this.SoldierAnimationSystemDefinition.FiringPoseAnimation.GetAnimationInput(), AnimationLayerID.LocomotionLayer_1);
        }

        public  void OnShootingAtPlayerEnd()
        {
            this.AnimationController.DestroyAnimationLayer(AnimationLayerID.LocomotionLayer_1);
        }
    }
}