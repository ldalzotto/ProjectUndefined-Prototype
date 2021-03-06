﻿using System;
using System.Collections.Generic;
using AIObjects;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using PlayerObject;
using SequencedAction;
using Targetting_Test;
using Tests;
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
        private PlayerInteractiveObject PlayerInteractiveObject;

        [WireCircleWorld(PositionFieldName = nameof(Fire1PlayerPosition))]
        public Vector3 Fire1PlayerPosition;

        public override void BeforeObjectInitialization()
        {
            CameraMovementJobManagerMocked.SetupForTestScene();
        }

        public override ASequencedAction[] BuildScenarioActions()
        {
            this.EnemyObject = InteractiveObjectV2Manager.Get().InteractiveObjects.Find(o => o.InteractiveGameObject.GetAssociatedGameObjectName() == EnemyObjectName);

            this.PlayerInteractiveObject = PlayerInteractiveObjectManager.Get().PlayerInteractiveObject;

            return new ASequencedAction[]
            {
                new AIWarpActionV2(this.PlayerInteractiveObject, this.Fire1PlayerPosition, null)
                    .Then(new TargetAndFireToInteractiveObjectAction(this.EnemyObject)
                        .Then(new AIMoveToActionV2(Vector3.zero, null, AIMovementSpeedAttenuationFactor.RUN, (strategy) => { this.PlayerInteractiveObject.SetDestination(strategy); }, this.PlayerInteractiveObject.SetAISpeedAttenuationFactor).Then(BuildWarpPlayerBehindEnemyAction())
                            .Then(new TargetAndFireToInteractiveObjectAction(this.EnemyObject)
                                .Then(new AIWarpActionV2(this.PlayerInteractiveObject, Vector3.zero, null))
                            )
                        )
                    )
            };
        }

        private AIWarpActionV2 BuildWarpPlayerBehindEnemyAction()
        {
            var enemyTransform = this.EnemyObject.InteractiveGameObject.InteractiveGameObjectParent.transform;
            return new AIWarpActionV2(this.PlayerInteractiveObject, enemyTransform.position - (enemyTransform.forward * 5f), null);
        }
    }
}