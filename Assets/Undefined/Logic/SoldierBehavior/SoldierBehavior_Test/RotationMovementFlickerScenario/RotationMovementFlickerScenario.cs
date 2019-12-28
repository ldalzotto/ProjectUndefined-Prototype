﻿using System;
using System.Collections.Generic;
using AIObjects;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using PlayerObject;
using SequencedAction;
using Tests.TestScenario;
using UnityEngine;

namespace SoliderBehavior_Test
{
    [Serializable]
    [SceneHandleDraw]
    public class RotationMovementFlickerScenario : ATestScenarioDefinition
    {
        private const string EnemyObjectName = "Enemy";

        private CoreInteractiveObject EnemyObject;
        private PlayerInteractiveObject PlayerObject;

        [WireCircleWorld(PositionFieldName = nameof(PlayerStartPosition))]
        public Vector3 PlayerStartPosition;

        [WireCircleWorld(PositionFieldName = nameof(PlayerTargetPosition))]
        public Vector3 PlayerTargetPosition;


        public override ASequencedAction[] BuildScenarioActions()
        {
            this.EnemyObject = InteractiveObjectV2Manager.Get().InteractiveObjects.Find(o => o.InteractiveGameObject.GetAssociatedGameObjectName() == EnemyObjectName);
            this.PlayerObject = PlayerInteractiveObjectManager.Get().PlayerInteractiveObject;

            return new ASequencedAction[]
            {
                new AIWarpActionV2(this.PlayerObject, this.PlayerStartPosition, null)
                    .Then(new WaitForSecondsAction(1.5f)
                        .Then(new AIMoveToActionV2(this.PlayerTargetPosition, null, AIMovementSpeedAttenuationFactor.RUN, (strategy) => { this.PlayerObject.SetDestination(strategy); }, this.PlayerObject.SetAISpeedAttenuationFactor))
                    )
            };
        }
    }
}