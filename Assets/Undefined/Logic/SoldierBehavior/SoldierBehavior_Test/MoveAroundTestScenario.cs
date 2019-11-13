using System;
using System.Collections.Generic;
using AIObjects;
using PlayerObject;
using SequencedAction;
using Tests.TestScenario;
using UnityEngine;

namespace SoliderBehavior_Test
{
    [SceneHandleDraw]
    public class MoveAroundTestScenario : ATestScenarioDefinition
    {
        [WireCircleWorld(PositionFieldName = nameof(MoveAroundTestScenario.Spawn))]
        public Vector3 Spawn;

        public override List<ASequencedAction> BuildScenarioActions()
        {
            var PlayerInteractiveObject = PlayerInteractiveObjectManager.Get().PlayerInteractiveObject;
            return new List<ASequencedAction>()
            {
                new AIWarpActionV2(PlayerInteractiveObject, Spawn, default(Nullable<Vector3>), null)
            };
        }
    }
}