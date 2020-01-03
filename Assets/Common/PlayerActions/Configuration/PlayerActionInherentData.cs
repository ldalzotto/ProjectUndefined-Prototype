using System;
using OdinSerializer;
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
        [Tooltip("Number of times the action can be executed. -1 is infinite. -2 is not displayed")]
        public int ExecutionAmount;

        [HideInInspector] public bool CooldownEnabled;

        [Foldable(canBeDisabled: true, disablingBoolAttribute: nameof(CooldownEnabled))]
        public CorePlayerActionCooldownDefinition CorePlayerActionCooldownDefinition;

        [Tooltip("Does the Player can move while this PlayerAction is executing ?")]
        public bool MovementAllowed;
    }

    [Serializable]
    public struct CorePlayerActionCooldownDefinition
    {
        public float CoolDownTime;
    }
}