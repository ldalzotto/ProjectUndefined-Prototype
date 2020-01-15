using System;
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

        public PlayerAgentMoveManager(PlayerInteractiveObject playerAimingInteractiveObject, TransformMoveManagerComponentV3 TransformMoveManagerComponentV3,
            OnAIInteractiveObjectDestinationReachedDelegate OnDestinationReachedCallback = null)
        {
            this.AIMoveToDestinationSystem = new AIMoveToDestinationSystem(playerAimingInteractiveObject, TransformMoveManagerComponentV3, OnDestinationReachedCallback);
        }

        public override void Tick(float d)
        {
        }

        public override void AfterTicks(float d)
        {
            base.AfterTicks(d);
            this.AIMoveToDestinationSystem.AfterTicks(d);
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