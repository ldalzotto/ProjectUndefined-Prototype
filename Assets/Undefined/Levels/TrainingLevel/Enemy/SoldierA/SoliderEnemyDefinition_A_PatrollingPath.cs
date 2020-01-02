using System;
using AIObjects;
using InteractiveObjects;
using SequencedAction;
using SequencedAction_Editor_Common;

[Serializable]
[SceneHandleDraw]
public class SoliderEnemyDefinition_A_PatrollingPath : AIPatrolGraphV2
{
    [WireCircleWorldAttribute(R = 0f, G = 1f, B = 0f, UseTransform = false, PositionFieldName = nameof(P1), Radius = 1f)] //
    [WireArrow(R = 0f, G = 1f, B = 0f, OriginFieldName = nameof(P1))]
    [WireArrowLink(SourceFieldName = nameof(P1), TargetFieldName = nameof(P2))]
    [MultiplePointMovementNested]
    public AIMoveToActionInputData P1;

    public float P1_SecondsToWait;

    [WireCircleWorldAttribute(UseTransform = false, PositionFieldName = nameof(P2), Radius = 1f)] //
    [WireArrow(OriginFieldName = nameof(P2))]
    [WireArrowLink(SourceFieldName = nameof(P2), TargetFieldName = nameof(P1))]
    [MultiplePointMovementNested]
    public AIMoveToActionInputData P2;

    public float P2_SecondsToWait;

    public override ASequencedAction[] AIPatrolGraphActions(CoreInteractiveObject InvolvedInteractiveObject, AIPatrolGraphRuntimeCallbacks AIPatrolGraphRuntimeCallbacks)
    {
        return new ASequencedAction[]
        {
            this.CreateAIWarpAction(InvolvedInteractiveObject, this.P1)
                .Then(new BranchInfiniteLoopAction(
                        new WaitForSecondsAction(this.P1_SecondsToWait)
                            .Then(this.CreateAIMoveToActionV2(this.P2, AIPatrolGraphRuntimeCallbacks)
                                .Then(new WaitForSecondsAction(this.P2_SecondsToWait)
                                    .Then(this.CreateAIMoveToActionV2(this.P1, AIPatrolGraphRuntimeCallbacks))
                                )
                            )
                    )
                )
        };
    }
}