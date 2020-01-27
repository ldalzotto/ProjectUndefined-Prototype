using System.Collections.Generic;
using AnimatorPlayable;
using DefaultNamespace;
using InteractiveObject_Animation;
using UnityEngine;

namespace SoldierAnimation
{
    public enum SoldierObjectLocomotionState
    {
        LISTENING,
        MOVING
    }

    public class SoldierObjectLocomotionStateBehavior : StateBehavior<SoldierObjectLocomotionState, SoldierObjectLocomotionStateManager>
    {
        public SoldierObjectLocomotionStateBehavior(AnimationController AnimationControllerRef, A_AnimationPlayableDefinition BaseLocomotionTree)
        {
            this.StateManagersLookup = new Dictionary<SoldierObjectLocomotionState, SoldierObjectLocomotionStateManager>()
            {
                {SoldierObjectLocomotionState.LISTENING, new SoldierObjectLocomotionListeningStateManager()},
                {SoldierObjectLocomotionState.MOVING, new SoldierObjectLocomotionMovingStateManager(AnimationControllerRef, BaseLocomotionTree)}
            };
            base.Init(SoldierObjectLocomotionState.LISTENING);
            this.SetState(SoldierObjectLocomotionState.MOVING);
        }

        public void SetUnscaledObjectLocalDirection(Vector3 localDirection)
        {
            this.GetCurrentStateManager().SetUnscaledObjectLocalDirection(localDirection);
        }
    }

    public abstract class SoldierObjectLocomotionStateManager : StateManager
    {
        public virtual void SetUnscaledObjectLocalDirection(Vector3 localDirection)
        {
        }
    }

    class SoldierObjectLocomotionListeningStateManager : SoldierObjectLocomotionStateManager
    {
    }

    class SoldierObjectLocomotionMovingStateManager : SoldierObjectLocomotionStateManager
    {
        private LocomotionAnimationStateSystem LocomotionAnimationStateSystem;
        private A_AnimationPlayableDefinition LocomotionTree;

        public SoldierObjectLocomotionMovingStateManager(AnimationController AnimationControllerRef, A_AnimationPlayableDefinition LocomotionTree)
        {
            this.LocomotionTree = LocomotionTree;
            this.LocomotionAnimationStateSystem = new LocomotionAnimationStateSystem(SoliderEnemyAnimationLayersOrders.GetLayerNumber(SoliderEnemyAnimationLayers.LOCOMOTION), AnimationControllerRef);
        }

        public override void OnStateEnter()
        {
            this.LocomotionAnimationStateSystem.PlayAnimation(this.LocomotionTree);
        }

        public override void OnStateExit()
        {
            this.LocomotionAnimationStateSystem.KillAnimation();
        }

        public override void SetUnscaledObjectLocalDirection(Vector3 localDirection)
        {
            this.LocomotionAnimationStateSystem.SetUnscaledObjectLocalDirection(localDirection);
        }
    }
}