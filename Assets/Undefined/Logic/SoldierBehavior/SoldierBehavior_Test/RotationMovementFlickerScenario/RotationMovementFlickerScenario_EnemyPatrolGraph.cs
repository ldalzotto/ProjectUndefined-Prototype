﻿using System;
using System.Collections.Generic;
using AIObjects;
using InteractiveObjects;
using SequencedAction;
using SequencedAction_Editor_Common;

namespace SoliderBehavior_Test
{
    [SceneHandleDraw]
    [Serializable]
    public class RotationMovementFlickerScenario_EnemyPatrolGraph : AIPatrolGraphV2
    {
        [WireCircleWorldAttribute(R = 0f, G = 1f, B = 0f, UseTransform = false, PositionFieldName = nameof(P1), Radius = 1f)] [WireArrow(R = 0f, G = 1f, B = 0f, OriginFieldName = nameof(P1))] [WireArrowLink(SourceFieldName = nameof(P1), TargetFieldName = nameof(P2))] [MultiplePointMovementNested]
        public AIMoveToActionInputData P1;

        public float P1_SecondsToWait;

        [WireCircleWorldAttribute(UseTransform = false, PositionFieldName = nameof(P2), Radius = 1f)] [WireArrow(OriginFieldName = nameof(P2))] [WireArrowLink(SourceFieldName = nameof(P2), TargetFieldName = nameof(P1))] [MultiplePointMovementNested]
        public AIMoveToActionInputData P2;

        public float P2_SecondsToWait;

        public override List<ASequencedAction> AIPatrolGraphActions(CoreInteractiveObject InvolvedInteractiveObject, AIPatrolGraphRuntimeCallbacks AIPatrolGraphRuntimeCallbacks)
        {
            return new List<ASequencedAction>()
            {
                new AIWarpActionV2(InvolvedInteractiveObject, this.P1.WorldPosition, this.P1.GetWorldRotation(), () => new List<ASequencedAction>()
                {
                    new BranchInfiniteLoopAction(
                        new List<ASequencedAction>()
                        {
                            new WaitForSecondsAction(this.P1_SecondsToWait, () => new List<ASequencedAction>()
                                {
                                    this.CreateAIMoveToActionV2(this.P2, AIPatrolGraphRuntimeCallbacks, () => new List<ASequencedAction>()
                                    {
                                        new WaitForSecondsAction(this.P2_SecondsToWait, () => new List<ASequencedAction>()
                                        {
                                            this.CreateAIMoveToActionV2(this.P1, AIPatrolGraphRuntimeCallbacks, null)
                                        })
                                    })
                                }
                            )
                        }
                    )
                })
            };
        }
    }
}