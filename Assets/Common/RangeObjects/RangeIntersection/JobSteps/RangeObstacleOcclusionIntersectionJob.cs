using System.Collections.Generic;
using GeometryIntersection;
using Obstacle;
using Unity.Collections;
using UnityEngine;

namespace RangeObjects
{
    public struct RangeObstacleOcclusionIntersection
    {
        public NativeArray<FrustumPointsPositions> AssociatedObstacleFrustumPointsPositions;

        private int currentObstacleFrustumPointsCounter;

        public void Prepare(int totalObstacleFrustumPointsCounter, RangeIntersectionCalculatorManager rangeIntersectionCalculatorManager)
        {
            int AllRangeIntersectionCalculatorV2Count = rangeIntersectionCalculatorManager.AllRangeIntersectionCalculatorV2.Count;
            this.AssociatedObstacleFrustumPointsPositions = new NativeArray<FrustumPointsPositions>(totalObstacleFrustumPointsCounter, Allocator.TempJob);
            this.currentObstacleFrustumPointsCounter = 0;
        }

        public bool ForRangeInteresectionCalculator(RangeIntersectionCalculator rangeIntersectionCalculator, ObstacleOcclusionCalculationManagerV2 ObstacleOcclusionCalculationManagerV2,
            out IsOccludedByObstacleJobData IsOccludedByObstacleJobData)
        {
            int ObstacleFrustumPointsPositionsBeginIndex = currentObstacleFrustumPointsCounter;
            var associatedObstacleListener = rangeIntersectionCalculator.GetAssociatedObstacleListener();

            if (associatedObstacleListener != null)
            {
                //obstacle listener may never have triggered calculation
                ObstacleOcclusionCalculationManagerV2.TryGetCalculatedOcclusionFrustumsForObstacleListener(associatedObstacleListener, out Dictionary<int, List<FrustumPointsPositions>> calculatedFrustumPositions);
                if (calculatedFrustumPositions != null)
                {
                    foreach (var calculatedObstacleFrustumList in calculatedFrustumPositions.Values)
                    {
                        foreach (var calculatedObstacleFrustum in calculatedObstacleFrustumList)
                        {
                            this.AssociatedObstacleFrustumPointsPositions[currentObstacleFrustumPointsCounter] = calculatedObstacleFrustum;
                            currentObstacleFrustumPointsCounter += 1;
                        }
                    }
                }

                IsOccludedByObstacleJobData = new IsOccludedByObstacleJobData
                {
                    TestedBoxCollider = new BoxDefinition(rangeIntersectionCalculator.TrackedInteractiveObject.InteractiveGameObject.GetLogicColliderAsBox()),
                    ObstacleFrustumPointsPositionsBeginIndex = ObstacleFrustumPointsPositionsBeginIndex,
                    ObstacleFrustumPointsPositionsEndIndex = currentObstacleFrustumPointsCounter
                };
                return true;
            }

            IsOccludedByObstacleJobData = default;
            return false;
        }

        public void Dispose()
        {
            if (this.AssociatedObstacleFrustumPointsPositions.IsCreated)
            {
                this.AssociatedObstacleFrustumPointsPositions.Dispose();
            }
        }
    }

    public struct IsOccludedByObstacleJobData
    {
        public BoxDefinition TestedBoxCollider;
        public int ObstacleFrustumPointsPositionsBeginIndex;
        public int ObstacleFrustumPointsPositionsEndIndex;

        public bool IsOccluded(NativeArray<FrustumPointsPositions> AssociatedObstacleFrustumPointsPositions)
        {
            Intersection.ExtractBoxColliderWorldPointsV2(this.TestedBoxCollider,
                out Vector3 BC1, out Vector3 BC2, out Vector3 BC3, out Vector3 BC4, out Vector3 BC5, out Vector3 BC6, out Vector3 BC7, out Vector3 BC8);

            return
                IsPointFullyOccludedByObstacle(BC1, AssociatedObstacleFrustumPointsPositions) &&
                IsPointFullyOccludedByObstacle(BC2, AssociatedObstacleFrustumPointsPositions) &&
                IsPointFullyOccludedByObstacle(BC3, AssociatedObstacleFrustumPointsPositions) &&
                IsPointFullyOccludedByObstacle(BC4, AssociatedObstacleFrustumPointsPositions) &&
                IsPointFullyOccludedByObstacle(BC5, AssociatedObstacleFrustumPointsPositions) &&
                IsPointFullyOccludedByObstacle(BC6, AssociatedObstacleFrustumPointsPositions) &&
                IsPointFullyOccludedByObstacle(BC7, AssociatedObstacleFrustumPointsPositions) &&
                IsPointFullyOccludedByObstacle(BC8, AssociatedObstacleFrustumPointsPositions);
        }

        private bool IsPointFullyOccludedByObstacle(Vector3 PointWorldPosition, NativeArray<FrustumPointsPositions> AssociatedObstacleFrustumPointsPositions)
        {
            for (var FrustumPointsPositionsIndex = this.ObstacleFrustumPointsPositionsBeginIndex; FrustumPointsPositionsIndex < this.ObstacleFrustumPointsPositionsEndIndex; FrustumPointsPositionsIndex++)
            {
                if (Intersection.PointInsideFrustum(AssociatedObstacleFrustumPointsPositions[FrustumPointsPositionsIndex], PointWorldPosition))
                {
                    return true;
                }
            }

            return false;
        }
    }
}