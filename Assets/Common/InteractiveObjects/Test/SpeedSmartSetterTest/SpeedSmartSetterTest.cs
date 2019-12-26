using System;
using System.Collections.Generic;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using UnityEngine;


public class SpeedSmartSetterTest : MonoBehaviour
{
    private void Start()
    {
    }
}

public enum TestSpeedAttenuationState
{
    DEFAULT_NOTHING,
    PATROLLING
}

public class TestSpeedAttenuationSmartSetterStateBehavior : SpeedAttenuationSmartSetterStateBehavior<TestSpeedAttenuationState, SpeedAttenuationStateManager>
{
    public TestSpeedAttenuationSmartSetterStateBehavior()
    {
        this.StateManagersLookup = new Dictionary<TestSpeedAttenuationState, SpeedAttenuationStateManager>()
        {
            {TestSpeedAttenuationState.DEFAULT_NOTHING, new SpeedAttenuationStateManager(new SpeedAttenuationLayer(new NoneSpeedAttenuationConstraint(), AIMovementSpeedAttenuationFactor.ZERO))},
            {TestSpeedAttenuationState.PATROLLING, new SpeedAttenuationStateManager(new SpeedAttenuationLayer(new NoneSpeedAttenuationConstraint(), AIMovementSpeedAttenuationFactor.WALK))}
        };
    }
}

#region Generic Definition

public class SpeedAttenuationStateManager : StateManager
{
    public SpeedAttenuationLayer SpeedAttenuationLayer { get; private set; }

    public SpeedAttenuationStateManager(SpeedAttenuationLayer SpeedAttenuationLayer)
    {
        this.SpeedAttenuationLayer = SpeedAttenuationLayer;
    }
}

public abstract class SpeedAttenuationSmartSetterStateBehavior<S, SM> : StateBehavior<S, SM> where S : Enum where SM : SpeedAttenuationStateManager
{
}

public struct SpeedAttenuationLayer
{
    private IObjectSpeedAttenuationConstraint SpeedAttenuationConstraint;
    private AIMovementSpeedAttenuationFactor AIMovementSpeedAttenuationFactor;

    public SpeedAttenuationLayer(IObjectSpeedAttenuationConstraint speedAttenuationConstraint, AIMovementSpeedAttenuationFactor aiMovementSpeedAttenuationFactor)
    {
        SpeedAttenuationConstraint = speedAttenuationConstraint;
        AIMovementSpeedAttenuationFactor = aiMovementSpeedAttenuationFactor;
    }
}

#endregion