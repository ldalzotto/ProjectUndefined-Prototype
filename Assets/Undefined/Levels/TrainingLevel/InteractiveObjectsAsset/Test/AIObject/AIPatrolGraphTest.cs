using System;
using System.Collections.Generic;
using AIObjects;
using InteractiveObjects;
using SequencedAction;
using SequencedAction_Editor_Common;
using UnityEngine;

namespace RTPuzzle
{
    [Serializable]
    [CreateAssetMenu(fileName = "AIPatrolGraphTest", menuName = "Test/AIPatrolGraphTest", order = 1)]
    [SceneHandleDraw]
    public class AIPatrolGraphTest : AIPatrolGraphV2
    {
        [WireCircleWorldAttribute(R = 0f, G = 1f, B = 0f, UseTransform = false, PositionFieldName = nameof(AIPatrolGraphTest.P1), Radius = 1f)]
        [WireArrow(R = 0f, G = 1f, B = 0f, OriginFieldName = nameof(AIPatrolGraphTest.P1))]
        [WireArrowLink(SourceFieldName = nameof(AIPatrolGraphTest.P1), TargetFieldName = nameof(AIPatrolGraphTest.P3))]
        [MultiplePointMovementNested]
        public AIMoveToActionInputData P1;

        [WireCircleWorldAttribute(UseTransform = false, PositionFieldName = nameof(AIPatrolGraphTest.P2), Radius = 1f)]
        [WireArrow(OriginFieldName = nameof(AIPatrolGraphTest.P2))]
        [WireArrowLink(SourceFieldName = nameof(AIPatrolGraphTest.P2), TargetFieldName = nameof(AIPatrolGraphTest.P1))]
        [MultiplePointMovementNested]
        public AIMoveToActionInputData P2;

        [WireCircleWorldAttribute(UseTransform = false, PositionFieldName = nameof(AIPatrolGraphTest.P3), Radius = 1f)]
        [WireArrow(OriginFieldName = nameof(AIPatrolGraphTest.P3))]
        [WireArrowLink(SourceFieldName = nameof(AIPatrolGraphTest.P3), TargetFieldName = nameof(AIPatrolGraphTest.P2))]
        [MultiplePointMovementNested]
        public AIMoveToActionInputData P3;

        public override List<ASequencedAction> AIPatrolGraphActions(CoreInteractiveObject InvolvedInteractiveObject)
        {
            return new List<ASequencedAction>()
            {
                new AIWarpActionV2(InvolvedInteractiveObject, this.P1.WorldPosition, this.P1.GetWorldRotation(), () => new List<ASequencedAction>()
                {
                    new BranchInfiniteLoopAction(
                        new List<ASequencedAction>()
                        {
                            this.CreateAIMoveToActionV2(InvolvedInteractiveObject, this.P2, () => new List<ASequencedAction>()
                            {
                                this.CreateAIMoveToActionV2(InvolvedInteractiveObject, this.P1, () => new List<ASequencedAction>()
                                {
                                    this.CreateAIMoveToActionV2(InvolvedInteractiveObject, this.P3, null)
                                })
                            })
                        }
                    )
                })
            };
        }
    }
}