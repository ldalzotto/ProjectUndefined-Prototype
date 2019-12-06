using System;
using InteractiveObjects_Interfaces;
using OdinSerializer;
using UnityEngine;

namespace PlayerObject
{
    [Serializable]
    public class LowHealthPlayerSystemDefinition : SerializedScriptableObject
    {
        [Range(0.0f, 1.0f)] public float LowHealthThreshold;
        public AIMovementSpeedAttenuationFactor OnLowhealthSpeedAttenuationFactor;
    }
}