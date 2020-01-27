using System.Collections.Generic;
using AnimatorPlayable;
using InteractiveObject_Animation;
using UnityEngine;

namespace SoldierAnimation
{
    public class SoliderEnemyAnimationStateManager
    {
        private SoldierObjectLocomotionStateBehavior SoldierObjectLocomotionStateBehavior;
        private SoldierTargettingUpperBodyStateBehavior SoldierTargettingUpperBodyStateBehavior;

        public SoliderEnemyAnimationStateManager(AnimationController AnimationControllerRef, A_AnimationPlayableDefinition BaseLocomotionTree, SoldierAnimationSystemDefinition SoldierAnimationSystemDefinition)
        {
            this.SoldierObjectLocomotionStateBehavior = new SoldierObjectLocomotionStateBehavior(AnimationControllerRef, BaseLocomotionTree);
            this.SoldierTargettingUpperBodyStateBehavior = new SoldierTargettingUpperBodyStateBehavior(AnimationControllerRef, SoldierAnimationSystemDefinition.FiringPoseAnimation);
        }

        public void Tick(float d)
        {
            this.SoldierObjectLocomotionStateBehavior.Tick(d);
            this.SoldierTargettingUpperBodyStateBehavior.Tick(d);
        }

        public void SetUnscaledObjectLocalDirection(Vector3 localDirection)
        {
            this.SoldierObjectLocomotionStateBehavior.SetUnscaledObjectLocalDirection(localDirection);
        }

        public void StartTargetting()
        {
            this.SoldierTargettingUpperBodyStateBehavior.StartTargetting();
        }

        public void StopTargetting()
        {
            this.SoldierTargettingUpperBodyStateBehavior.StopTargetting();
        }
    }

    public enum SoliderEnemyAnimationLayers
    {
        LOCOMOTION,
        TARGETTING_UPPER_BODY_POSE
    }

    public static class SoliderEnemyAnimationLayersOrders
    {
        public static int GetLayerNumber(SoliderEnemyAnimationLayers PlayerObjectAnimationLayers)
        {
            return PlayerObjectAnimationLayersOrder[PlayerObjectAnimationLayers];
        }

        static Dictionary<SoliderEnemyAnimationLayers, int> PlayerObjectAnimationLayersOrder = new Dictionary<SoliderEnemyAnimationLayers, int>()
        {
            {SoliderEnemyAnimationLayers.LOCOMOTION, 0},
            {SoliderEnemyAnimationLayers.TARGETTING_UPPER_BODY_POSE, 1}
        };
    }
}