﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AnimatorPlayable
{
    public class TwoDBlendTree : MyAnimationLayer
    {
        public AnimationMixerPlayable AnimationMixerPlayable { get; private set; }

        private List<TwoDBlendTreeAnimationClip> TwoDBlendTreeAnimationClips;
        private Vector2[] TwoDBlendTreeAnimationClipsPositions;
        private float[] Weights;
        private Func<Vector2> TwoDInputWheigtProvider;

        public TwoDBlendTree(int layerId, PlayableGraph PlayableGraph, AnimationLayerMixerPlayable parentAnimationLayerMixerPlayable,
            List<TwoDBlendTreeAnimationClip> TwoDBlendTreeAnimationClips) : base(layerId, parentAnimationLayerMixerPlayable)
        {
            this.TwoDBlendTreeAnimationClips = TwoDBlendTreeAnimationClips;
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
        }


        public void Register2DTwoDInputWheigtProvider(Func<Vector2> TwoDInputWheigtProvider)
        {
            this.TwoDInputWheigtProvider = TwoDInputWheigtProvider;
        }

        public override void Tick(float d)
        {
            FreeformDirectionalInterpolator.SampleWeightsPolar(this.TwoDInputWheigtProvider.Invoke(), this.TwoDBlendTreeAnimationClipsPositions, ref this.Weights);
            for (var i = 0; i < this.Weights.Length; i++)
            {
                this.AnimationMixerPlayable.SetInputWeight(this.TwoDBlendTreeAnimationClips[i].InputHandler, this.Weights[i]);
            }
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

    /*
    public class TwoDBlendTree : MyAnimationLayer
    {
        public AnimationMixerPlayable AnimationMixerPlayable { get; private set; }

        private List<TwoDBlendTreeAnimationClip> TwoDBlendTreeAnimationClips;
        private Vector2[] TwoDBlendTreeAnimationClipsPositions;
        private float[] Weights;
        private Func<Vector2> TwoDInputWheigtProvider;

        public TwoDBlendTree(int LayerID, PlayableGraph PlayableGraph, List<TwoDBlendTreeAnimationClip> TwoDBlendTreeAnimationClips,
            AnimationLayerMixerPlayable parentAnimationLayerMixerPlayable) : base(LayerID, parentAnimationLayerMixerPlayable)
        {
            this.AnimationMixerPlayable = AnimationMixerPlayable.Create(PlayableGraph);
            this.TwoDBlendTreeAnimationClips = TwoDBlendTreeAnimationClips;
            this.TwoDBlendTreeAnimationClipsPositions = this.TwoDBlendTreeAnimationClips.ConvertAll(c => c.TreePosition).ToArray();
            this.Weights = new float[this.TwoDBlendTreeAnimationClipsPositions.Length];
            foreach (var TwoDBlendTreeAnimationClip in TwoDBlendTreeAnimationClips)
            {
                var animationClipPlayable = AnimationClipPlayable.Create(PlayableGraph, TwoDBlendTreeAnimationClip.AnimationClip);
                animationClipPlayable.SetApplyFootIK(false);
                animationClipPlayable.SetApplyPlayableIK(false);
                TwoDBlendTreeAnimationClip.InputHandler = PlayableExtensions.AddInput(this.AnimationMixerPlayable, animationClipPlayable, 0);
                PlayableExtensions.Play(animationClipPlayable);
                TwoDBlendTreeAnimationClip.AnimationClipPlayable = animationClipPlayable;
            }

            this.AnimationMixerPlayable.Play();
        }

        public void Register2DTwoDInputWheigtProvider(Func<Vector2> TwoDInputWheigtProvider)
        {
            this.TwoDInputWheigtProvider = TwoDInputWheigtProvider;
        }

        public override void Tick(float d)
        {
            FreeformDirectionalInterpolator.SampleWeightsPolar(this.TwoDInputWheigtProvider.Invoke(), this.TwoDBlendTreeAnimationClipsPositions, ref this.Weights);
            for (var i = 0; i < this.Weights.Length; i++)
            {
                this.AnimationMixerPlayable.SetInputWeight(this.TwoDBlendTreeAnimationClips[i].InputHandler, this.Weights[i]);
            }
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
    */
}