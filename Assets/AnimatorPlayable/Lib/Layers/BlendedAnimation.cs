using System;
using System.Collections.Generic;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AnimatorPlayable
{
    public class BlendedAnimationLayer : MyAnimationLayer
    {
        public List<BlendedAnimationClip> BlendedAnimationClips;
        private BlendedAnimationSpeedCurve BlendedAnimationSpeedCurve;
        public AnimationMixerPlayable AnimationMixerPlayable { get; private set; }

        private Func<float> inputWeightProvider;

        public BlendedAnimationLayer(PlayableGraph PlayableGraph, AnimationLayerMixerPlayable parentAnimationLayerMixerPlayable,
            int layerId, List<BlendedAnimationClip> blendedAnimationClips, BlendedAnimationSpeedCurve BlendedAnimationSpeedCurve) : base(layerId, parentAnimationLayerMixerPlayable)
        {
            BlendedAnimationClips = blendedAnimationClips;
            this.BlendedAnimationSpeedCurve = BlendedAnimationSpeedCurve;
            //create a playable mixer
            this.AnimationMixerPlayable = AnimationMixerPlayable.Create(PlayableGraph);

            foreach (var blendedAnimationClip in blendedAnimationClips)
            {
                var animationClipPlayable = AnimationClipPlayable.Create(PlayableGraph, blendedAnimationClip.AnimationClip);
                animationClipPlayable.SetApplyFootIK(false);
                animationClipPlayable.SetApplyPlayableIK(false);
                blendedAnimationClip.InputHandler = PlayableExtensions.AddInput(this.AnimationMixerPlayable, animationClipPlayable, 0);
                PlayableExtensions.Play(animationClipPlayable);
                blendedAnimationClip.AnimationClipPlayable = animationClipPlayable;
            }

            //calculate blendings
            for (var i = 0; i < blendedAnimationClips.Count; i++)
            {
                if (i == 0)
                {
                    blendedAnimationClips[i].Blending = blendedAnimationClips[i].Blending.SetWeightTimePoints(
                        AnimationWeightStartIncreasingTime: 0f,
                        AnimationWeightEndIncreasingTime: blendedAnimationClips[i].WeightTime,
                        AnimationWeightStartDecreasingTime: blendedAnimationClips[i].WeightTime,
                        AnimationWeightEndDecreasingTime: blendedAnimationClips[i + 1].WeightTime
                    );
                }
                else if (i == blendedAnimationClips.Count - 1)
                {
                    blendedAnimationClips[i].Blending = blendedAnimationClips[i].Blending.SetWeightTimePoints(
                        AnimationWeightStartIncreasingTime: blendedAnimationClips[i - 1].WeightTime,
                        AnimationWeightEndIncreasingTime: blendedAnimationClips[i].WeightTime,
                        AnimationWeightStartDecreasingTime: blendedAnimationClips[i].WeightTime,
                        AnimationWeightEndDecreasingTime: 1f
                    );
                }
                else
                {
                    blendedAnimationClips[i].Blending = blendedAnimationClips[i].Blending.SetWeightTimePoints(
                        AnimationWeightStartIncreasingTime: blendedAnimationClips[i - 1].WeightTime,
                        AnimationWeightEndIncreasingTime: blendedAnimationClips[i].WeightTime,
                        AnimationWeightStartDecreasingTime: blendedAnimationClips[i].WeightTime,
                        AnimationWeightEndDecreasingTime: blendedAnimationClips[i + 1].WeightTime
                    );
                }
            }
        }

        private float oldWeightEvaluation = -1f;

        public override void RegisterInputWeightProvider(Func<float> InputWeightProvider)
        {
            this.inputWeightProvider = InputWeightProvider;
        }

        public override void Tick(float d)
        {
            if (this.oldWeightEvaluation != this.inputWeightProvider())
            {
                if (this.BlendedAnimationSpeedCurve.BlendedSpeedCurveEnabled)
                {
                    float sampledSpeed = this.BlendedAnimationSpeedCurve.SpeedCurve.Evaluate(this.inputWeightProvider());
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
    }
}