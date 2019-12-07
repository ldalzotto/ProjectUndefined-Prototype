using System;
using AnimatorPlayable;
using InteractiveObjects_Interfaces;
using OdinSerializer;
using UnityEngine;

namespace PlayerLowHealth
{
    [Serializable]
    public class LowHealthPlayerSystemDefinition : SerializedScriptableObject
    {
        [Range(0.0f, 1.0f)] public float LowHealthThreshold;
        public AIMovementSpeedAttenuationFactor OnLowhealthSpeedAttenuationFactor;
        [Inline(CreateAtSameLevelIfAbsent = true)]
        public A_AnimationPlayableDefinition OnLowHealthLocomotionAnimation;
    }
}