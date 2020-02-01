using GeometryIntersection;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace RangeObjects
{
    public interface IIntersectionManager
    {
        void ClearState();
        void CountingForRangeIntersectionCalculator(RangeIntersectionCalculator rangeIntersectionCalculator);

        void CalculationDataSetupForRangeIntersectionCalculator(RangeIntersectionCalculator rangeIntersectionCalculator,
            IsOccludedByObstacleJobData IsOccludedByObstacleJobData, VisibilityProbeIntersectionJobData visibilityProbeIntersectionJobData, int currentObstacleIntersectionCalculatorCounter);

        void BuildJobHandle(NativeArray<IsOccludedByObstacleJobData> IsOccludedByObstacleJobData,
            NativeArray<Vector3> VisibilityProbeLocalPoints, RangeObstacleOcclusionIntersection RangeObstacleOcclusionIntersection);

        void Complete();
        void WaitForResults();
        void CreateNativeArrays();
        void Dispose();
        NativeArray<RangeIntersectionResult> GetIntersectionResults();
    }

    public struct RoundedFrustumIntersectionManager : IIntersectionManager
    {
        private NativeArray<RoundedFrustumIntersectionJobData> RoundedFrustumIntersectionJobData;
        public NativeArray<RangeIntersectionResult> RoundedFrustumIntersectionJobResult { get; private set; }

        private JobHandle JobHandle;

        private int totalRoundedFrustumTypeIntersection;
        private int currentRoundedFrustumIntersectionJobDataCounter;

        public void ClearState()
        {
            this.totalRoundedFrustumTypeIntersection = 0;
            this.currentRoundedFrustumIntersectionJobDataCounter = 0;
        }

        public void CountingForRangeIntersectionCalculator(RangeIntersectionCalculator rangeIntersectionCalculator)
        {
            if (rangeIntersectionCalculator.GetAssociatedRangeObjectType() == RangeType.ROUNDED_FRUSTUM)
            {
                this.totalRoundedFrustumTypeIntersection += 1;
            }
        }

        public void CalculationDataSetupForRangeIntersectionCalculator(RangeIntersectionCalculator rangeIntersectionCalculator,
            IsOccludedByObstacleJobData IsOccludedByObstacleJobData, VisibilityProbeIntersectionJobData visibilityProbeIntersectionJobData, int currentObstacleIntersectionCalculatorCounter)
        {
            if (rangeIntersectionCalculator.GetAssociatedRangeObjectType() == RangeType.ROUNDED_FRUSTUM)
            {
                var RoundedFrustumRangeObject = (RoundedFrustumRangeObjectV2) rangeIntersectionCalculator.GetAssociatedRangeObject();
                var RoundedFrustumIntersectionJobData = new RoundedFrustumIntersectionJobData
                {
                    RangeIntersectionCalculatorV2UniqueID = rangeIntersectionCalculator.RangeIntersectionCalculatorV2UniqueID,
                    FrustumRadius = RoundedFrustumRangeObject.GetFrustum().GetFrustumFaceRadius(),
                    RangeTransform = RoundedFrustumRangeObject.GetTransform(),
                    IsOccludedByObstacleJobData = IsOccludedByObstacleJobData,
                    RoundedFrustumPositions = RoundedFrustumRangeObject.GetFrustumWorldPositions(),
                    VisibilityProbeIntersectionJobData = visibilityProbeIntersectionJobData,
                    ObstacleCalculationDataIndex = RoundedFrustumRangeObject.GetObstacleListener() == null ? -1 : (currentObstacleIntersectionCalculatorCounter - 1)
                };
                this.RoundedFrustumIntersectionJobData[currentRoundedFrustumIntersectionJobDataCounter] = RoundedFrustumIntersectionJobData;
                currentRoundedFrustumIntersectionJobDataCounter += 1;
            }
        }

        public void BuildJobHandle(NativeArray<IsOccludedByObstacleJobData> IsOccludedByObstacleJobData,
            NativeArray<Vector3> VisibilityProbeLocalPoints, RangeObstacleOcclusionIntersection RangeObstacleOcclusionIntersection)
        {
            this.JobHandle = new RoundedFrustumIntersectionJob
            {
                RoundedFrustumIntersectionJobData = this.RoundedFrustumIntersectionJobData,
                IsOccludedByObstacleJobData = IsOccludedByObstacleJobData,
                AssociatedObstacleFrustumPointsPositions = RangeObstacleOcclusionIntersection.AssociatedObstacleFrustumPointsPositions,
                VisibilityProbeLocalPoints = VisibilityProbeLocalPoints,
                IntersectionResult = this.RoundedFrustumIntersectionJobResult
            }.Schedule(this.totalRoundedFrustumTypeIntersection, 5);
        }

        public void Complete()
        {
            this.JobHandle.Complete();
        }

        public void WaitForResults()
        {
            while (!this.JobHandle.IsCompleted)
            {
            }
        }

        public void CreateNativeArrays()
        {
            this.RoundedFrustumIntersectionJobData = new NativeArray<RoundedFrustumIntersectionJobData>(totalRoundedFrustumTypeIntersection, Allocator.TempJob);
            this.RoundedFrustumIntersectionJobResult = new NativeArray<RangeIntersectionResult>(totalRoundedFrustumTypeIntersection, Allocator.TempJob);
        }

        public void Dispose()
        {
            if (this.RoundedFrustumIntersectionJobData.IsCreated)
            {
                this.RoundedFrustumIntersectionJobData.Dispose();
            }

            if (this.RoundedFrustumIntersectionJobResult.IsCreated)
            {
                this.RoundedFrustumIntersectionJobResult.Dispose();
            }
        }

        public NativeArray<RangeIntersectionResult> GetIntersectionResults()
        {
            return this.RoundedFrustumIntersectionJobResult;
        }
    }

    [BurstCompile]
    public struct RoundedFrustumIntersectionJob : IJobParallelFor
    {
        public NativeArray<RoundedFrustumIntersectionJobData> RoundedFrustumIntersectionJobData;
        public NativeArray<RangeIntersectionResult> IntersectionResult;

        [ReadOnly] public NativeArray<IsOccludedByObstacleJobData> IsOccludedByObstacleJobData;
        [ReadOnly] public NativeArray<FrustumPointsPositions> AssociatedObstacleFrustumPointsPositions;
        [ReadOnly] public NativeArray<Vector3> VisibilityProbeLocalPoints;

        public void Execute(int RoundedFrustumIntersectionDataIndex)
        {
            var RoundedFrustumIntersectionJobData = this.RoundedFrustumIntersectionJobData[RoundedFrustumIntersectionDataIndex];

            bool isVisibleReturnValue = false;

            /// If at least one of VisibilityProbe points is visible, we consider that the targetted interactive object is visible
            
            for (var i = RoundedFrustumIntersectionJobData.VisibilityProbeIntersectionJobData.VisibilityProbePositionsBeginIndexIncluded;
                i <= RoundedFrustumIntersectionJobData.VisibilityProbeIntersectionJobData.VisibilityProbePositionsEndIndexIncluded;
                i++)
            {
                Vector3 worldVisibilityProbePoint = RoundedFrustumIntersectionJobData.VisibilityProbeIntersectionJobData.VisibilityProbeLocalToWorld.MultiplyPoint(this.VisibilityProbeLocalPoints[i]);
                bool isVisible = IsInsideRoundedFrustum(worldVisibilityProbePoint, RoundedFrustumIntersectionJobData);
                RangeIntersectionAlgorithm.IsWorldPointOccludedByObstacleTest(worldVisibilityProbePoint, ref isVisible, RoundedFrustumIntersectionJobData.ObstacleCalculationDataIndex,
                    IsOccludedByObstacleJobData, AssociatedObstacleFrustumPointsPositions);
                isVisibleReturnValue = isVisible;
                if (isVisibleReturnValue)
                {
                    break;
                }
            }

            this.IntersectionResult[RoundedFrustumIntersectionDataIndex] =
                new RangeIntersectionResult
                {
                    RangeIntersectionCalculatorV2UniqueID = RoundedFrustumIntersectionJobData.RangeIntersectionCalculatorV2UniqueID,
                    IsInsideRange = isVisibleReturnValue
                };
        }

        private bool IsInsideRoundedFrustum(Vector3 point, RoundedFrustumIntersectionJobData roundedFrustumIntersectionWithBoxJobData)
        {
            return (Vector3.Distance(point, roundedFrustumIntersectionWithBoxJobData.RangeTransform.WorldPosition) <= roundedFrustumIntersectionWithBoxJobData.FrustumRadius)
                   && Intersection.PointInsideFrustum(roundedFrustumIntersectionWithBoxJobData.RoundedFrustumPositions, point);
        }
    }

    public struct RangeIntersectionResult
    {
        public int RangeIntersectionCalculatorV2UniqueID;
        public bool IsInsideRange;
    }

    public struct RoundedFrustumIntersectionJobData
    {
        public int RangeIntersectionCalculatorV2UniqueID;
        public TransformStruct RangeTransform;
        public FrustumPointsPositions RoundedFrustumPositions;
        public float FrustumRadius;
        public VisibilityProbeIntersectionJobData VisibilityProbeIntersectionJobData;
        public IsOccludedByObstacleJobData IsOccludedByObstacleJobData;
        public int ObstacleCalculationDataIndex;
    }

    public struct SphereIntersectionManager : IIntersectionManager
    {
        private int totalSphereTypeIntersection;
        private int currentSphereIntersectionJobdataCounter;

        private NativeArray<SphereIntersectionJobData> SphereIntersectionJobData;
        public NativeArray<RangeIntersectionResult> SphereIntersectionJobResult { get; private set; }

        private JobHandle JobHandle;

        public void CountingForRangeIntersectionCalculator(RangeIntersectionCalculator rangeIntersectionCalculator)
        {
            if (rangeIntersectionCalculator.GetAssociatedRangeObjectType() == RangeType.SPHERE)
            {
                this.totalSphereTypeIntersection += 1;
            }
        }

        public void BuildJobHandle(NativeArray<IsOccludedByObstacleJobData> IsOccludedByObstacleJobData,
            NativeArray<Vector3> VisibilityProbeLocalPoints, RangeObstacleOcclusionIntersection RangeObstacleOcclusionIntersection)
        {
            this.JobHandle = new SphereIntersectionJob
            {
                SphereIntersectionJobDatas = this.SphereIntersectionJobData,
                IsOccludedByObstacleJobData = IsOccludedByObstacleJobData,
                AssociatedObstacleFrustumPointsPositions = RangeObstacleOcclusionIntersection.AssociatedObstacleFrustumPointsPositions,
                IntersectionResult = this.SphereIntersectionJobResult
            }.Schedule(totalSphereTypeIntersection, 5);
        }

        public void CalculationDataSetupForRangeIntersectionCalculator(RangeIntersectionCalculator rangeIntersectionCalculator,
            IsOccludedByObstacleJobData IsOccludedByObstacleJobData,
            VisibilityProbeIntersectionJobData visibilityProbeIntersectionJobData, int currentObstacleIntersectionCalculatorCounter)
        {
            if (rangeIntersectionCalculator.GetAssociatedRangeObjectType() == RangeType.SPHERE)
            {
                var SphereRangeObject = (SphereRangeObjectV2) rangeIntersectionCalculator.GetAssociatedRangeObject();
                var SphereIntersectionJobData = new SphereIntersectionJobData
                {
                    RangeIntersectionCalculatorV2UniqueID = rangeIntersectionCalculator.RangeIntersectionCalculatorV2UniqueID,
                    RangeTransform = SphereRangeObject.GetTransform(),
                    IsOccludedByObstacleJobData = IsOccludedByObstacleJobData,
                    VisibilityProbeIntersectionJobData = visibilityProbeIntersectionJobData,
                    ObstacleCalculationDataIndex = SphereRangeObject.GetObstacleListener() == null ? -1 : (currentObstacleIntersectionCalculatorCounter - 1),
                    SphereRadius = SphereRangeObject.SphereBoundingCollider.radius
                };
                this.SphereIntersectionJobData[this.currentSphereIntersectionJobdataCounter] = SphereIntersectionJobData;
                this.currentSphereIntersectionJobdataCounter += 1;
            }
        }

        public void ClearState()
        {
            this.totalSphereTypeIntersection = 0;
            this.currentSphereIntersectionJobdataCounter = 0;
        }

        public void Complete()
        {
            this.JobHandle.Complete();
        }

        public void CreateNativeArrays()
        {
            this.SphereIntersectionJobData = new NativeArray<SphereIntersectionJobData>(this.totalSphereTypeIntersection, Allocator.TempJob);
            this.SphereIntersectionJobResult = new NativeArray<RangeIntersectionResult>(this.totalSphereTypeIntersection, Allocator.TempJob);
        }

        public void Dispose()
        {
            if (this.SphereIntersectionJobData.IsCreated)
            {
                this.SphereIntersectionJobData.Dispose();
            }

            if (this.SphereIntersectionJobResult.IsCreated)
            {
                this.SphereIntersectionJobResult.Dispose();
            }
        }

        public void WaitForResults()
        {
            while (!this.JobHandle.IsCompleted)
            {
            }
        }

        public NativeArray<RangeIntersectionResult> GetIntersectionResults()
        {
            return this.SphereIntersectionJobResult;
        }
    }

    [BurstCompile]
    public struct SphereIntersectionJob : IJobParallelFor
    {
        public NativeArray<SphereIntersectionJobData> SphereIntersectionJobDatas;
        public NativeArray<RangeIntersectionResult> IntersectionResult;

        [ReadOnly] public NativeArray<IsOccludedByObstacleJobData> IsOccludedByObstacleJobData;
        [ReadOnly] public NativeArray<FrustumPointsPositions> AssociatedObstacleFrustumPointsPositions;
        [ReadOnly] public NativeArray<Vector3> VisibilityProbeLocalPoints;

        public void Execute(int SphereIntersectionJobDataIndex)
        {
            var SphereIntersectionJobData = this.SphereIntersectionJobDatas[SphereIntersectionJobDataIndex];

            bool isVisibleReturnValue = false;
            
            /// If at least one of VisibilityProbe points is visible, we consider that the targetted interactive object is visible
            
            for (var i = SphereIntersectionJobData.VisibilityProbeIntersectionJobData.VisibilityProbePositionsBeginIndexIncluded;
                i <= SphereIntersectionJobData.VisibilityProbeIntersectionJobData.VisibilityProbePositionsEndIndexIncluded;
                i++)
            {
                Vector3 worldVisibilityProbePoint = SphereIntersectionJobData.VisibilityProbeIntersectionJobData.VisibilityProbeLocalToWorld.MultiplyPoint(this.VisibilityProbeLocalPoints[i]);

                bool isVisible = Vector3.Distance(worldVisibilityProbePoint, SphereIntersectionJobData.RangeTransform.WorldPosition) <= SphereIntersectionJobData.SphereRadius;
                RangeIntersectionAlgorithm.IsWorldPointOccludedByObstacleTest(worldVisibilityProbePoint, ref isVisible, SphereIntersectionJobData.ObstacleCalculationDataIndex,
                    IsOccludedByObstacleJobData, AssociatedObstacleFrustumPointsPositions);
                isVisibleReturnValue = isVisible;
                if (isVisibleReturnValue)
                {
                    break;
                }
            }

            this.IntersectionResult[SphereIntersectionJobDataIndex] =
                new RangeIntersectionResult
                {
                    RangeIntersectionCalculatorV2UniqueID = SphereIntersectionJobData.RangeIntersectionCalculatorV2UniqueID,
                    IsInsideRange = isVisibleReturnValue
                };
        }
    }

    public struct SphereIntersectionJobData
    {
        public int RangeIntersectionCalculatorV2UniqueID;
        public TransformStruct RangeTransform;
        public float SphereRadius;
        public VisibilityProbeIntersectionJobData VisibilityProbeIntersectionJobData;
        public IsOccludedByObstacleJobData IsOccludedByObstacleJobData;
        public int ObstacleCalculationDataIndex;
    }
}