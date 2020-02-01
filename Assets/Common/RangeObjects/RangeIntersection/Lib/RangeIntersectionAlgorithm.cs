using System;
using GeometryIntersection;
using Unity.Collections;
using UnityEngine;

namespace RangeObjects
{
    public static class RangeIntersectionAlgorithm
    {
        public static void IsWorldPointOccludedByObstacleTest(Vector3 WorldPoint, ref bool isVisible, int ObstacleCalculationDataIndex,
            in NativeArray<IsOccludedByObstacleJobData> IsOccludedByObstacleJobData, in NativeArray<FrustumPointsPositions> AssociatedObstacleFrustumPointsPositions)
        {
            if (isVisible && ObstacleCalculationDataIndex != -1)
            {
                isVisible = !IsOccludedByObstacleJobData[ObstacleCalculationDataIndex]
                    .IsPointFullyOccludedByObstacle(WorldPoint, AssociatedObstacleFrustumPointsPositions);
            }
        }
    }
}