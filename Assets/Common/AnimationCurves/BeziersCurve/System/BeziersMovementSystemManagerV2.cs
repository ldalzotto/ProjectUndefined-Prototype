using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;

namespace CoreGame.System
{
    public class BeziersMovementSystemManagerV2 : GameSingleton<BeziersMovementSystemManagerV2>
    {
        private Dictionary<int, BeziersMovementSystem> BeziersMovementSystemsForResultProcessing;
        private BeziersMovementPositionCalculationsJob BeziersMovementPositionCalculationsJob;

        private JobHandle BeziersMovementPositionCalculationsJobhandle;

        public BeziersMovementSystemManagerV2()
        {
            this.BeziersMovementSystemsForResultProcessing = new Dictionary<int, BeziersMovementSystem>();
            this.BeziersMovementPositionCalculationsJob = new BeziersMovementPositionCalculationsJob();
        }

        public void OnBeziersMovementSystemCreated(BeziersMovementSystem BeziersMovementSystem)
        {
            this.ProcessJobResults();
            this.BeziersMovementSystemsForResultProcessing.Add(BeziersMovementSystem.UniqueBeziersMovementSystemID, BeziersMovementSystem);
        }

        public void OnBeziersMovementSystemDestroyed(BeziersMovementSystem BeziersMovementSystem)
        {
            this.ProcessJobResults();
            if (BeziersMovementSystem.IsCurrentlyMoving())
            {
                this.OnBeziersMovementStopped(BeziersMovementSystem);
            }

            this.BeziersMovementSystemsForResultProcessing.Remove(BeziersMovementSystem.UniqueBeziersMovementSystemID);
        }

        public void OnBeziersMovementStarted(BeziersMovementSystem BeziersMovementSystem, BeziersMovementPositionState IntiialBeziersMovementPositionState)
        {
            this.ProcessJobResults();
            this.BeziersMovementPositionCalculationsJob.OnBeziersMovementStarted(BeziersMovementSystem, IntiialBeziersMovementPositionState);
        }

        public void OnBeziersMovementStopped(BeziersMovementSystem BeziersMovementSystem)
        {
            this.ProcessJobResults();
            this.BeziersMovementPositionCalculationsJob.OnBeziersMovementStopped(BeziersMovementSystem);
        }

        public void SetupJob(float d)
        {
            this.BeziersMovementPositionCalculationsJobhandle = this.BeziersMovementPositionCalculationsJob.ScheduleJob(d);
        }

        public void AfterTick(float d)
        {
            this.ProcessJobResults();
        }

        private void ProcessJobResults()
        {
            if (!this.BeziersMovementPositionCalculationsJobhandle.IsCompleted)
            {
                this.BeziersMovementPositionCalculationsJobhandle.Complete();

                List<BeziersMovementSystem> EndedBeziersMovement = null;

                if (BeziersMovementPositionCalculationsJob.BeziersMovementPositionCalculationKeys.IsCreated)
                {
                    foreach (var beziersMovementPositionCalculationKey in BeziersMovementPositionCalculationsJob.BeziersMovementPositionCalculationKeys)
                    {
                        var beziersMovementSystem = this.BeziersMovementSystemsForResultProcessing[beziersMovementPositionCalculationKey];
                        beziersMovementSystem.ProcessBeziersMovementPositionCalculationsJobResult();
                        if (this.GetBeziersMovementPositionState(beziersMovementSystem).MovementEnded)
                        {
                            if (EndedBeziersMovement == null)
                            {
                                EndedBeziersMovement = new List<BeziersMovementSystem>();
                            }

                            EndedBeziersMovement.Add(beziersMovementSystem);
                        }
                    }
                }

                if (EndedBeziersMovement != null)
                {
                    foreach (var beziersMovementSystem in EndedBeziersMovement)
                    {
                        beziersMovementSystem.StopBeziersMovement();
                    }
                }
            }
        }

        public override void OnDestroy()
        {
            this.BeziersMovementPositionCalculationsJob.DeAllocate();
            base.OnDestroy();
        }

        private BeziersMovementPositionState GetBeziersMovementPositionState(BeziersMovementSystem BeziersMovementSystem)
        {
            return this.BeziersMovementPositionCalculationsJob.BeziersMovementPositionCalculations[BeziersMovementSystem.UniqueBeziersMovementSystemID];
        }


        #region Data Retrieval

        public Vector3 GetCurrentPosition(BeziersMovementSystem BeziersMovementSystem)
        {
            this.ProcessJobResults();
            return this.BeziersMovementPositionCalculationsJob.BeziersMovementPositionCalculations[BeziersMovementSystem.UniqueBeziersMovementSystemID].CurrentPosition;
        }

        #endregion
    }

    [BurstCompile]
    struct BeziersMovementPositionCalculationsJob : IJobParallelFor
    {
        [ReadOnly] public NativeList<int> BeziersMovementPositionCalculationKeys;
        public UnsafeHashMap<int, BeziersMovementPositionState> BeziersMovementPositionCalculations;
        [ReadOnly] public float DeltaTime;

        public JobHandle ScheduleJob(float d)
        {
            this.DeltaTime = d;
            if (this.BeziersMovementPositionCalculationKeys.IsCreated)
            {
                if (this.BeziersMovementPositionCalculationKeys.Length > 0)
                {
                    return this.Schedule(BeziersMovementPositionCalculationKeys.Length, 10);
                }
            }

            return default;
        }

        public void Execute(int index)
        {
            var involvedBeziersMovementPositionState = this.BeziersMovementPositionCalculations[this.BeziersMovementPositionCalculationKeys[index]];
            involvedBeziersMovementPositionState.JOB_TickEvaluate(this.DeltaTime);
            this.BeziersMovementPositionCalculations[this.BeziersMovementPositionCalculationKeys[index]] = involvedBeziersMovementPositionState;
        }

        public void OnBeziersMovementStarted(BeziersMovementSystem BeziersMovementSystem, BeziersMovementPositionState InitialBeziersMovementPositionState)
        {
            if (!this.BeziersMovementPositionCalculationKeys.IsCreated)
            {
                this.Allocate();
            }

            this.BeziersMovementPositionCalculationKeys.Add(BeziersMovementSystem.UniqueBeziersMovementSystemID);
            this.BeziersMovementPositionCalculations.Add(BeziersMovementSystem.UniqueBeziersMovementSystemID, InitialBeziersMovementPositionState);
        }

        public void OnBeziersMovementStopped(BeziersMovementSystem beziersMovementSystem)
        {
            this.BeziersMovementPositionCalculationKeys.RemoveAtSwapBack(this.BeziersMovementPositionCalculationKeys.IndexOf(beziersMovementSystem.UniqueBeziersMovementSystemID));
            this.BeziersMovementPositionCalculations.Remove(beziersMovementSystem.UniqueBeziersMovementSystemID);

            if (this.BeziersMovementPositionCalculationKeys.Length == 0)
            {
                this.DeAllocate();
            }
        }

        public void Allocate()
        {
            this.BeziersMovementPositionCalculationKeys = new NativeList<int>(0, Allocator.Persistent);
            this.BeziersMovementPositionCalculations = new UnsafeHashMap<int, BeziersMovementPositionState>(0, Allocator.Persistent);
        }

        public void DeAllocate()
        {
            if (this.BeziersMovementPositionCalculations.IsCreated)
            {
                this.BeziersMovementPositionCalculations.Dispose();
            }

            if (this.BeziersMovementPositionCalculationKeys.IsCreated)
            {
                this.BeziersMovementPositionCalculationKeys.Dispose();
            }
        }
    }
}