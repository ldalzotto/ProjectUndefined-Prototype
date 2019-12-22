using System;
using System.Collections;
using System.Collections.Generic;
using CoreGame;
using CoreGame.System;
using Unity.Collections;
using UnityEngine;

public class BeziersMovementSystem
{
    public static int UniqueBeziersMovementSystemIDCounter = 0;
    public int UniqueBeziersMovementSystemID { get; private set; }

    private BeziersControlPointsBuildInput BeziersControlPointsBuildInput;

    private Vector3 CurrentBeziersPathPosition;

    public BeziersMovementSystem(BeziersControlPointsBuildInput BeziersControlPointsBuildInput)
    {
        this.UniqueBeziersMovementSystemID = UniqueBeziersMovementSystemIDCounter;
        UniqueBeziersMovementSystemIDCounter += 1;
        this.BeziersControlPointsBuildInput = BeziersControlPointsBuildInput;
        BeziersMovementSystemManagerV2.Get().OnBeziersMovementSystemCreated(this);
    }

    private bool isCurrentlyMoving;

    public void Destroy()
    {
        BeziersMovementSystemManagerV2.Get().OnBeziersMovementSystemDestroyed(this);
    }

    public void StartBeziersMovement()
    {
        BeziersMovementSystemManagerV2.Get().OnBeziersMovementStarted(this, new BeziersMovementPositionState(new BeziersControlPoints(this.BeziersControlPointsBuildInput), 3));
        this.ProcessBeziersMovementPositionCalculationsJobResult();
        this.isCurrentlyMoving = true;
    }

    public void StopBeziersMovement()
    {
        this.isCurrentlyMoving = false;
        BeziersMovementSystemManagerV2.Get().OnBeziersMovementStopped(this);
        this.Destroy();
    }

    public void ProcessBeziersMovementPositionCalculationsJobResult()
    {
        this.CurrentBeziersPathPosition = BeziersMovementSystemManagerV2.Get().GetCurrentPosition(this);
    }

    #region Logical Condition

    public bool IsCurrentlyMoving()
    {
        return this.isCurrentlyMoving;
    }

    #endregion

    #region Data Retrieval

    public Vector3 GetBeziersPathPosition()
    {
        return this.CurrentBeziersPathPosition;
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

    public void JOB_TickEvaluate(float d)
    {
        this.CurrentTime = Mathf.Min(this.CurrentTime + (d * this.Speed), 1f);
        this.MovementEnded = this.CurrentTime == 1f;
        this.CurrentPosition = this.BeziersControlPoints.ResolvePoint(this.CurrentTime);
    }
}