using System;
using System.Collections.Generic;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using SequencedAction;
using UnityEngine;

namespace AIObjects
{
    public class AIMoveToActionV2 : ASequencedAction, IActionAbortedOnDestinationReached
    {
        private bool destinationReached;
        private Vector3 WorldPosition;
        private Vector3? WorldRotation;
        private AIMovementSpeedAttenuationFactor AIMovementSpeed;

        #region Callbacks

        private Action<IAgentMovementCalculationStrategy> SetCoreInteractiveObjectDestinationCallback;
        private Action<AIMovementSpeedAttenuationFactor> SetAISpeedAttenuationFactorCallback;

        #endregion

        public AIMoveToActionV2(Vector3 WorldPosition, Vector3? WorldRotation, AIMovementSpeedAttenuationFactor AIMovementSpeed,
            Action<IAgentMovementCalculationStrategy> SetCoreInteractiveObjectDestinationCallback,
            Action<AIMovementSpeedAttenuationFactor> SetAISpeedAttenuationFactorCallback, Func<List<ASequencedAction>> nextActionsDeffered) : base(nextActionsDeffered)
        {
            this.destinationReached = false;
            this.WorldPosition = WorldPosition;
            this.WorldRotation = WorldRotation;
            this.AIMovementSpeed = AIMovementSpeed;
            this.SetCoreInteractiveObjectDestinationCallback = SetCoreInteractiveObjectDestinationCallback;
            this.SetAISpeedAttenuationFactorCallback = SetAISpeedAttenuationFactorCallback;
        }

        public override void AfterFinishedEventProcessed()
        {
        }

        public override bool ComputeFinishedConditions()
        {
            return this.destinationReached;
        }

        public override void FirstExecutionAction()
        {
            this.destinationReached = false;
        }

        /// This provoque allocation ?
        public override void Tick(float d)
        {
            if (!this.destinationReached)
            {
                var strategy = ForwardAgentMovementCalculationStrategy();
                this.SetCoreInteractiveObjectDestinationCallback?.Invoke(strategy);
                this.SetAISpeedAttenuationFactorCallback?.Invoke(this.AIMovementSpeed);
            }
        }

        private ForwardAgentMovementCalculationStrategy ForwardAgentMovementCalculationStrategy()
        {
            var strategy = new ForwardAgentMovementCalculationStrategy(new AIDestination
            {
                WorldPosition = this.WorldPosition,
                Rotation = this.WorldRotation.HasValue ? Quaternion.Euler(this.WorldRotation.Value) : default(Nullable<Quaternion>)
            });
            return strategy;
        }

        public void OnDestinationReached()
        {
            this.Interupt();
        }

        public override void Interupt()
        {
            this.destinationReached = true;
        }
    }
}