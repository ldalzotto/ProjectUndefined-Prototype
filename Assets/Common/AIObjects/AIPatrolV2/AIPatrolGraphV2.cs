using System;
using System.Collections.Generic;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
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
            return new AIMoveToActionV2(InvolvedInteractiveObject, AIMoveToActionInputData.WorldPosition, AIMoveToActionInputData.GetWorldRotation(), AIMoveToActionInputData.AIMovementSpeed, nextActionsDeffered);
        }
    }

    [Serializable]
    public class AIMoveToActionInputData
    {
        [MultiplePointMovementAware] public Vector3 WorldPosition;

        [HideInInspector]
        [SerializeField]
        private bool WorldRotationEnabled;

        [SerializeField]
        [Foldable(canBeDisabled: true, disablingBoolAttribute: nameof(AIMoveToActionInputData.WorldRotationEnabled))]
        private Vector3 worldRotation;

        public AIMovementSpeedDefinition AIMovementSpeed;

        public Nullable<Vector3> GetWorldRotation()
        {
            return this.WorldRotationEnabled ? this.worldRotation : default(Nullable<Vector3>);
        }
    }
}