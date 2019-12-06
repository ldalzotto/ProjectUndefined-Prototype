﻿using System;
using AIObjects;
using CoreGame;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace PlayerObject
{
    public class PlayerAgentMoveManager : APlayerMoveManager
    {
        private AIMoveToDestinationSystem AIMoveToDestinationSystem;

        public PlayerAgentMoveManager(PlayerInteractiveObject PlayerInteractiveObject, TransformMoveManagerComponentV3 TransformMoveManagerComponentV3,
            OnAIInteractiveObjectDestinationReachedDelegate OnDestinationReachedCallback = null) : base(PlayerInteractiveObject, TransformMoveManagerComponentV3)
        {
            this.AIMoveToDestinationSystem = new AIMoveToDestinationSystem(PlayerInteractiveObject, TransformMoveManagerComponentV3, this.ObjectMovementSpeedSystem.GetSpeedAttenuationFactor, OnDestinationReachedCallback);
        }

        public override void Tick(float d)
        {
            this.AIMoveToDestinationSystem.Tick(d);
        }

        public override void AfterTicks()
        {
            base.AfterTicks();
            this.AIMoveToDestinationSystem.AfterTicks();
        }

        public override void ResetSpeed()
        {
            base.ResetSpeed();
            this.AIMoveToDestinationSystem.StopAgent();
        }
        
        public override NavMeshPathStatus SetDestination(IAgentMovementCalculationStrategy IAgentMovementCalculationStrategy)
        {
            return this.AIMoveToDestinationSystem.SetDestination(IAgentMovementCalculationStrategy);
        }
    }
}