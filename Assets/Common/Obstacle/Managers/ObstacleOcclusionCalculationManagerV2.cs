using System.Collections.Generic;
using CoreGame;
using GeometryIntersection;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.Profiling;

namespace Obstacle
{
    public class ObstacleOcclusionCalculationManagerV2 : GameSingleton<ObstacleOcclusionCalculationManagerV2>
    {
        //ObstacleListener -> ObstacleInteractiveObject -> FrustumPositions
        private Dictionary<int, Dictionary<int, List<FrustumPointsPositions>>> CalculatedOcclusionFrustums = new Dictionary<int, Dictionary<int, List<FrustumPointsPositions>>>();
        private Dictionary<ObstacleInteractiveObject, TransformStruct> ObstacleLastFramePositions = new Dictionary<ObstacleInteractiveObject, TransformStruct>();

        private Dictionary<ObstacleListenerSystem, TransformStruct> ObstacleListenerLastFramePositions = new Dictionary<ObstacleListenerSystem, TransformStruct>();

        private List<ObstacleListenerSystem> obstacleListenersThatHasChangedThisFrame = new List<ObstacleListenerSystem>();
        private Dictionary<ObstacleListenerSystem, List<ObstacleInteractiveObject>> singleObstacleThatHasChangedThisFrame = new Dictionary<ObstacleListenerSystem, List<ObstacleInteractiveObject>>();


        private Dictionary<int, Dictionary<int, List<FrustumPointsPositions>>> GetCalculatedOcclusionFrustums()
        {
            if (!JobEnded)
            {
                JobHandle.Complete();
                while (!JobHandle.IsCompleted)
                {
                }

                JobEnded = true;
                OnJobEnded();
            }

            return CalculatedOcclusionFrustums;
        }

        public List<FrustumPointsPositions> GetCalculatedOcclusionFrustums(ObstacleListenerSystem ObstacleListener, ObstacleInteractiveObject obstacleInteractiveObject)
        {
            return GetCalculatedOcclusionFrustums()[ObstacleListener.ObstacleListenerUniqueID][obstacleInteractiveObject.ObstacleInteractiveObjectUniqueID];
        }

        public void TryGetCalculatedOcclusionFrustumsForObstacleListener(ObstacleListenerSystem ObstacleListener, out Dictionary<int, List<FrustumPointsPositions>> calculatedFrustumPositions)
        {
            GetCalculatedOcclusionFrustums().TryGetValue(ObstacleListener.ObstacleListenerUniqueID, out calculatedFrustumPositions);
        }

        public void Tick(float d)
        {
            Profiler.BeginSample("ObstacleOcclusionCalculationManagerV2");
            ManualCalculation(ObstaclesListenerManager.GetAllObstacleListeners(), false);
            Profiler.EndSample();
        }

        public void ManualCalculation(List<ObstacleListenerSystem> ConcernedObstacleListeners, bool forceCalculation)
        {
            var occlusionCalculationCounter = 0;
            var totalFrustumCounter = 0;

            if (!forceCalculation)
            {
                //Position change detection
                foreach (var obstacleListener in ConcernedObstacleListeners)
                {
                    ObstacleListenerLastFramePositions.TryGetValue(obstacleListener, out var lastFramePosition);
                    var hasChanged = !obstacleListener.AssociatedRangeTransformProvider().IsEqualTo(lastFramePosition);

                    //The obstacle listener has changed -> all associated near square obstacles are updated
                    if (hasChanged)
                    {
                        obstacleListenersThatHasChangedThisFrame.Add(obstacleListener);

                        foreach (var obstacleInteractiveObject in obstacleListener.NearSquareObstacles)
                        {
                            occlusionCalculationCounter += 1;
                            totalFrustumCounter += obstacleInteractiveObject.GetFaceFrustums().Count;
                            ClearAndCreateCalculatedFrustums(obstacleListener, obstacleInteractiveObject);
                        }
                    }

                    //The obstacle listener hasn't changed -> we compate near square obstacles positions for update
                    else
                    {
                        singleObstacleThatHasChangedThisFrame.TryGetValue(obstacleListener, out var obstacleInteractiveObjectsThatChanged);
                        if (obstacleInteractiveObjectsThatChanged == null)
                        {
                            singleObstacleThatHasChangedThisFrame.Add(obstacleListener, new List<ObstacleInteractiveObject>());
                            obstacleInteractiveObjectsThatChanged = singleObstacleThatHasChangedThisFrame[obstacleListener];
                        }


                        foreach (var squareObstacle in obstacleListener.NearSquareObstacles)
                        {
                            ObstacleLastFramePositions.TryGetValue(squareObstacle, out var lastFrameSquareObstacleTrasform);
                            //Static obstacles are not detecting changes
                            if (!squareObstacle.SquareObstacleSystemInitializationData.IsStatic
                                && !squareObstacle.GetObstacleCenterTransform().IsEqualTo(lastFrameSquareObstacleTrasform))
                            {
                                //We add this single couple (listener <-> obstacle) to calculation
                                obstacleInteractiveObjectsThatChanged.Add(squareObstacle);
                                occlusionCalculationCounter += 1;
                                totalFrustumCounter += squareObstacle.GetFaceFrustums().Count;
                                ClearAndCreateCalculatedFrustums(obstacleListener, squareObstacle);
                            }
                        }
                    }

                    //Update Obstacle Listener Positions
                    ObstacleListenerLastFramePositions[obstacleListener] = obstacleListener.AssociatedRangeTransformProvider();
                }

                //Update Square Obstacle Positions
                foreach (var obstacleInteractiveObject in obstacleInteractiveObjectManager.AllObstacleInteractiveObjects) ObstacleLastFramePositions[obstacleInteractiveObject] = obstacleInteractiveObject.GetObstacleCenterTransform();
            }
            else
            {
                foreach (var obstacleListener in ConcernedObstacleListeners)
                foreach (var squareObstacle in obstacleListener.NearSquareObstacles)
                {
                    occlusionCalculationCounter += 1;
                    totalFrustumCounter += squareObstacle.GetFaceFrustums().Count;
                }
            }

            if (occlusionCalculationCounter > 0)
            {
                FrustumOcclusionCalculationDatas = new NativeArray<FrustumOcclusionCalculationData>(occlusionCalculationCounter, Allocator.TempJob);
                AssociatedFrustums = new NativeArray<FrustumV2Indexed>(totalFrustumCounter, Allocator.TempJob);
                Results = new NativeArray<FrustumPointsWithInitializedFlag>(totalFrustumCounter, Allocator.TempJob);

                var currentOcclusionCalculationCounter = 0;
                var currentFrustumCounter = 0;

                foreach (var obstacleListenerThatChanged in obstacleListenersThatHasChangedThisFrame)
                foreach (var nearSquareObstacle in obstacleListenerThatChanged.NearSquareObstacles)
                    AddToArrays(ref FrustumOcclusionCalculationDatas, AssociatedFrustums, ref currentOcclusionCalculationCounter, ref currentFrustumCounter, obstacleListenerThatChanged, nearSquareObstacle);

                foreach (var singleObstacleSystemThatChanged in singleObstacleThatHasChangedThisFrame)
                    if (singleObstacleSystemThatChanged.Value.Count > 0)
                        foreach (var nearSquareObstacle in singleObstacleSystemThatChanged.Value)
                            AddToArrays(ref FrustumOcclusionCalculationDatas, AssociatedFrustums, ref currentOcclusionCalculationCounter, ref currentFrustumCounter, singleObstacleSystemThatChanged.Key, nearSquareObstacle);

                var jobHandle = new FrustumOcclusionCalculationJob()
                {
                    AssociatedFrustums = AssociatedFrustums,
                    FrustumOcclusionCalculationDatas = FrustumOcclusionCalculationDatas,
                    Results = Results
                }.Schedule(totalFrustumCounter, 36);

                if (!forceCalculation)
                {
                    JobEnded = false;
                    JobHandle = jobHandle;
                }
                else
                {
                    jobHandle.Complete();
                    while (!jobHandle.IsCompleted)
                    {
                    }

                    OnJobEnded();
                }
            }
            else
            {
                ClearFrameDependantData();
            }
        }

        private void OnJobEnded()
        {
            //Store results
            foreach (var result in Results)
                if (result.Isinitialized)
                    CalculatedOcclusionFrustums[result.FrustumCalculationDataID.ObstacleListenerUniqueID][result.FrustumCalculationDataID.SquareObstacleSystemUniqueID].Add(result.FrustumPointsPositions);

            ClearFrameDependantData();

            if (FrustumOcclusionCalculationDatas.IsCreated) FrustumOcclusionCalculationDatas.Dispose();

            if (AssociatedFrustums.IsCreated) AssociatedFrustums.Dispose();

            if (Results.IsCreated) Results.Dispose();
        }

        private void ClearFrameDependantData()
        {
            //Clear data that changed
            obstacleListenersThatHasChangedThisFrame.Clear();
            foreach (var singleObstacleSystemThatChanged in singleObstacleThatHasChangedThisFrame) singleObstacleSystemThatChanged.Value.Clear();
        }

        private static void AddToArrays(ref NativeArray<FrustumOcclusionCalculationData> FrustumOcclusionCalculationDatas, NativeArray<FrustumV2Indexed> AssociatedFrustums,
            ref int currentOcclusionCalculationCounter, ref int currentFrustumCounter, ObstacleListenerSystem obstacleListenerThatChanged, ObstacleInteractiveObject nearSquareObstacle)
        {
            foreach (var nearSquaureObstacleFrustum in nearSquareObstacle.GetFaceFrustums())
            {
                AssociatedFrustums[currentFrustumCounter] = new FrustumV2Indexed
                {
                    FrustumV2 = nearSquaureObstacleFrustum,
                    CalculationDataIndex = currentOcclusionCalculationCounter
                };
                currentFrustumCounter += 1;
            }

            FrustumOcclusionCalculationDatas[currentOcclusionCalculationCounter] = new FrustumOcclusionCalculationData
            {
                FrustumCalculationDataID = new FrustumCalculationDataID
                {
                    ObstacleListenerUniqueID = obstacleListenerThatChanged.ObstacleListenerUniqueID,
                    SquareObstacleSystemUniqueID = nearSquareObstacle.ObstacleInteractiveObjectUniqueID,
                },
                ObstacleListenerTransform = obstacleListenerThatChanged.AssociatedRangeTransformProvider(),
                SquareObstacleTransform = nearSquareObstacle.GetObstacleCenterTransform()
            };

            currentOcclusionCalculationCounter += 1;
        }

        public void LateTick()
        {
            //We trigger the end of calculations
            GetCalculatedOcclusionFrustums();
        }

        private void ClearAndCreateCalculatedFrustums(ObstacleListenerSystem obstacleListener, ObstacleInteractiveObject obstalceInteractiveObject)
        {
            CalculatedOcclusionFrustums.TryGetValue(obstacleListener.ObstacleListenerUniqueID, out var obstalceFrustumPointsPositions);
            if (obstalceFrustumPointsPositions == null)
            {
                CalculatedOcclusionFrustums.Add(obstacleListener.ObstacleListenerUniqueID, new Dictionary<int, List<FrustumPointsPositions>>());
                obstalceFrustumPointsPositions = CalculatedOcclusionFrustums[obstacleListener.ObstacleListenerUniqueID];
            }

            obstalceFrustumPointsPositions.TryGetValue(obstalceInteractiveObject.ObstacleInteractiveObjectUniqueID, out var squareObstacleFrustumPositions);
            if (squareObstacleFrustumPositions == null)
                obstalceFrustumPointsPositions.Add(obstalceInteractiveObject.ObstacleInteractiveObjectUniqueID, new List<FrustumPointsPositions>());
            else
                squareObstacleFrustumPositions.Clear();
        }

        #region External Dependencies

        private ObstaclesListenerManager ObstaclesListenerManager = ObstaclesListenerManager.Get();
        private ObstacleInteractiveObjectManager obstacleInteractiveObjectManager = ObstacleInteractiveObjectManager.Get();

        #endregion

        #region Native Arrays

        private NativeArray<FrustumOcclusionCalculationData> FrustumOcclusionCalculationDatas;
        private NativeArray<FrustumV2Indexed> AssociatedFrustums;
        private NativeArray<FrustumPointsWithInitializedFlag> Results;

        #endregion

        #region Job State   

        private bool JobEnded;
        private JobHandle JobHandle;

        #endregion
    }

    [BurstCompile]
    public struct FrustumOcclusionCalculationJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<FrustumOcclusionCalculationData> FrustumOcclusionCalculationDatas;

        public NativeArray<FrustumV2Indexed> AssociatedFrustums;
        public NativeArray<FrustumPointsWithInitializedFlag> Results;

        public void Execute(int frustumIndex)
        {
            var FrustumV2Indexed = AssociatedFrustums[frustumIndex];
            var FrustumOcclusionCalculationData = FrustumOcclusionCalculationDatas[FrustumV2Indexed.CalculationDataIndex];
            FrustumV2Indexed.FrustumV2.CalculateFrustumPointsWorldPosByProjection(out var FrustumPointsPositions, out var IsFacing, FrustumOcclusionCalculationData.SquareObstacleTransform, FrustumOcclusionCalculationData.ObstacleListenerTransform.WorldPosition);

            if (IsFacing)
                Results[frustumIndex] = new FrustumPointsWithInitializedFlag
                {
                    Isinitialized = true,
                    FrustumCalculationDataID = FrustumOcclusionCalculationData.FrustumCalculationDataID,
                    FrustumPointsPositions = FrustumPointsPositions
                };
        }
    }

    #region Type Definition

    public struct FrustumV2Indexed
    {
        public FrustumV2 FrustumV2;
        public int CalculationDataIndex;
    }

    public struct FrustumCalculationDataID
    {
        public int ObstacleListenerUniqueID;
        public int SquareObstacleSystemUniqueID;
    }

    public struct FrustumOcclusionCalculationData
    {
        public FrustumCalculationDataID FrustumCalculationDataID;
        public TransformStruct ObstacleListenerTransform;
        public TransformStruct SquareObstacleTransform;
    }

    public struct FrustumPointsWithInitializedFlag
    {
        public bool Isinitialized;
        public FrustumCalculationDataID FrustumCalculationDataID;
        public FrustumPointsPositions FrustumPointsPositions;
    }

    #endregion
}