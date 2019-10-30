using System;
using System.Collections.Generic;
using UnityEngine;

namespace InteractiveObjects_Interfaces
{
    public struct AIDestination
    {
        public Vector3 WorldPosition;
        public Quaternion? Rotation;
    }

    public enum AIMovementSpeedDefinition
    {
        RUN = 0,
        WALK = 1,
        ZERO = 2
    }

    public static class AIMovementDefinitions
    {
        public static Dictionary<AIMovementSpeedDefinition, float> AIMovementSpeedAttenuationFactorLookup = new Dictionary<AIMovementSpeedDefinition, float>()
        {
            {AIMovementSpeedDefinition.ZERO, 0f},
            {AIMovementSpeedDefinition.WALK, 0.5f},
            {AIMovementSpeedDefinition.RUN, 1f}
        };
    }

    [Serializable]
    [SceneHandleDraw]
    public class AIAgentDefinition
    {
        [WireDirectionalLineAttribute(R = 0f, G = 1f, B = 0f, dY = 1f)]
        public float AgentHeight = 2f;

        [WireCircle(R = 1f, G = 0f, B = 0F)] public float AgentRadius = 0.5f;
        [WireCircle(R = 0f, G = 1f, B = 0f)] public float AgentStoppingDistance = 0.5f;
    }


    [Serializable]
    [SceneHandleDraw]
    public class InteractiveObjectLogicColliderDefinition
    {
        public bool Enabled = true;
        public bool HasRigidBody = true;

        [WireBox(R = 1, G = 1, B = 0, CenterFieldName = nameof(InteractiveObjectLogicColliderDefinition.LocalCenter),
            SizeFieldName = nameof(InteractiveObjectLogicColliderDefinition.LocalSize))]
        public Vector3 LocalCenter;

        public Vector3 LocalSize;
    }
}