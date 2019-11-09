using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace GeometryIntersection
{
    [BurstCompile(FloatPrecision.High, FloatMode.Default)]
    public struct GeometryFaceIntersectionCalculationJob : IJobParallelFor
    {
        public NativeArray<GeometryIntersectionJobSingleCalculationData> GeometryIntersectionJobSingleCalculationDatas;
        public NativeArray<bool> GeometryIntersectionJobSingleCalculationResult;

        public void Execute(int index)
        {
            this.GeometryIntersectionJobSingleCalculationResult[index] =
                Intersection.GeometryFaceIntersection(this.GeometryIntersectionJobSingleCalculationDatas[index].SourcePoint, this.GeometryIntersectionJobSingleCalculationDatas[index].TargetPoint);
        }
    }

    [BurstCompile]
    public struct GeometryFaceIntersectionCombined : IJobParallelFor
    {
        [ReadOnly] public NativeArray<bool> GeometryIntersectionJobSingleCalculationResult;
        public NativeArray<GeometryFaceIntersectionResultIndex> GeometryFaceIntersectionResultIndexes;
        public NativeArray<bool> GeometryFaceIntersectionResult;

        public void Execute(int index)
        {
            var GeometryFaceIntersectionResultIndex = this.GeometryFaceIntersectionResultIndexes[index];
            for (var i = GeometryFaceIntersectionResultIndex.StartIncluded; i < GeometryFaceIntersectionResultIndex.EndExcluded; i++)
            {
                if (this.GeometryIntersectionJobSingleCalculationResult[i])
                {
                    this.GeometryFaceIntersectionResult[index] = true;
                    break;
                }
            }
        }
    }

    [Serializable]
    public struct GeometryFaceIntersectionResultIndex
    {
        public int StartIncluded;
        public int EndExcluded;
    }
}