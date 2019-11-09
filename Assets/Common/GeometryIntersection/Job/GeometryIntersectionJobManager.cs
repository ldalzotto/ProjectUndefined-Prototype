using System;
using System.Collections.Generic;
using CoreGame;
using Unity.Collections;
using Unity.Jobs;

namespace GeometryIntersection
{
    public class GeometryIntersectionJobManager : GameSingleton<GeometryIntersectionJobManager>
    {
        private List<IGeometryIntersectionJobSingleCalculation> GeometryIntersectionJobCalculations = new List<IGeometryIntersectionJobSingleCalculation>();
        private NativeArray<GeometryIntersectionJobSingleCalculationData> GeometryIntersectionJobSingleCalculationDatas;
        private NativeArray<bool> GeometryIntersectionJobSingleCalculationResults;

        private NativeArray<GeometryFaceIntersectionResultIndex> GeometryFaceIntersectionResultIndexes;
        private NativeArray<bool> GeometryFaceIntersectionResult;

        private JobHandle JobHandle;

        public void Tick(float d)
        {
            //Counting
            int calculationCount = 0;
            foreach (var geometryIntersectionJobCalculation in this.GeometryIntersectionJobCalculations)
            {
                calculationCount += geometryIntersectionJobCalculation.GetCaluclationCount();
            }

            int currentCalculationCount = 0;
            if (calculationCount > 0)
            {
                this.GeometryIntersectionJobSingleCalculationDatas = new NativeArray<GeometryIntersectionJobSingleCalculationData>(calculationCount, Allocator.TempJob);
                this.GeometryIntersectionJobSingleCalculationResults = new NativeArray<bool>(calculationCount, Allocator.TempJob);
                for (var i = 0; i < this.GeometryIntersectionJobCalculations.Count; i++)
                {
                    this.GeometryIntersectionJobCalculations[i].GeometryIntersectionJobSingleCalculationInitialize(ref this.GeometryIntersectionJobSingleCalculationDatas, ref currentCalculationCount);
                }

                var GeometryFaceIntersectionCalculationJobHandle = new GeometryFaceIntersectionCalculationJob()
                {
                    GeometryIntersectionJobSingleCalculationDatas = this.GeometryIntersectionJobSingleCalculationDatas,
                    GeometryIntersectionJobSingleCalculationResult = this.GeometryIntersectionJobSingleCalculationResults
                }.Schedule(calculationCount, 10);


                this.GeometryFaceIntersectionResultIndexes = new NativeArray<GeometryFaceIntersectionResultIndex>(this.GeometryIntersectionJobCalculations.Count, Allocator.TempJob);
                this.GeometryFaceIntersectionResult = new NativeArray<bool>(this.GeometryIntersectionJobCalculations.Count, Allocator.TempJob);

                for (var i = 0; i < this.GeometryIntersectionJobCalculations.Count; i++)
                {
                    this.GeometryIntersectionJobCalculations[i].IntersectionCombinedJobInitialize(ref this.GeometryFaceIntersectionResultIndexes, i);
                }

                this.JobHandle = new GeometryFaceIntersectionCombined()
                {
                    GeometryIntersectionJobSingleCalculationResult = this.GeometryIntersectionJobSingleCalculationResults,
                    GeometryFaceIntersectionResultIndexes = this.GeometryFaceIntersectionResultIndexes,
                    GeometryFaceIntersectionResult = this.GeometryFaceIntersectionResult
                }.Schedule(this.GeometryIntersectionJobCalculations.Count, 10, dependsOn: GeometryFaceIntersectionCalculationJobHandle);
            }
        }

        public void LateTick()
        {
            this.WaitForJobHandle();
        }

        public void WaitForJobHandle()
        {
            if (!this.JobHandle.IsCompleted)
            {
                this.JobHandle.Complete();
                this.OnjobCompleted();
            }
        }

        private void OnjobCompleted()
        {
            for (var i = 0; i < GeometryIntersectionJobCalculations.Count; i++)
            {
                this.GeometryIntersectionJobCalculations[i].PersistResults(ref this.GeometryFaceIntersectionResult, i);
            }

            this.GeometryIntersectionJobSingleCalculationDatas.Dispose();
            this.GeometryIntersectionJobSingleCalculationResults.Dispose();
            this.GeometryFaceIntersectionResultIndexes.Dispose();
            this.GeometryFaceIntersectionResult.Dispose();
        }

        public void Add(IGeometryIntersectionJobSingleCalculation IGeometryIntersectionJobSingleCalculation)
        {
            this.GeometryIntersectionJobCalculations.Add(IGeometryIntersectionJobSingleCalculation);
        }

        public void Remove(IGeometryIntersectionJobSingleCalculation IGeometryIntersectionJobSingleCalculation)
        {
            this.GeometryIntersectionJobCalculations.Remove(IGeometryIntersectionJobSingleCalculation);
        }
    }

    [Serializable]
    public struct GeometryIntersectionJobSingleCalculationData
    {
        public SingleFacePosition SourcePoint;
        public SingleFacePosition TargetPoint;
    }

    public interface IGeometryIntersectionJobSingleCalculation
    {
        int GetCaluclationCount();

        void GeometryIntersectionJobSingleCalculationInitialize(ref NativeArray<GeometryIntersectionJobSingleCalculationData> GeometryIntersectionJobSingleCalculationDatas,
            ref int CurrentCounter);

        void IntersectionCombinedJobInitialize(ref NativeArray<GeometryFaceIntersectionResultIndex> GeometryFaceIntersectionResultIndexes, int index);

        void PersistResults(ref NativeArray<bool> GeometryFaceIntersectionResult, int index);

        bool Intersected();
    }

    public class GeometryIntersectionJobSingleCalculation : IGeometryIntersectionJobSingleCalculation
    {
        private BoxDefinitionManaged Box1;
        private BoxDefinitionManaged Box2;
        private List<int> SingleCalculationJobResultIndexes = new List<int>();
        private int FinalCalculationResultIntex;
        private bool IsIntersected;

        public GeometryIntersectionJobSingleCalculation(BoxDefinitionManaged box1, BoxDefinitionManaged box2)
        {
            Box1 = box1;
            Box2 = box2;
        }

        private SingleFacePosition[] Box1Faces;
        private SingleFacePosition[] Box2Faces;

        public int GetCaluclationCount()
        {
            this.Box1Faces = this.Box1.GetFaces();
            this.Box2Faces = this.Box2.GetFaces();
            return this.Box1Faces.Length * this.Box2Faces.Length;
        }

        public void GeometryIntersectionJobSingleCalculationInitialize(ref NativeArray<GeometryIntersectionJobSingleCalculationData> GeometryIntersectionJobSingleCalculationDatas, ref int CurrentCounter)
        {
            this.SingleCalculationJobResultIndexes.Clear();

            foreach (var box1Faces in this.Box1Faces)
            {
                foreach (var box2Faces in this.Box2Faces)
                {
                    GeometryIntersectionJobSingleCalculationDatas[CurrentCounter] = new GeometryIntersectionJobSingleCalculationData()
                    {
                        SourcePoint = box1Faces,
                        TargetPoint = box2Faces
                    };
                    this.SingleCalculationJobResultIndexes.Add(CurrentCounter);

                    CurrentCounter += 1;
                }
            }
        }

        public void IntersectionCombinedJobInitialize(ref NativeArray<GeometryFaceIntersectionResultIndex> GeometryFaceIntersectionResultIndexes, int index)
        {
            this.FinalCalculationResultIntex = index;
            GeometryFaceIntersectionResultIndexes[index] = new GeometryFaceIntersectionResultIndex()
            {
                StartIncluded = this.SingleCalculationJobResultIndexes[0],
                EndExcluded = this.SingleCalculationJobResultIndexes[this.SingleCalculationJobResultIndexes.Count - 1]
            };
        }

        public void PersistResults(ref NativeArray<bool> GeometryFaceIntersectionResult, int index)
        {
            this.IsIntersected = GeometryFaceIntersectionResult[index];
        }

        public bool Intersected()
        {
            return this.IsIntersected;
        }
    }
}