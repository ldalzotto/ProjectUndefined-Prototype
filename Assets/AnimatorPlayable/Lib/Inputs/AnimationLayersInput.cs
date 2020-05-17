﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AnimatorPlayable
{
    public enum AnimationInputType
    {
        BLENDED,
        SEQUENCED
    }

    public interface IAnimationInput
    {
        AnimationInputType AnimationInputType { get; }
    }

    [Serializable]
    public class BlendedAnimationInput : IAnimationInput
    {
        public AnimationInputType AnimationInputType { get; private set; } = AnimationInputType.BLENDED;
        public List<BlendedAnimationClipInput> BlendedAnimationClips;
        public BlendedAnimationSpeedCurve BlendedAnimationSpeedCurve;
    }

    [Serializable]
    public class SequencedAnimationInput : IAnimationInput
    {
        public AnimationInputType AnimationInputType { get; private set; } = AnimationInputType.SEQUENCED;
        public bool isInfinite;
        public float BeginTransitionTime;
        public float EndTransitionTime;
        public List<UniqueAnimationClipInput> UniqueAnimationClips;
    }

    [Serializable]
    public class UniqueAnimationClip
    {
        public AnimationClip AnimationClip;

        public LinearBlending TransitionBlending;

        #region Dynamically Setted

        public int InputHandler;

        #endregion
    }

    [Serializable]
    public class UniqueAnimationClipInput
    {
        public AnimationClip AnimationClip;

        public LinearBlending TransitionBlending;

        public UniqueAnimationClip ToUniqueAnimationClip()
        {
            return new UniqueAnimationClip()
            {
                AnimationClip = AnimationClip,
                TransitionBlending = TransitionBlending
            };
        }
    }

    [Serializable]
    public struct LinearBlending
    {
        public float EndTransitionTime;
        public float EndClipDelay;

        [MyReadOnly] public float AnimationWeightStartIncreasingTime;
        [MyReadOnly] public float AnimationWeightEndIncreasingTime;

        [MyReadOnly] public float AnimationWeightStartDecreasingTime;
        [MyReadOnly] public float AnimationWeightEndDecreasingTime;


        public LinearBlending SetWeightTimePoints(float AnimationWeightStartIncreasingTime, float AnimationWeightEndIncreasingTime, float AnimationWeightStartDecreasingTime, float AnimationWeightEndDecreasingTime)
        {
            this.AnimationWeightStartIncreasingTime = AnimationWeightStartIncreasingTime;
            this.AnimationWeightEndIncreasingTime = AnimationWeightEndIncreasingTime;
            this.AnimationWeightStartDecreasingTime = AnimationWeightStartDecreasingTime;
            this.AnimationWeightEndDecreasingTime = AnimationWeightEndDecreasingTime;
            return this;
        }

        public float GetInterpolatedWeight(float sampledTime)
        {
            if (sampledTime >= this.AnimationWeightStartIncreasingTime && sampledTime <= this.AnimationWeightEndIncreasingTime)
            {
                if ((this.AnimationWeightEndIncreasingTime - this.AnimationWeightStartIncreasingTime) == 0f)
                {
                    return 1f;
                }

                return (sampledTime - this.AnimationWeightStartIncreasingTime) / (this.AnimationWeightEndIncreasingTime - this.AnimationWeightStartIncreasingTime);
            }
            else if (sampledTime >= this.AnimationWeightEndIncreasingTime && sampledTime <= this.AnimationWeightStartDecreasingTime)
            {
                return 1f;
            }
            else if (sampledTime >= this.AnimationWeightStartDecreasingTime && sampledTime <= this.AnimationWeightEndDecreasingTime)
            {
                if ((this.AnimationWeightEndDecreasingTime - this.AnimationWeightStartDecreasingTime) == 0f)
                {
                    return 0f;
                }

                return 1 - ((sampledTime - this.AnimationWeightStartDecreasingTime) / (this.AnimationWeightEndDecreasingTime - this.AnimationWeightStartDecreasingTime));
            }
            else
            {
                return 0f;
            }
        }
    }


    [Serializable]
    public class BlendedAnimationClip
    {
        public AnimationClip AnimationClip;
        [Range(0f, 1f)] public float WeightTime;
        public float Speed = 1f;
        [MyReadOnly] public LinearBlending Blending;
        [MyReadOnly] public int InputHandler;
        [NonSerialized] public AnimationClipPlayable AnimationClipPlayable;


        public void SetSpeed(float speed)
        {
            this.AnimationClipPlayable.SetSpeed(speed);
        }
    }


    [Serializable]
    public class BlendedAnimationClipInput
    {
        public AnimationClip AnimationClip;
        [Range(0f, 1f)] public float WeightTime;
        public float Speed = 1f;

        public BlendedAnimationClip ToBlendedAnimationClip()
        {
            return new BlendedAnimationClip()
            {
                AnimationClip = AnimationClip, WeightTime = WeightTime, Speed = Speed
            };
        }
    }

    [Serializable]
    public struct BlendedAnimationSpeedCurve
    {
        public bool BlendedSpeedCurveEnabled;
        public AnimationCurve SpeedCurve;
    }
}