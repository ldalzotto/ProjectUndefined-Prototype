using System.Collections.Generic;
using AnimatorPlayable;
using InteractiveObject_Animation;

namespace SoldierAnimation
{
    public enum SoldierTargettingUpperBodyState
    {
        LISTENING,
        TARGETTING
    }

    public class SoldierTargettingUpperBodyStateBehavior : StateBehavior<SoldierTargettingUpperBodyState, SoldierTargettingUpperBodyStateManager>
    {
        public SoldierTargettingUpperBodyStateBehavior(AnimationController AnimationControllerRef, A_AnimationPlayableDefinition TargettingAnimation)
        {
            this.StateManagersLookup = new Dictionary<SoldierTargettingUpperBodyState, SoldierTargettingUpperBodyStateManager>()
            {
                {SoldierTargettingUpperBodyState.LISTENING, new SoldierTargettingUpperBodyListeningStateManager()},
                {SoldierTargettingUpperBodyState.TARGETTING, new SoldierTargettingUpperBodyTargettingStateManager(AnimationControllerRef, TargettingAnimation)}
            };
            base.Init(SoldierTargettingUpperBodyState.LISTENING);
        }

        public void StartTargetting()
        {
            this.SetState(SoldierTargettingUpperBodyState.TARGETTING);
        }

        public void StopTargetting()
        {
            if (this.GetCurrentState() == SoldierTargettingUpperBodyState.TARGETTING)
            {
                this.SetState(SoldierTargettingUpperBodyState.LISTENING);
            }
        }
    }

    public abstract class SoldierTargettingUpperBodyStateManager : StateManager
    {
    }

    class SoldierTargettingUpperBodyListeningStateManager : SoldierTargettingUpperBodyStateManager
    {
    }

    class SoldierTargettingUpperBodyTargettingStateManager : SoldierTargettingUpperBodyStateManager
    {
        private AnimationController AnimationControllerRef;

        private A_AnimationPlayableDefinition TargettingAnimation;
        public SoldierTargettingUpperBodyTargettingStateManager(AnimationController animationControllerRef, A_AnimationPlayableDefinition TargettingAnimation)
        {
            AnimationControllerRef = animationControllerRef;
            this.TargettingAnimation = TargettingAnimation;
        }

        public override void OnStateEnter()
        {
            this.AnimationControllerRef.PlayAnimationV2(SoliderEnemyAnimationLayersOrders.GetLayerNumber(SoliderEnemyAnimationLayers.TARGETTING_UPPER_BODY_POSE), this.TargettingAnimation.GetAnimationInput());
        }

        public override void OnStateExit()
        {
            this.AnimationControllerRef.StopAnimationLayer(SoliderEnemyAnimationLayersOrders.GetLayerNumber(SoliderEnemyAnimationLayers.TARGETTING_UPPER_BODY_POSE));
        }
    }
}