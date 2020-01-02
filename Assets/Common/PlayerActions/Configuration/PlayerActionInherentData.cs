﻿using System;
using OdinSerializer;
using PlayerObject_Interfaces;
using SelectionWheel;
using UnityEngine;

namespace PlayerActions
{
    [Serializable]
    public abstract class PlayerActionInherentData : SerializedScriptableObject
    {
        public CorePlayerActionDefinition CorePlayerActionDefinition;
    }

    [Serializable]
    public struct CorePlayerActionDefinition
    {
        [CustomEnum(ConfigurationType = typeof(SelectionWheelNodeConfiguration))]
        public SelectionWheelNodeConfigurationId ActionWheelNodeConfigurationId;

        [Tooltip("Number of times the action can be executed. -1 is infinite. -2 is not displayed")]
        public int ExecutionAmount;

        public float CoolDownTime;

        [Tooltip("Does the Player can move while this PlayerAction is executing ?")]
        public bool MovementAllowed;
    }
}