﻿using System;
using System.Collections;
using System.Collections.Generic;
using AIObjects;
using InteractiveObjects;
using RTPuzzle;
using SequencedAction;
using SequencedAction_Editor_Common;
using UnityEngine;

[Serializable]
[SceneHandleDraw]
public class SoliderEnemyDefinition_F_PatrollingPath : AIPatrolGraphV2
{
    [WireCircleWorldAttribute(R = 0f, G = 1f, B = 0f, UseTransform = false, PositionFieldName = nameof(P1), Radius = 1f)] [WireArrow(R = 0f, G = 1f, B = 0f, OriginFieldName = nameof(P1))] [WireArrowLink(SourceFieldName = nameof(P1), TargetFieldName = nameof(P2))] [MultiplePointMovementNested]
    public AIMoveToActionInputData P1;

    [WireCircleWorldAttribute(UseTransform = false, PositionFieldName = nameof(P2), Radius = 1f)] [WireArrow(OriginFieldName = nameof(P2))] [WireArrowLink(SourceFieldName = nameof(P2), TargetFieldName = nameof(P3))] [MultiplePointMovementNested]
    public AIMoveToActionInputData P2;

    [WireCircleWorldAttribute(UseTransform = false, PositionFieldName = nameof(P3), Radius = 1f)] [WireArrow(OriginFieldName = nameof(P3))] [WireArrowLink(SourceFieldName = nameof(P3), TargetFieldName = nameof(P4))] [MultiplePointMovementNested]
    public AIMoveToActionInputData P3;

    [WireCircleWorldAttribute(UseTransform = false, PositionFieldName = nameof(P4), Radius = 1f)] [WireArrow(OriginFieldName = nameof(P4))] [WireArrowLink(SourceFieldName = nameof(P4), TargetFieldName = nameof(P1))] [MultiplePointMovementNested]
    public AIMoveToActionInputData P4;

    public override List<ASequencedAction> AIPatrolGraphActions(CoreInteractiveObject InvolvedInteractiveObject, AIPatrolGraphRuntimeCallbacks AIPatrolGraphRuntimeCallbacks)
    {
        return new List<ASequencedAction>()
        {
            new AIWarpActionV2(InvolvedInteractiveObject, this.P1.WorldPosition, this.P1.GetWorldRotation(), () => new List<ASequencedAction>()
            {
                new BranchInfiniteLoopAction(
                    new List<ASequencedAction>()
                    {
                        this.CreateAIMoveToActionV2(this.P2, AIPatrolGraphRuntimeCallbacks, () => new List<ASequencedAction>()
                        {
                            this.CreateAIMoveToActionV2(this.P3, AIPatrolGraphRuntimeCallbacks, () => new List<ASequencedAction>()
                            {
                                this.CreateAIMoveToActionV2(this.P4, AIPatrolGraphRuntimeCallbacks, () => new List<ASequencedAction>()
                                {
                                    this.CreateAIMoveToActionV2(this.P1, AIPatrolGraphRuntimeCallbacks, null)
                                })
                            })
                        })
                    }
                )
            })
        };
    }
}