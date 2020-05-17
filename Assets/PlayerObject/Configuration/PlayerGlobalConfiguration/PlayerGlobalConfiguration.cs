﻿using System;
using OdinSerializer;
using UnityEngine;

namespace PlayerObject
{
    [Serializable]
    [CreateAssetMenu(fileName = "PlayerGlobalConfiguration", menuName = "Configuration/CoreGame/PlayerGlobalConfiguration/PlayerGlobalConfiguration", order = 1)]
    public class PlayerGlobalConfiguration : SerializedScriptableObject
    {
        public PlayerInteractiveObjectInitializerData PlayerInteractiveObjectInitializerData;
    }

    [Serializable]
    public class PlayerInteractiveObjectInitializerData
    {
        public float MinimumDistanceToStick = 0.01f;
        public float SpeedMultiplicationFactor = 20f;
    }
}