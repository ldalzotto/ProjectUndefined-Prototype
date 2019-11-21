using System.Collections.Generic;
using UnityEngine;

namespace InteractiveObjects_Interfaces
{
    public struct AIDestination
    {
        public Vector3 WorldPosition;
        public Quaternion? Rotation;

        public AIDestination(Vector3 worldPosition, Quaternion? rotation)
        {
            WorldPosition = worldPosition;
            Rotation = rotation;
        }
    }

    public interface IAgentMovementCalculationStrategy
    {
        AIDestination GetAIDestination();
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
}