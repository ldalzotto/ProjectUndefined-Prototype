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

        public void PlayAnimationV2(int LayerID, IAnimationInput Animation, Action OnAnimationEnd = null, Func<float> InputWeightProvider = null)
        {
            this.AnimatorPlayableObject.PlayAnimation(LayerID, Animation, OnAnimationEnd, InputWeightProvider);
        }

        public void StopAnimationLayer(int animationLayer)
        {
            this.AnimatorPlayableObject.StopLayer(animationLayer);
        }


        public void DestroyAnimationLayerV2(int animationLayer)
        {
            this.AnimatorPlayableObject.DestroyLayer(animationLayer);
        }

        public void SetTwoDInputWeight(int layerID, Vector2 inputWeight)
        {
            this.AnimatorPlayableObject.SetTwoDInputWeight(layerID, inputWeight);
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