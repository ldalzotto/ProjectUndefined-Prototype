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
            ObjectMovementSpeedSystem ObjectMovementSpeedSystemRef,
            OnAIInteractiveObjectDestinationReachedDelegate OnDestinationReachedCallback = null)
        {
            this.AIMoveToDestinationSystem = new AIMoveToDestinationSystem(playerAimingInteractiveObject, TransformMoveManagerComponentV3, ObjectMovementSpeedSystemRef.GetSpeedAttenuationFactor, OnDestinationReachedCallback);
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