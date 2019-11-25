using System;
using System.Collections.Generic;
using AIObjects;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using PlayerObject;
using SequencedAction;
using Targetting_Test;
using Tests.TestScenario;
using UnityEngine;

namespace SoliderBehavior_Test
{
    [Serializable]
    [SceneHandleDraw]
    public class TrackUnknownScenario : ATestScenarioDefinition
    {
        private const string EnemyObjectName = "Enemy";

        private CoreInteractiveObject EnemyObject;
        private CoreInteractiveObject PlayerObject;

        [WireCircleWorld(PositionFieldName = nameof(Fire1PlayerPosition))]
        public Vector3 Fire1PlayerPosition;

        public override List<ASequencedAction> BuildScenarioActions()
        {
            this.EnemyObject = InteractiveObjectV2Manager.Get().InteractiveObjects.Find(o => o.InteractiveGameObject.GetAssociatedGameObjectName() == EnemyObjectName);

            this.PlayerObject = PlayerInteractiveObjectManager.Get().PlayerInteractiveObject;

            return new List<ASequencedAction>()
            {
                new AIWarpActionV2(this.PlayerObject, this.Fire1PlayerPosition, null, () => new List<ASequencedAction>()
                {
                    new TargetAndFireToInteractiveObjectAction(this.EnemyObject, () => new List<ASequencedAction>()
                    {
                        new AIMoveToActionV2(this.PlayerObject, Vector3.zero, null, AIMovementSpeedDefinition.RUN, () => new List<ASequencedAction>()
                        {
                            BuildWarpPlayerBehindEnemyAction(() => new List<ASequencedAction>()
                            {
                                new TargetAndFireToInteractiveObjectAction(this.EnemyObject, () => new List<ASequencedAction>()
                                {
                                    new AIWarpActionV2(this.PlayerObject, Vector3.zero, null , null)
                                })
                            })
                        })
                    })
                })
            };
        }

        private AIWarpActionV2 BuildWarpPlayerBehindEnemyAction(Func<List<ASequencedAction>> nextActionsDeferred)
        {
            var enemyTransform = this.EnemyObject.InteractiveGameObject.InteractiveGameObjectParent.transform;
            return new AIWarpActionV2(this.PlayerObject, enemyTransform.position - (enemyTransform.forward * 5f), null, nextActionsDeferred);
        }
    }
}