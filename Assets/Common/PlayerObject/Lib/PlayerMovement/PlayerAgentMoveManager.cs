using System;
using AIObjects;
using CoreGame;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using PlayerObject_Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace PlayerObject
{
    public class PlayerAgentMoveManager : APlayerMoveManager
    {
        private PlayerInteractiveObject PlayerInteractiveObject;
        private AIMoveToDestinationSystem AIMoveToDestinationSystem;

        public PlayerAgentMoveManager(PlayerInteractiveObject playerAimingInteractiveObject, TransformMoveManagerComponentV3 TransformMoveManagerComponentV3,
            OnAIInteractiveObjectDestinationReachedDelegate OnDestinationReachedCallback = null)
        {
            this.PlayerInteractiveObject = playerAimingInteractiveObject;
            this.AIMoveToDestinationSystem = new AIMoveToDestinationSystem(playerAimingInteractiveObject, TransformMoveManagerComponentV3, OnDestinationReachedCallback);
            this.CurrentConstraint = new NoConstraint();
        }

        public override void Tick(float d)
        {
        }

        public override void AfterTicks(float d)
        {
            this.CurrentConstraint.ApplyConstraint(this.PlayerInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent.transform);
            
            /// Constraints are consumed every frame.
            this.CurrentConstraint = new NoConstraint();
            
            
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

        public override void Warp(Vector3 warpWorldPosition)
        {
            this.PlayerInteractiveObject.InteractiveGameObject.Agent.Warp(warpWorldPosition);
        }
    }
}