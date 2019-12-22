using System;
using System.Collections;
using System.Collections.Generic;
using CoreGame;
using CoreGame.System;
using Unity.Collections;
using UnityEngine;

public class BeziersMovementSystemV2
{
    private BeziersMovementPositionState BeziersMovementPositionState;
    private BeziersControlPointsBuildInput BeziersControlPointsBuildInput;

    private BoolVariable isCurrentlyMoving;

    public BeziersMovementSystemV2(BeziersControlPointsBuildInput BeziersControlPointsBuildInput, Action OnMovementStop = null)
    {
        this.BeziersControlPointsBuildInput = BeziersControlPointsBuildInput;
        this.isCurrentlyMoving = new BoolVariable(false, onJustSetToFalse: OnMovementStop);
    }

    public void Tick(float d)
    {
        if (this.isCurrentlyMoving.GetValue())
        {
            this.BeziersMovementPositionState.JOB_TickEvaluate(d);
            if (this.BeziersMovementPositionState.MovementEnded)
            {
                this.StopBeziersMovement();
            }
        }
    }

    public void StartBeziersMovement()
    {
        this.BeziersMovementPositionState = new BeziersMovementPositionState(new BeziersControlPoints(this.BeziersControlPointsBuildInput), this.BeziersControlPointsBuildInput.Speed);
        this.isCurrentlyMoving.SetValue(true);
    }

    public void StopBeziersMovement()
    {
        this.isCurrentlyMoving.SetValue(false);
    }

    #region Logical Condition

    public bool IsCurrentlyMoving()
    {
        return this.isCurrentlyMoving.GetValue();
    }

    #endregion

    #region Data Retrieval

    public Vector3 GetBeziersPathPosition()
    {
        return this.BeziersMovementPositionState.CurrentPosition;
    }

    #endregion
}