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
                    var calculatedFrustumPositionsEnumerator = calculatedFrustumPositions.Values.GetEnumerator();
                    while (calculatedFrustumPositionsEnumerator.MoveNext())
                    {
                        var calculatedObstacleFrustumList = calculatedFrustumPositionsEnumerator.Current;

                        for (int calculatedObstacleFrustumIndex = 0; calculatedObstacleFrustumIndex < calculatedObstacleFrustumList.Count; calculatedObstacleFrustumIndex++)
                        {
                            this.AssociatedObstacleFrustumPointsPositions[currentObstacleFrustumPointsCounter] = calculatedObstacleFrustumList[calculatedObstacleFrustumIndex];
                            currentObstacleFrustumPointsCounter += 1;
                        }
                    }
                }

                IsOccludedByObstacleJobData = new IsOccludedByObstacleJobData
                {
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
        public int ObstacleFrustumPointsPositionsBeginIndex;
        public int ObstacleFrustumPointsPositionsEndIndex;

        public bool IsPointFullyOccludedByObstacle(Vector3 PointWorldPosition, NativeArray<FrustumPointsPositions> AssociatedObstacleFrustumPointsPositions)
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

    public struct VisibilityProbeJobData
    {
        public Matrix4x4 VisibilityProbeLocalToWorld;
        public int VisibilityProbePositionsBeginIndexIncluded;
        public int VisibilityProbePositionsEndIndexIncluded;

        public static VisibilityProbeJobData Empty()
        {
            VisibilityProbeJobData VisibilityProbeJobData = default;
            VisibilityProbeJobData.VisibilityProbePositionsBeginIndexIncluded = -1;
            return VisibilityProbeJobData;
        }
    }
}