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
        private CoreInteractiveObject InteractiveObject;
        private TransformStruct WorldPoint;
        private AIMovementSpeedDefinition AIMovementSpeed;

        public AIMoveToActionV2(CoreInteractiveObject InteractiveObject, TransformStruct WorldPoint, AIMovementSpeedDefinition AIMovementSpeed, Func<List<ASequencedAction>> nextActionsDeffered) : base(nextActionsDeffered)
        {
            this.destinationReached = false;
            this.InteractiveObject = InteractiveObject;
            this.WorldPoint = WorldPoint;
            this.AIMovementSpeed = AIMovementSpeed;
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

        public override void Tick(float d)
        {
            if (!this.destinationReached)
            {
                this.InteractiveObject.SetAIDestination(new AIDestination
                {
                    WorldPosition = this.WorldPoint.WorldPosition,
                    Rotation = Quaternion.Euler(this.WorldPoint.WorldRotationEuler)
                });
                this.InteractiveObject.SetAISpeedAttenuationFactor(this.AIMovementSpeed);
            }
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