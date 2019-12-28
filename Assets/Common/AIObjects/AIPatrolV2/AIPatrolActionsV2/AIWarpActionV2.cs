using System;
using System.Collections.Generic;
using InteractiveObjects;
using SequencedAction;
using UnityEngine;

namespace AIObjects
{
    public class AIWarpActionV2 : ASequencedAction
    {
        private CoreInteractiveObject InteractiveObject;
        private Vector3 WorldPosition;
        private Vector3? WorldRotation;

        public AIWarpActionV2(CoreInteractiveObject InteractiveObject, Vector3 WorldPosition, Vector3? WorldRotation)
        {
            this.InteractiveObject = InteractiveObject;
            this.WorldPosition = WorldPosition;
            this.WorldRotation = WorldRotation;
        }

        public override void AfterFinishedEventProcessed()
        {
        }

        public override bool ComputeFinishedConditions()
        {
            return true;
        }

        public override void FirstExecutionAction()
        {
            this.InteractiveObject.InteractiveGameObject.Agent.Warp(this.WorldPosition);
            if (this.WorldRotation.HasValue)
            {
                this.InteractiveObject.InteractiveGameObject.Agent.transform.rotation = Quaternion.Euler(this.WorldRotation.Value);
            }
        }

        public override void Tick(float d)
        {
        }
    }
}