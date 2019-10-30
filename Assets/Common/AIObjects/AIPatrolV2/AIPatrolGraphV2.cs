using System;
using System.Collections.Generic;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using OdinSerializer;
using SequencedAction;
using SequencedAction_Editor_Common;
using UnityEngine;

namespace AIObjects
{
    [Serializable]
    public abstract class AIPatrolGraphV2 : ASequencedActionGraph
    {
        public abstract List<ASequencedAction> AIPatrolGraphActions(CoreInteractiveObject InvolvedInteractiveObject);

        protected AIMoveToActionV2 CreateAIMoveToActionV2(CoreInteractiveObject InvolvedInteractiveObject, AIMoveToActionInputData AIMoveToActionInputData, Func<List<ASequencedAction>> nextActionsDeffered)
        {
            return new AIMoveToActionV2(InvolvedInteractiveObject, AIMoveToActionInputData.WorldPoint, AIMoveToActionInputData.AIMovementSpeed, nextActionsDeffered);
        }
    }

    [Serializable]
    public class AIMoveToActionInputData
    {
        [MultiplePointMovementAware] public TransformStruct WorldPoint;
        public AIMovementSpeedDefinition AIMovementSpeed;

        public Vector3 GetWorldPosition()
        {
            return this.WorldPoint.WorldPosition;
        }
    }
}