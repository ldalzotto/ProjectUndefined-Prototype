using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AnimatorPlayable
{
    public class SequencedAnimationLayer : MyAnimationLayer
    {
        public List<UniqueAnimationClip> UniqueAnimationClips;
        public AnimationClipPlayable[] AssociatedAnimationClipsPlayable { get; private set; }

        public AnimationMixerPlayable AnimationMixerPlayable { get; private set; }

        private SequencedAnimationInput SequencedAnimationInput;

        private bool IsTransitioningIn;
        private bool IsTransitioningOut;
        private float TransitioningOutStartTime;
        private bool HasEnded;

        private Action OnSequencedAnimationEnd;

        public SequencedAnimationLayer(PlayableGraph playableGraph, AnimationLayerMixerPlayable parentAnimationLayerMixerPlayable, int layerId,
            SequencedAnimationInput SequencedAnimationInput, Action OnAnimationEnd) : base(layerId, parentAnimationLayerMixerPlayable)
        {
            this.SequencedAnimationInput = SequencedAnimationInput;
            this.UniqueAnimationClips = SequencedAnimationInput.UniqueAnimationClips.ConvertAll(clip => clip.ToUniqueAnimationClip());
            this.IsTransitioningIn = false;
            this.IsTransitioningOut = false;
            this.HasEnded = false;

            this.AnimationMixerPlayable = AnimationMixerPlayable.Create(playableGraph);
            this.AssociatedAnimationClipsPlayable = new AnimationClipPlayable[this.UniqueAnimationClips.Count];

            this.Inputhandler = PlayableExtensions.AddInput(this.ParentAnimationLayerMixerPlayable, this.AnimationMixerPlayable, 0);

            if (SequencedAnimationInput.AvatarMask != null)
            {
                this.ParentAnimationLayerMixerPlayable.SetLayerMaskFromAvatarMask((uint) this.Inputhandler, SequencedAnimationInput.AvatarMask);
            }

            this.ParentAnimationLayerMixerPlayable.SetLayerAdditive((uint) this.Inputhandler, SequencedAnimationInput.IsAdditive);

            if (OnAnimationEnd != null)
            {
                this.OnSequencedAnimationEnd = OnAnimationEnd;
            }

            for (var i = 0; i < this.UniqueAnimationClips.Count; i++)
            {
                this.AssociatedAnimationClipsPlayable[i] = AnimationClipPlayable.Create(playableGraph, this.UniqueAnimationClips[i].AnimationClip);
                PlayableExtensions.SetDuration(this.AssociatedAnimationClipsPlayable[i], this.UniqueAnimationClips[i].AnimationClip.length);
                this.AssociatedAnimationClipsPlayable[i].SetApplyFootIK(false);
                this.AssociatedAnimationClipsPlayable[i].SetApplyPlayableIK(false);
                this.AssociatedAnimationClipsPlayable[i].Pause();
                this.UniqueAnimationClips[i].InputHandler = PlayableExtensions.AddInput(this.AnimationMixerPlayable, this.AssociatedAnimationClipsPlayable[i], 0);
            }

            if (this.SequencedAnimationInput.BeginTransitionTime > 0f)
            {
                this.IsTransitioningIn = true;
            }
            else
            {
                SetAnimationMixerPlayableWeight(this.AnimationMixerPlayable, this.AssociatedAnimationClipsPlayable[0], this.UniqueAnimationClips[0].InputHandler, 1f);
            }


            //Calculate linear blendings times
            float virtualClipElapsedTime = 0f;


            for (var i = 0; i < this.UniqueAnimationClips.Count; i++)
            {
                float animationClipLength = this.UniqueAnimationClips[i].AnimationClip.length;
                if (this.UniqueAnimationClips[i].AnimationSpeedMultiplier != 0)
                {
                    animationClipLength = animationClipLength / this.UniqueAnimationClips[i].AnimationSpeedMultiplier;
                }

                if (i == 0)
                {
                    virtualClipElapsedTime = this.SequencedAnimationInput.BeginTransitionTime;
                    this.UniqueAnimationClips[i].TransitionBlending = this.UniqueAnimationClips[i].TransitionBlending.SetWeightTimePoints(
                        AnimationWeightStartIncreasingTime: virtualClipElapsedTime,
                        AnimationWeightEndIncreasingTime: virtualClipElapsedTime,
                        AnimationWeightStartDecreasingTime: virtualClipElapsedTime + this.UniqueAnimationClips[i].TransitionBlending.EndClipDelay + animationClipLength - this.UniqueAnimationClips[i].TransitionBlending.EndTransitionTime,
                        AnimationWeightEndDecreasingTime: virtualClipElapsedTime + this.UniqueAnimationClips[i].TransitionBlending.EndClipDelay + animationClipLength
                    );

                    virtualClipElapsedTime += animationClipLength + this.UniqueAnimationClips[i].TransitionBlending.EndClipDelay;
                }
                else if (i == this.UniqueAnimationClips.Count - 1)
                {
                    this.UniqueAnimationClips[i].TransitionBlending = this.UniqueAnimationClips[i].TransitionBlending.SetWeightTimePoints(
                        AnimationWeightStartIncreasingTime: virtualClipElapsedTime - this.UniqueAnimationClips[i - 1].TransitionBlending.EndTransitionTime,
                        AnimationWeightEndIncreasingTime: virtualClipElapsedTime,
                        AnimationWeightStartDecreasingTime: (virtualClipElapsedTime + this.UniqueAnimationClips[i].TransitionBlending.EndClipDelay - this.UniqueAnimationClips[i - 1].TransitionBlending.EndTransitionTime) + animationClipLength,
                        AnimationWeightEndDecreasingTime: (virtualClipElapsedTime + this.UniqueAnimationClips[i].TransitionBlending.EndClipDelay - this.UniqueAnimationClips[i - 1].TransitionBlending.EndTransitionTime) + animationClipLength
                    );

                    virtualClipElapsedTime += animationClipLength + this.UniqueAnimationClips[i].TransitionBlending.EndClipDelay - this.UniqueAnimationClips[i - 1].TransitionBlending.EndTransitionTime;
                }
                else
                {
                    this.UniqueAnimationClips[i].TransitionBlending = this.UniqueAnimationClips[i].TransitionBlending.SetWeightTimePoints(
                        AnimationWeightStartIncreasingTime: virtualClipElapsedTime - this.UniqueAnimationClips[i - 1].TransitionBlending.EndTransitionTime,
                        AnimationWeightEndIncreasingTime: virtualClipElapsedTime,
                        AnimationWeightStartDecreasingTime: (virtualClipElapsedTime + this.UniqueAnimationClips[i].TransitionBlending.EndClipDelay - this.UniqueAnimationClips[i - 1].TransitionBlending.EndTransitionTime) + animationClipLength -
                                                            this.UniqueAnimationClips[i].TransitionBlending.EndTransitionTime,
                        AnimationWeightEndDecreasingTime: (virtualClipElapsedTime + this.UniqueAnimationClips[i].TransitionBlending.EndClipDelay - this.UniqueAnimationClips[i - 1].TransitionBlending.EndTransitionTime) + animationClipLength
                    );

                    virtualClipElapsedTime += animationClipLength + this.UniqueAnimationClips[i].TransitionBlending.EndClipDelay - this.UniqueAnimationClips[i - 1].TransitionBlending.EndTransitionTime;
                }
            }

            PlayableExtensions.SetTime(this.AnimationMixerPlayable, 0);
        }

        public override void Tick(float d)
        {
            if (!this.HasEnded)
            {
                var elapsedTime = PlayableExtensions.GetTime(this.AnimationMixerPlayable);
                if (this.IsTransitioningIn)
                {
                    float weightSetted = Mathf.Clamp01((float) elapsedTime / this.SequencedAnimationInput.BeginTransitionTime);
                    SetAnimationMixerPlayableWeight(this.AnimationMixerPlayable, this.AssociatedAnimationClipsPlayable[0], this.UniqueAnimationClips[0].InputHandler, 1f);
                    this.AssociatedAnimationClipsPlayable[0].Pause();
                    PlayableExtensions.SetInputWeight(this.ParentAnimationLayerMixerPlayable, this.Inputhandler, weightSetted);
                    if (weightSetted == 1f)
                    {
                        this.IsTransitioningIn = false;
                        this.AssociatedAnimationClipsPlayable[0].Play();
                        this.AssociatedAnimationClipsPlayable[0].SetSpeed(this.UniqueAnimationClips[0].AnimationSpeedMultiplier);
                    }
                }
                else if (this.IsTransitioningOut)
                {
                    float weightSetted = Mathf.Clamp01(((this.SequencedAnimationInput.EndTransitionTime - ((float) elapsedTime - this.TransitioningOutStartTime)) / this.SequencedAnimationInput.EndTransitionTime));

                    this.AssociatedAnimationClipsPlayable[this.AssociatedAnimationClipsPlayable.Length - 1].SetTime(this.AssociatedAnimationClipsPlayable[this.AssociatedAnimationClipsPlayable.Length - 1].GetDuration());
                    PlayableExtensions.SetInputWeight(this.ParentAnimationLayerMixerPlayable, this.Inputhandler, weightSetted);
                    if (weightSetted == 0f)
                    {
                        this.HasEnded = true;
                        this.IsTransitioningOut = false;
                    }
                }
                else
                {
                    bool atLeastOneClipIsPlaying = false;
                    for (var i = 0; i < this.UniqueAnimationClips.Count; i++)
                    {
                        this.AssociatedAnimationClipsPlayable[i].SetSpeed(this.UniqueAnimationClips[i].AnimationSpeedMultiplier);
                        if (i == this.UniqueAnimationClips.Count - 1 && elapsedTime >= this.UniqueAnimationClips[i].TransitionBlending.AnimationWeightEndDecreasingTime)
                        {
                            SetAnimationMixerPlayableWeight(this.AnimationMixerPlayable, this.AssociatedAnimationClipsPlayable[i], this.UniqueAnimationClips[i].InputHandler, 1f);
                            atLeastOneClipIsPlaying = false;
                        }
                        else
                        {
                            float weightSetted = this.UniqueAnimationClips[i].TransitionBlending.GetInterpolatedWeight((float) elapsedTime);
                            SetAnimationMixerPlayableWeight(this.AnimationMixerPlayable, this.AssociatedAnimationClipsPlayable[i], this.UniqueAnimationClips[i].InputHandler, weightSetted);
                            if (weightSetted > 0)
                            {
                                atLeastOneClipIsPlaying = true;
                            }
                        }
                    }

                    if (!atLeastOneClipIsPlaying)
                    {
                        if (this.SequencedAnimationInput.EndTransitionTime > 0f && !this.SequencedAnimationInput.isInfinite)
                        {
                            //  Debug.Break();
                            this.IsTransitioningOut = true;
                            this.TransitioningOutStartTime = (float) elapsedTime;
                        }
                        else if (this.SequencedAnimationInput.isInfinite)
                        {
                            this.HasEnded = false;
                            this.AnimationMixerPlayable.SetTime(0f);
                            for (var i = 0; i < this.AssociatedAnimationClipsPlayable.Length; i++)
                            {
                                this.AssociatedAnimationClipsPlayable[i].SetTime(0f);
                                this.AssociatedAnimationClipsPlayable[i].Pause();
                                this.AssociatedAnimationClipsPlayable[i].SetDone(false);
                            }

                            this.AssociatedAnimationClipsPlayable[0].Play();
                        }
                        else
                        {
                            this.HasEnded = true;
                        }

                        PlayableExtensions.SetInputWeight(AnimationMixerPlayable, this.UniqueAnimationClips[this.UniqueAnimationClips.Count - 1].InputHandler, 1f);
                    }
                }
            }
        }


        private static void SetAnimationMixerPlayableWeight(AnimationMixerPlayable AnimationMixerPlayable, AnimationClipPlayable AnimationClipPlayable, int inputHandler, float weight)
        {
            if (PlayableExtensions.GetInputWeight(AnimationMixerPlayable, inputHandler) == 0f && weight > 0f)
            {
                AnimationClipPlayable.SetTime(0f);
                AnimationClipPlayable.Play();
            }
            else if (weight == 0f)
            {
                AnimationClipPlayable.Pause();
            }

            PlayableExtensions.SetInputWeight(AnimationMixerPlayable, inputHandler, weight);
        }

        public override bool AskedToBeDestoyed()
        {
            return this.HasEnded && !this.SequencedAnimationInput.isInfinite;
        }

        public override AnimationMixerPlayable GetEntryPointMixerPlayable()
        {
            return this.AnimationMixerPlayable;
        }

        public override AvatarMask GetLayerAvatarMask()
        {
            return this.SequencedAnimationInput.AvatarMask;
        }

        public override bool IsLayerAdditive()
        {
            return this.SequencedAnimationInput.IsAdditive;
        }

        public override void Destroy(AnimationLayerMixerPlayable AnimationLayerMixerPlayable)
        {
            base.Destroy(AnimationLayerMixerPlayable);
            this.AnimationMixerPlayable.Destroy();
            if (this.OnSequencedAnimationEnd != null)
            {
                this.OnSequencedAnimationEnd.Invoke();
            }
        }


        public override void Stop()
        {
            this.IsTransitioningIn = false;
            this.IsTransitioningOut = true;
        }
    }
}