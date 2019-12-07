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

    public enum AIMovementSpeedAttenuationFactor
    {
        RUN = 0,
        WALK = 1,
        ZERO = 2,
        WALK_INJURED = 3
    }

    public static class AIMovementSpeedAttenuationFactors
    {
        public static Dictionary<AIMovementSpeedAttenuationFactor, float> AIMovementSpeedAttenuationFactorLookup = new Dictionary<AIMovementSpeedAttenuationFactor, float>()
        {
            {AIMovementSpeedAttenuationFactor.ZERO, 0f},
            {AIMovementSpeedAttenuationFactor.WALK, 0.5f},
            {AIMovementSpeedAttenuationFactor.WALK_INJURED, 0.6f},
            {AIMovementSpeedAttenuationFactor.RUN, 1f}
        };
    }
}