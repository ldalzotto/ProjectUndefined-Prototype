using System;
using AnimatorPlayable;
using UnityEngine;
using UnityEngine.AI;

namespace InteractiveObject_Animation
{
    public class AnimationController
    {
        private NavMeshAgent Agent;
        private AnimatorPlayableObject AnimatorPlayableObject;
        private Rigidbody Rigidbody;

        private BoolVariable RootMotionEnabled;


        public AnimationController(NavMeshAgent agent, AnimatorPlayableObject animatorPlayableObject, Rigidbody rigidbody,
            Action OnRootMotionEnabled = null, Action OnRootMotionDisabled = null)
        {
            Agent = agent;
            AnimatorPlayableObject = animatorPlayableObject;
            Rigidbody = rigidbody;

            this.RootMotionEnabled = new BoolVariable(false,
                () =>
                {
                    this.OnRootMotionEnabled();
                    if (OnRootMotionEnabled != null)
                    {
                        OnRootMotionEnabled.Invoke();
                    }
                },
                () =>
                {
                    this.OnRootMotionDisabled();
                    if (OnRootMotionDisabled != null)
                    {
                        OnRootMotionDisabled.Invoke();
                    }
                });
        }

        public void Tick(float d)
        {
            if (this.RootMotionEnabled.GetValue())
            {
                this.Agent.nextPosition = this.AnimatorPlayableObject.Animator.transform.position;
            }
        }

        public void PlayAnimationV2(int LayerID, IAnimationInput Animation, Action OnAnimationEnd = null, Func<float> InputWeightProvider = null, Func<Vector2> TwoDInputWheigtProvider = null)
        {
            this.AnimatorPlayableObject.PlayAnimation(LayerID, Animation, OnAnimationEnd, InputWeightProvider, TwoDInputWheigtProvider);
        }

        /// <summary>
        /// Plays an <see cref="IAnimationInput"/> to the layer <see cref="AnimationLayerID.LocomotionLayer"/>
        /// </summary>
        /// <param name="LocomotionAnimation">The played animation</param>
        public void PlayLocomotionAnimation(IAnimationInput LocomotionAnimation, Func<float> InputWeightProvider = null, Func<Vector2> TwoDInputWheigtProvider = null)
        {
            this.AnimatorPlayableObject.PlayAnimation(AnimationLayerStatic.AnimationLayers[AnimationLayerID.LocomotionLayer].ID, LocomotionAnimation,
                InputWeightProvider: InputWeightProvider, TwoDInputWheigtProvider: TwoDInputWheigtProvider);
        }

        /// <summary>
        /// Plays an <see cref="IAnimationInput"/> to a layer after <see cref="AnimationLayerID.LocomotionLayer"/>.
        /// </summary>
        public void PlayLocomotionAnimationOverride(IAnimationInput LocomotionAnimation, AnimationLayerID overrideLayer,
            Func<float> InputWeightProvider = null, Func<Vector2> TwoDInputWheigtProvider = null)
        {
            this.AnimatorPlayableObject.PlayAnimation(AnimationLayerStatic.AnimationLayers[overrideLayer].ID, LocomotionAnimation,
                InputWeightProvider: InputWeightProvider, TwoDInputWheigtProvider: TwoDInputWheigtProvider);
        }

        /// <summary>
        /// Plays an <see cref="SequencedAnimationInput"/> to the Layer <see cref="AnimationLayerID.ContextActionLayer"/>
        /// </summary>
        /// <param name="ContextActionAnimation">The played animation</param>
        public void PlayContextAction(IAnimationInput ContextActionAnimation, bool rootMotion, Action OnAnimationFinished = null)
        {
            this.RootMotionEnabled.SetValue(rootMotion);
            this.AnimatorPlayableObject.PlayAnimation(AnimationLayerStatic.AnimationLayers[AnimationLayerID.ContextActionLayer].ID, ContextActionAnimation, () => { this.OnAnimationFinished(OnAnimationFinished); });
        }

        public void KillContextAction(SequencedAnimationInput ContextActionAnimation)
        {
            this.AnimatorPlayableObject.DestroyLayer(AnimationLayerStatic.AnimationLayers[AnimationLayerID.ContextActionLayer].ID);
        }

        public void DestroyAnimationLayer(AnimationLayerID animationLayer)
        {
            this.AnimatorPlayableObject.DestroyLayer(AnimationLayerStatic.AnimationLayers[animationLayer].ID);
        }
        public void DestroyAnimationLayerV2(int animationLayer)
        {
            this.AnimatorPlayableObject.DestroyLayer(animationLayer);
        }
        
        private void OnAnimationFinished(Action parentCallback)
        {
            this.RootMotionEnabled.SetValue(false);
            parentCallback?.Invoke();
        }

        private void OnRootMotionEnabled()
        {
            this.AnimatorPlayableObject.Animator.applyRootMotion = true;
        }

        private void OnRootMotionDisabled()
        {
            this.AnimatorPlayableObject.Animator.applyRootMotion = false;
        }
    }
}
