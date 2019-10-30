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
        private TransformStruct WorldPoint;

        public AIWarpActionV2(CoreInteractiveObject InteractiveObject, TransformStruct WorldPoint,
            Func<List<ASequencedAction>> nextActionsDeferred) : base(nextActionsDeferred)
        {
            this.InteractiveObject = InteractiveObject;
            this.WorldPoint = WorldPoint;
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
            this.InteractiveObject.InteractiveGameObject.Agent.Warp(this.WorldPoint.WorldPosition);
            this.InteractiveObject.InteractiveGameObject.Agent.transform.rotation = Quaternion.Euler(this.WorldPoint.WorldRotationEuler);
        }

        public override void Tick(float d)
        {
        }
    }
}