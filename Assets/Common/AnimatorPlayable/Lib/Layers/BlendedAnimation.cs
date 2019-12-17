using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AnimatorPlayable
{
    public class BlendedAnimationLayer : MyAnimationLayer
    {
        public List<BlendedAnimationClip> BlendedAnimationClips;
        private BlendedAnimationInput BlendedAnimationInput;
        public AnimationMixerPlayable AnimationMixerPlayable { get; private set; }
        private Func<float> inputWeightProvider;

        public BlendedAnimationLayer(PlayableGraph PlayableGraph, AnimationLayerMixerPlayable parentAnimationLayerMixerPlayable,
            int layerId, BlendedAnimationInput BlendedAnimationInput, Func<float> InputWeightProvider) : base(layerId, parentAnimationLayerMixerPlayable)
        {
            this.BlendedAnimationInput = BlendedAnimationInput;
            this.BlendedAnimationClips = BlendedAnimationInput.BlendedAnimationClips.ConvertAll(i => i.ToBlendedAnimationClip());
     
            //create a playable mixer
            this.AnimationMixerPlayable = AnimationMixerPlayable.Create(PlayableGraph);

            foreach (var blendedAnimationClip in  this.BlendedAnimationClips)
            {
                var animationClipPlayable = AnimationClipPlayable.Create(PlayableGraph, blendedAnimationClip.AnimationClip);
                animationClipPlayable.SetApplyFootIK(false);
                animationClipPlayable.SetApplyPlayableIK(false);
                blendedAnimationClip.InputHandler = PlayableExtensions.AddInput(this.AnimationMixerPlayable, animationClipPlayable, 0);
                PlayableExtensions.Play(animationClipPlayable);
                blendedAnimationClip.AnimationClipPlayable = animationClipPlayable;
            }

            //calculate blendings
            for (var i = 0; i <  this.BlendedAnimationClips.Count; i++)
            {
                if (i == 0)
                {
                    this.BlendedAnimationClips[i].Blending =  this.BlendedAnimationClips[i].Blending.SetWeightTimePoints(
                        AnimationWeightStartIncreasingTime: 0f,
                        AnimationWeightEndIncreasingTime:  this.BlendedAnimationClips[i].WeightTime,
                        AnimationWeightStartDecreasingTime:  this.BlendedAnimationClips[i].WeightTime,
                        AnimationWeightEndDecreasingTime:  this.BlendedAnimationClips[i + 1].WeightTime
                    );
                }
                else if (i ==  this.BlendedAnimationClips.Count - 1)
                {
                    this.BlendedAnimationClips[i].Blending =  this.BlendedAnimationClips[i].Blending.SetWeightTimePoints(
                        AnimationWeightStartIncreasingTime:  this.BlendedAnimationClips[i - 1].WeightTime,
                        AnimationWeightEndIncreasingTime:  this.BlendedAnimationClips[i].WeightTime,
                        AnimationWeightStartDecreasingTime:  this.BlendedAnimationClips[i].WeightTime,
                        AnimationWeightEndDecreasingTime: 1f
                    );
                }
                else
                {
                    this.BlendedAnimationClips[i].Blending =  this.BlendedAnimationClips[i].Blending.SetWeightTimePoints(
                        AnimationWeightStartIncreasingTime:  this.BlendedAnimationClips[i - 1].WeightTime,
                        AnimationWeightEndIncreasingTime:  this.BlendedAnimationClips[i].WeightTime,
                        AnimationWeightStartDecreasingTime:  this.BlendedAnimationClips[i].WeightTime,
                        AnimationWeightEndDecreasingTime:  this.BlendedAnimationClips[i + 1].WeightTime
                    );
                }
            }
            
            this.Inputhandler = PlayableExtensions.AddInput(parentAnimationLayerMixerPlayable, this.AnimationMixerPlayable, 0);
            parentAnimationLayerMixerPlayable.SetLayerAdditive((uint) layerId, BlendedAnimationInput.IsAdditive);
            if (BlendedAnimationInput.AvatarMask != null)
            {
                parentAnimationLayerMixerPlayable.SetLayerMaskFromAvatarMask((uint) layerId, BlendedAnimationInput.AvatarMask);
            }
            
            if (InputWeightProvider != null)
            {
                this.inputWeightProvider = InputWeightProvider;
            }

        }

        private float oldWeightEvaluation = -1f;

        public override void Tick(float d)
        {
            if (this.oldWeightEvaluation != this.inputWeightProvider())
            {
                if (this.BlendedAnimationInput.BlendedAnimationSpeedCurve.BlendedSpeedCurveEnabled)
                {
                    float sampledSpeed = this.BlendedAnimationInput.BlendedAnimationSpeedCurve.SpeedCurve.Evaluate(this.inputWeightProvider());
                    foreach (var blendedAnimationClip in BlendedAnimationClips)
                    {
                        blendedAnimationClip.SetSpeed(sampledSpeed);
                    }
                }

                foreach (var blendedAnimationClip in BlendedAnimationClips)
                {
                    PlayableExtensions.SetInputWeight(this.AnimationMixerPlayable, blendedAnimationClip.InputHandler, blendedAnimationClip.Blending.GetInterpolatedWeight(this.inputWeightProvider()));
                }
            }

            this.oldWeightEvaluation = this.inputWeightProvider();
        }

        public override bool AskedToBeDestoyed()
        {
            return false;
        }

        public override AnimationMixerPlayable GetEntryPointMixerPlayable()
        {
            return this.AnimationMixerPlayable;
        }

        public override AvatarMask GetLayerAvatarMask()
        {
            return this.BlendedAnimationInput.AvatarMask;
        }

        public override bool IsLayerAdditive()
        {
            return false;
        }

        public override void Stop()
        {
        }
    }
}