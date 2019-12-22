using System.Collections.Generic;
using CoreGame;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

public class BeziersMovementJobManager : GameSingleton<BeziersMovementJobManager>
{
    private List<BeziersMovementSystem> AllBeziersMovementSystemsRunning = new List<BeziersMovementSystem>();

    public void OnBeziersMovementSystemCreated(BeziersMovementSystem BeziersMovementSystem)
    {
        this.AllBeziersMovementSystemsRunning.Add(BeziersMovementSystem);
    }

    public void OnBeziersMovementSystemDestroyed(BeziersMovementSystem BeziersMovementSystem)
    {
        this.AllBeziersMovementSystemsRunning.Remove(BeziersMovementSystem);
    }

    private BeziersMovementJob BeziersMovementJob;
    private bool CalculatedThisFrame;
    private JobHandle JobHandle;

    public void SetupJob(float d)
    {
        if (AllBeziersMovementSystemsRunning.Count > 0)
        {
            this.CalculatedThisFrame = false;
            this.BeziersMovementJob = new BeziersMovementJob();
            this.JobHandle = this.BeziersMovementJob.ScheduleJob(d, ref this.AllBeziersMovementSystemsRunning);
        }
    }

    public void EnsureCalculationIsDone()
    {
        if (!this.CalculatedThisFrame)
        {
            this.CalculatedThisFrame = true;
            this.JobHandle.Complete();
            this.BeziersMovementJob.ProcessResults(ref AllBeziersMovementSystemsRunning);
        }
    }
}

[BurstCompile]
public struct BeziersMovementJob : IJobParallelFor
{
    [ReadOnly] public float DeltaTime;
    private NativeArray<BeziersMovementPositionState> BeziersMovementPositionStates;

    public JobHandle ScheduleJob(float d, ref List<BeziersMovementSystem> BeziersMovementSystems)
    {
        this.DeltaTime = d;
        this.BeziersMovementPositionStates = new NativeArray<BeziersMovementPositionState>(BeziersMovementSystems.Count, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        for (var i = 0; i < BeziersMovementSystems.Count; i++)
        {
            BeziersMovementSystems[i].ScheduleJob(ref this.BeziersMovementPositionStates, i);
        }

        return this.Schedule(this.BeziersMovementPositionStates.Length, 10);
    }

    public void Execute(int index)
    {
        this.BeziersMovementPositionStates[index] = this.BeziersMovementPositionStates[index].JOB_TickEvaluate(this.DeltaTime);
    }

    public void ProcessResults(ref List<BeziersMovementSystem> BeziersMovementSystems)
    {
        for (var i = 0; i < BeziersMovementSystems.Count; i++)
        {
            BeziersMovementSystems[i].ProcessResults(ref this.BeziersMovementPositionStates, i);
        }

        this.BeziersMovementPositionStates.Dispose();
    }
}