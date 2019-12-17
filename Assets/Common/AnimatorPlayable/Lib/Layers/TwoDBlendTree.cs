using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AnimatorPlayable
{
    public class TwoDBlendTree : MyAnimationLayer
    {
        public AnimationMixerPlayable AnimationMixerPlayable { get; private set; }

        private TwoDAnimationInput TwoDAnimationInput;
        
        private List<TwoDBlendTreeAnimationClip> TwoDBlendTreeAnimationClips;
        private Vector2[] TwoDBlendTreeAnimationClipsPositions;
        private float[] Weights;

        private Vector2 TwoDInputWeight;

        public TwoDBlendTree(int layerId, PlayableGraph PlayableGraph, AnimationLayerMixerPlayable parentAnimationLayerMixerPlayable,
            TwoDAnimationInput TwoDAnimationInput) : base(layerId, parentAnimationLayerMixerPlayable)
        {
            this.TwoDAnimationInput = TwoDAnimationInput;
            this.TwoDBlendTreeAnimationClips = TwoDAnimationInput.TwoDBlendTreeAnimationClipInputs.ConvertAll(i => new TwoDBlendTreeAnimationClip(i.AnimationClip, i.TreePosition, i.Speed));
            this.TwoDBlendTreeAnimationClipsPositions = this.TwoDBlendTreeAnimationClips.ConvertAll(c => c.TreePosition).ToArray();
            this.Weights = new float[this.TwoDBlendTreeAnimationClipsPositions.Length];

            //create a playable mixer
            this.AnimationMixerPlayable = AnimationMixerPlayable.Create(PlayableGraph);

            foreach (var TwoDBlendTreeAnimationClip in TwoDBlendTreeAnimationClips)
            {
                var animationClipPlayable = AnimationClipPlayable.Create(PlayableGraph, TwoDBlendTreeAnimationClip.AnimationClip);
                animationClipPlayable.SetApplyFootIK(false);
                animationClipPlayable.SetApplyPlayableIK(false);
                animationClipPlayable.SetSpeed(TwoDBlendTreeAnimationClip.Speed);
                TwoDBlendTreeAnimationClip.InputHandler = PlayableExtensions.AddInput(this.AnimationMixerPlayable, animationClipPlayable, 0);
                PlayableExtensions.Play(animationClipPlayable);
                TwoDBlendTreeAnimationClip.AnimationClipPlayable = animationClipPlayable;
            }

            this.Inputhandler = PlayableExtensions.AddInput(parentAnimationLayerMixerPlayable, this.AnimationMixerPlayable, 0);
            if (TwoDAnimationInput.AvatarMask != null)
            {
                parentAnimationLayerMixerPlayable.SetLayerMaskFromAvatarMask((uint) this.Inputhandler, TwoDAnimationInput.AvatarMask);
            }
        }

        public override void Tick(float d)
        {
            FreeformDirectionalInterpolator.SampleWeightsPolar(this.TwoDInputWeight, this.TwoDBlendTreeAnimationClipsPositions, ref this.Weights);
            for (var i = 0; i < this.Weights.Length; i++)
            {
                this.AnimationMixerPlayable.SetInputWeight(this.TwoDBlendTreeAnimationClips[i].InputHandler, this.Weights[i]);
            }
        }

        public override bool AskedToBeDestoyed()
        {
            return false;
        }

        public override void SetTwoDInputWeight(Vector2 inputWeihgt)
        {
            this.TwoDInputWeight = inputWeihgt;
        }

        public override void Stop()
        {
            
        }

        public override AnimationMixerPlayable GetEntryPointMixerPlayable()
        {
            return this.AnimationMixerPlayable;
        }

        public override AvatarMask GetLayerAvatarMask()
        {
            return this.TwoDAnimationInput.AvatarMask;
        }

        public override bool IsLayerAdditive()
        {
            return false;
        }
    }
    
}