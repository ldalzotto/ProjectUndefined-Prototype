﻿using System.Collections.Generic;
using CoreGame;
using GeometryIntersection;
using Obstacle;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Profiling;

namespace RangeObjects
{
    /// <summary>
    /// Executre Range Intersection calculation Job.
    /// </summary>
    public class RangeIntersectionCalculationManagerV2 : GameSingleton<RangeIntersectionCalculationManagerV2>
    {
        #region External Dependencies

        private ObstacleOcclusionCalculationManagerV2 ObstacleOcclusionCalculationManagerV2 = ObstacleOcclusionCalculationManagerV2.Get();
        private RangeIntersectionCalculatorManager _rangeIntersectionCalculatorManager = RangeIntersectionCalculatorManager.Get();

        #endregion

        private NativeArray<IsOccludedByObstacleJobData> IsOccludedByObstacleJobData;
        private NativeArray<Vector3> VisibilityProbeLocalPoints;

        #region Job State   

        private bool JobEnded;

        #endregion

        private List<RangeIntersectionCalculator> RangeIntersectionCalculatorThatChangedThatFrame = new List<RangeIntersectionCalculator>();

        private IIntersectionManager[] RangeIntersectionmanagers;

        //RangeIntersectionCalculatorV2 -> intersection value
        private Dictionary<int, bool> RangeIntersectionResults = new Dictionary<int, bool>();

        private RangeObstacleOcclusionIntersection RangeObstacleOcclusionIntersection = default;

        public RangeIntersectionCalculationManagerV2()
        {
            RangeIntersectionmanagers = new IIntersectionManager[]
            {
                new SphereIntersectionManager(),
                new RoundedFrustumIntersectionManager()
            };
        }

        private Dictionary<int, bool> GetRangeIntersectionResult()
        {
            if (!JobEnded)
            {
                JobEnded = true;
                foreach (var RangeIntersectionmanager in RangeIntersectionmanagers) RangeIntersectionmanager.Complete();

                foreach (var RangeIntersectionmanager in RangeIntersectionmanagers) RangeIntersectionmanager.WaitForResults();

                OnJobEnded();
            }

            return RangeIntersectionResults;
        }

        public bool GetRangeIntersectionResult(RangeIntersectionCalculator rangeIntersectionCalculator)
        {
            return GetRangeIntersectionResult()[rangeIntersectionCalculator.RangeIntersectionCalculatorV2UniqueID];
        }

        public void TryGetRangeintersectionResult(RangeIntersectionCalculator rangeIntersectionCalculator, out bool result)
        {
            GetRangeIntersectionResult().TryGetValue(rangeIntersectionCalculator.RangeIntersectionCalculatorV2UniqueID, out result);
        }

        public void Tick(float d)
        {
            Profiler.BeginSample("RangeIntersectionCalculationManagerV2");
            ManualCalculation(_rangeIntersectionCalculatorManager.AllRangeIntersectionCalculatorV2, false);
            Profiler.EndSample();
        }

        public void ManualCalculation(List<RangeIntersectionCalculator> InvolvedRangeIntersectionCalculatorV2, bool forceCalculation)
        {
            var AllRangeIntersectionCalculatorV2Count = InvolvedRangeIntersectionCalculatorV2.Count;
            if (AllRangeIntersectionCalculatorV2Count > 0)
            {
                #region Counting

                var totalObstacleFrustumPointsCounter = 0;
                var totalVisibilityProbePointsCounter = 0;
                foreach (var rangeIntersectionCalculatorV2 in InvolvedRangeIntersectionCalculatorV2)
                    // If the calculation is forced or the intersection calculator needs recalculation
                    if (forceCalculation || !forceCalculation && rangeIntersectionCalculatorV2.TickChangedPositions())
                    {
                        // We push the range intersection calculator to the stack that will be calculated
                        RangeIntersectionCalculatorThatChangedThatFrame.Add(rangeIntersectionCalculatorV2);
                        // We increment manager counter this will be used later to allocate native arrays.
                        foreach (var RangeIntersectionmanager in RangeIntersectionmanagers) RangeIntersectionmanager.CountingForRangeIntersectionCalculator(rangeIntersectionCalculatorV2);

                        var associatedObstacleListener = rangeIntersectionCalculatorV2.GetAssociatedObstacleListener();
                        if (associatedObstacleListener != null) //The range can ignore obstacles
                        {
                            //Obstacle listener could have never triggered a calculation
                            // We get the occlusion frustums associated to the obstacle listener. These frustums will be used during the probe visibility
                            ObstacleOcclusionCalculationManagerV2.TryGetCalculatedOcclusionFrustumsForObstacleListener(associatedObstacleListener, out var calculatedFrustumPositions);
                            if (calculatedFrustumPositions != null)
                                foreach (var calculatedObstacleFrustum in calculatedFrustumPositions.Values)
                                    totalObstacleFrustumPointsCounter += calculatedObstacleFrustum.Count;
                        }

                        // visibility probe counting
                        var visibilityProbes = rangeIntersectionCalculatorV2.TrackedInteractiveObject.InteractiveGameObject.VisibilityProbe;
                        if (visibilityProbes.LocalPoints != null)
                        {
                            totalVisibilityProbePointsCounter += visibilityProbes.LocalPoints.Length;
                        }
                    }

                #endregion

                if (RangeIntersectionCalculatorThatChangedThatFrame.Count > 0)
                {
                    foreach (var RangeIntersectionmanager in RangeIntersectionmanagers)
                        RangeIntersectionmanager.CreateNativeArrays();

                    this.IsOccludedByObstacleJobData = new NativeArray<IsOccludedByObstacleJobData>(AllRangeIntersectionCalculatorV2Count, Allocator.TempJob);
                    this.VisibilityProbeLocalPoints = new NativeArray<Vector3>(totalVisibilityProbePointsCounter, Allocator.TempJob);

                    RangeObstacleOcclusionIntersection.Prepare(totalObstacleFrustumPointsCounter, _rangeIntersectionCalculatorManager);

                    var currentObstacleIntersectionCalculatorCounter = 0;
                    var currentVisibilityProbeLocalPointsCounter = 0;

                    foreach (var RangeIntersectionCalculatorV2 in RangeIntersectionCalculatorThatChangedThatFrame)
                    {
                        if (RangeObstacleOcclusionIntersection.ForRangeInteresectionCalculator(RangeIntersectionCalculatorV2, ObstacleOcclusionCalculationManagerV2, out var IsOccludedByObstacleJobData))
                        {
                            // when there is occlusion, we push it to the job
                            this.IsOccludedByObstacleJobData[currentObstacleIntersectionCalculatorCounter] = IsOccludedByObstacleJobData;
                            currentObstacleIntersectionCalculatorCounter += 1;
                        }

                        // TODO
                        var VisibilityProbeJobData = CalculatingVisibilityProbeJobData(RangeIntersectionCalculatorV2, ref currentVisibilityProbeLocalPointsCounter);

                        foreach (var RangeIntersectionmanager in RangeIntersectionmanagers)
                            RangeIntersectionmanager.CalculationDataSetupForRangeIntersectionCalculator(RangeIntersectionCalculatorV2,
                                IsOccludedByObstacleJobData, VisibilityProbeJobData, currentObstacleIntersectionCalculatorCounter);
                    }

                    foreach (var RangeIntersectionmanager in RangeIntersectionmanagers)
                        RangeIntersectionmanager.BuildJobHandle(IsOccludedByObstacleJobData, this.VisibilityProbeLocalPoints, RangeObstacleOcclusionIntersection);

                    if (!forceCalculation)
                    {
                        JobEnded = false;
                    }
                    else
                    {
                        foreach (var RangeIntersectionmanager in RangeIntersectionmanagers) RangeIntersectionmanager.Complete();

                        foreach (var RangeIntersectionmanager in RangeIntersectionmanagers) RangeIntersectionmanager.WaitForResults();

                        OnJobEnded();
                    }
                }
            }
        }

        public void LateTick()
        {
            //We trigger the end of calculations
            GetRangeIntersectionResult();
        }

        private void OnJobEnded()
        {
            foreach (var RangeIntersectionmanager in RangeIntersectionmanagers)
            {
                foreach (var IntersectionJobResult in RangeIntersectionmanager.GetIntersectionResults()) RangeIntersectionResults[IntersectionJobResult.RangeIntersectionCalculatorV2UniqueID] = IntersectionJobResult.IsInsideRange;

                RangeIntersectionmanager.Dispose();
                RangeIntersectionmanager.ClearState();
                RangeIntersectionCalculatorThatChangedThatFrame.Clear();
            }

            if (IsOccludedByObstacleJobData.IsCreated) IsOccludedByObstacleJobData.Dispose();
            if (this.VisibilityProbeLocalPoints.IsCreated) this.VisibilityProbeLocalPoints.Dispose();

            RangeObstacleOcclusionIntersection.Dispose();
        }

        /// <summary>
        /// Build the <see cref="VisibilityProbeIntersectionJobData"/> and feed the <see cref="VisibilityProbeLocalPoints"/> based on targetted interactive object probe.
        /// </summary>
        private VisibilityProbeIntersectionJobData CalculatingVisibilityProbeJobData(RangeIntersectionCalculator RangeIntersectionCalculatorV2, ref int currentVisibilityProbeLocalPointsCounter)
        {
            var visibilityProbes = RangeIntersectionCalculatorV2.TrackedInteractiveObject.InteractiveGameObject.VisibilityProbe;
            VisibilityProbeIntersectionJobData visibilityProbeIntersectionJobData = default;

            if (visibilityProbes.LocalPoints != null && visibilityProbes.LocalPoints.Length > 0)
            {
                int startIndex = currentVisibilityProbeLocalPointsCounter;
                int endIndex = startIndex;
                for (int i = 0; i < visibilityProbes.LocalPoints.Length; i++)
                {
                    this.VisibilityProbeLocalPoints[currentVisibilityProbeLocalPointsCounter] = visibilityProbes[i];
                    endIndex = currentVisibilityProbeLocalPointsCounter;
                    currentVisibilityProbeLocalPointsCounter += 1;
                }

                visibilityProbeIntersectionJobData = new VisibilityProbeIntersectionJobData()
                {
                    VisibilityProbeLocalToWorld = RangeIntersectionCalculatorV2.TrackedInteractiveObject.InteractiveGameObject.LogicCollider.transform.localToWorldMatrix,
                    VisibilityProbePositionsBeginIndexIncluded = startIndex,
                    VisibilityProbePositionsEndIndexIncluded = endIndex
                };
            }

            return visibilityProbeIntersectionJobData;
        }
    }
}