using System;
using CoreGame;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

public class BeziersMovementSystem
{
    private BeziersMovementPositionState BeziersMovementPositionState;
    private BeziersControlPointsBuildInput BeziersControlPointsBuildInput;

    private bool isCurrentlyMoving;

    #region Events Callback

    private event Action<BeziersMovementSystem> OnMovementStartEvent;
    private event Action<BeziersMovementSystem> OnMovementEndEvent;

    #endregion

    public BeziersMovementSystem(BeziersControlPointsBuildInput BeziersControlPointsBuildInput, Action<BeziersMovementSystem> OnMovementStop = null)
    {
        this.BeziersControlPointsBuildInput = BeziersControlPointsBuildInput;
        this.OnMovementEndEvent += OnMovementStop;
        BeziersMovementJobManager.Get().OnBeziersMovementSystemCreated(this);
    }

    public void Tick(float d)
    {
        if (this.isCurrentlyMoving)
        {
            BeziersMovementJobManager.Get().EnsureCalculationIsDone();
            if (this.BeziersMovementPositionState.MovementEnded)
            {
                this.StopBeziersMovement();
            }
        }
    }

    public void Destroy()
    {
        BeziersMovementJobManager.Get().OnBeziersMovementSystemDestroyed(this);
    }

    public void StartBeziersMovement()
    {
        this.BeziersMovementPositionState = new BeziersMovementPositionState(new BeziersControlPoints(this.BeziersControlPointsBuildInput), this.BeziersControlPointsBuildInput.Speed);
        this.isCurrentlyMoving = true;
        this.OnMovementStartEvent?.Invoke(this);
    }

    public void StopBeziersMovement()
    {
        this.isCurrentlyMoving = false;
        this.OnMovementEndEvent?.Invoke(this);
    }

    #region Event Register

    public void RegisterOnMovementStartEvent(Action<BeziersMovementSystem> action)
    {
        this.OnMovementStartEvent += action;
    }

    public void UnRegisterOnMovementStartEvent(Action<BeziersMovementSystem> action)
    {
        this.OnMovementStartEvent -= action;
    }

    public void RegisterOnMovementEndEvent(Action<BeziersMovementSystem> action)
    {
        this.OnMovementEndEvent += action;
    }

    public void UnRegisterOnMovementEndEvent(Action<BeziersMovementSystem> action)
    {
        this.OnMovementEndEvent -= action;
    }

    #endregion

    #region Job Related

    public void ScheduleJob(ref NativeArray<BeziersMovementPositionState> BeziersMovementPositionStates, int index)
    {
        BeziersMovementPositionStates[index] = this.BeziersMovementPositionState;
    }

    public void ProcessResults(ref NativeArray<BeziersMovementPositionState> beziersMovementPositionStates, int i)
    {
        this.BeziersMovementPositionState = beziersMovementPositionStates[i];
    }

    #endregion

    #region Data Retrieval

    public Vector3 GetBeziersPathPosition()
    {
        return this.BeziersMovementPositionState.CurrentPosition;
    }

    #endregion
}

[Serializable]
public struct BeziersMovementPositionState
{
    public BeziersControlPoints BeziersControlPoints;
    private float Speed;
    public Vector3 CurrentPosition;
    private float CurrentTime;
    public bool MovementEnded;

    public BeziersMovementPositionState(BeziersControlPoints beziersControlPoints, float Speed)
    {
        BeziersControlPoints = beziersControlPoints;
        this.Speed = Speed;
        CurrentTime = 0f;
        this.CurrentPosition = this.BeziersControlPoints.ResolvePoint(this.CurrentTime);
        this.MovementEnded = false;
    }

    public BeziersMovementPositionState JOB_TickEvaluate(float d)
    {
        this.CurrentTime = Mathf.Min(this.CurrentTime + (d * this.Speed), 1f);
        this.MovementEnded = this.CurrentTime == 1f;
        this.CurrentPosition = this.BeziersControlPoints.ResolvePoint(this.CurrentTime);
        return this;
    }
}