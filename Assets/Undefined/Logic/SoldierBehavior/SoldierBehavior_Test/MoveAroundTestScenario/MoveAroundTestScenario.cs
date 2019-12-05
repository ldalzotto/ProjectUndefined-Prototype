using System;
using System.Collections.Generic;
using AIObjects;
using PlayerObject;
using SequencedAction;
using Tests;
using Tests.TestScenario;
using UnityEngine;

namespace SoliderBehavior_Test
{
    [SceneHandleDraw]
    public class MoveAroundTestScenario : ATestScenarioDefinition
    {
        [WireCircleWorld(PositionFieldName = nameof(Spawn))] [WireArrowLink(SourceFieldName = nameof(Spawn), TargetFieldName = nameof(P1))]
        public Vector3 Spawn;

        [WireCircleWorld(PositionFieldName = nameof(P1))] [WireArrow(OriginFieldName = nameof(P1))] [WireArrowLink(SourceFieldName = nameof(P1), TargetFieldName = nameof(P2))]
        public AIMoveToActionInputData P1;

        [WireCircleWorld(PositionFieldName = nameof(P2))] [WireArrow(OriginFieldName = nameof(P2))] [WireArrowLink(SourceFieldName = nameof(P2), TargetFieldName = nameof(P3))]
        public AIMoveToActionInputData P2;
        [WireCircleWorld(PositionFieldName = nameof(P3))] [WireArrow(OriginFieldName = nameof(P3))]
        public AIMoveToActionInputData P3;

        public override List<ASequencedAction> BuildScenarioActions()
        {
            var PlayerInteractiveObject = PlayerInteractiveObjectManager.Get().PlayerInteractiveObject;
            return new List<ASequencedAction>()
            {
                new AIWarpActionV2(PlayerInteractiveObject, Spawn, default(Nullable<Vector3>), () => new List<ASequencedAction>()
                {
                    new AIMoveToActionV2(PlayerInteractiveObject, this.P1.WorldPosition, this.P1.GetWorldRotation(), P1.AIMovementSpeed, () => new List<ASequencedAction>()
                    {
                        new AIMoveToActionV2(PlayerInteractiveObject, this.P2.WorldPosition, P2.GetWorldRotation(), P2.AIMovementSpeed, () => new List<ASequencedAction>()
                        {
                            new AIMoveToActionV2(PlayerInteractiveObject, this.P3.WorldPosition, P3.GetWorldRotation(), P3.AIMovementSpeed, () => new List<ASequencedAction>()
                            {
                                new AIWarpActionV2(PlayerInteractiveObject, Vector3.zero, null, null)
                            })
                        })
                    })
                })
            };
        }
    }
}