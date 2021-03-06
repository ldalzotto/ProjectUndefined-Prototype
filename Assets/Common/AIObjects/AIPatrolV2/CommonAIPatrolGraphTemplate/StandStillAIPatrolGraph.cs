﻿using System;
using System.Collections.Generic;
using InteractiveObjects;
using SequencedAction;
using SequencedAction_Editor_Common;

namespace AIObjects
{
    /// <summary>
    /// The default <see cref="AIPatrolGraphV2"/>
    /// </summary>
    [Serializable]
    [SceneHandleDraw]
    public class StandStillAIPatrolGraph : AIPatrolGraphV2
    {
        [WireCircleWorldAttribute(R = 0f, G = 1f, B = 0f, UseTransform = false, PositionFieldName = nameof(StandingStillPoint), Radius = 1f)] //
        [WireArrow(R = 0f, G = 1f, B = 0f, OriginFieldName = nameof(StandingStillPoint))]
        [MultiplePointMovementNested]
        public AIMoveToActionInputData StandingStillPoint;

        public override ASequencedAction[] AIPatrolGraphActions(CoreInteractiveObject InvolvedInteractiveObject, AIPatrolGraphRuntimeCallbacks AIPatrolGraphRuntimeCallbacks)
        {
            return new ASequencedAction[]
            {
                CreateAIWarpAction(InvolvedInteractiveObject, this.StandingStillPoint)
                    .Then(new AIWarpActionV2(InvolvedInteractiveObject, this.StandingStillPoint.WorldPosition, this.StandingStillPoint.GetWorldRotation())
                        .Then(new BranchInfiniteLoopAction(CreateAIMoveToActionV2(this.StandingStillPoint, AIPatrolGraphRuntimeCallbacks))))
            };
        }
    }
}