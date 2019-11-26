using System;
using AIObjects;
using CoreGame;
using InteractiveObjects_Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace PlayerObject
{
    public class PlayerAgentMoveManager : APlayerMoveManager
    {
        private AIMoveToDestinationSystem AIMoveToDestinationSystem;
        private PlayerSpeedProcessingInput PlayerSpeedProcessingInput;
        private AIMovementSpeedAttenuationFactor _aiMovementSpeedAttenuationFactorAttenuationFactor;

        public PlayerAgentMoveManager(PlayerInteractiveObject PlayerInteractiveObject, TransformMoveManagerComponentV3 TransformMoveManagerComponentV3, 
            OnAIInteractiveObjectDestinationReachedDelegate OnDestinationReachedCallback = null)
        {
            this._aiMovementSpeedAttenuationFactorAttenuationFactor = AIMovementSpeedAttenuationFactor.RUN;
            this.AIMoveToDestinationSystem = new AIMoveToDestinationSystem(PlayerInteractiveObject, TransformMoveManagerComponentV3, OnDestinationReachedCallback, this.AgentSpeedDirectionWithMagnitude);
        }

        public override void Tick(float d)
        {
            this.AIMoveToDestinationSystem.Tick(d);
        }

        public override void AfterTicks()
        {
            this.AIMoveToDestinationSystem.AfterTicks();
        }

        public override void ResetSpeed()
        {
            this.AIMoveToDestinationSystem.StopAgent();
            this.PlayerSpeedProcessingInput = new PlayerSpeedProcessingInput(Vector3.zero, 0f);
        }

        private void AgentSpeedDirectionWithMagnitude(Vector3 speedDirectionWithMagnitude)
        {
            this.PlayerSpeedProcessingInput = new PlayerSpeedProcessingInput(speedDirectionWithMagnitude.normalized,
                speedDirectionWithMagnitude.magnitude * AIMovementSpeedAttenuationFactors.AIMovementSpeedAttenuationFactorLookup[this._aiMovementSpeedAttenuationFactorAttenuationFactor]);
        }

        public override float GetPlayerSpeedMagnitude()
        {
            return this.PlayerSpeedProcessingInput.PlayerSpeedMagnitude;
        }
        
        public override void SetSpeedAttenuationFactor(AIMovementSpeedAttenuationFactor aiMovementSpeedAttenuationFactor)
        {
            this._aiMovementSpeedAttenuationFactorAttenuationFactor = aiMovementSpeedAttenuationFactor;
        }

        public override AIMovementSpeedAttenuationFactor GetCurrentSpeedAttenuationFactor()
        {
            return this._aiMovementSpeedAttenuationFactorAttenuationFactor;
        }

        public override NavMeshPathStatus SetDestination(IAgentMovementCalculationStrategy IAgentMovementCalculationStrategy)
        {
            return this.AIMoveToDestinationSystem.SetDestination(IAgentMovementCalculationStrategy);
        }
    }
}