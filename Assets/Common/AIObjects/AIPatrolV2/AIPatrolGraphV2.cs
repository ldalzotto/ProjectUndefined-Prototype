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
        public abstract List<ASequencedAction> AIPatrolGraphActions(CoreInteractiveObject InvolvedInteractiveObject, AIPatrolGraphRuntimeCallbacks AIPatrolGraphRuntimeCallbacks);

        protected AIMoveToActionV2 CreateAIMoveToActionV2(AIMoveToActionInputData AIMoveToActionInputData, AIPatrolGraphRuntimeCallbacks AIPatrolGraphRuntimeCallbacks, Func<List<ASequencedAction>> nextActionsDeffered)
        {
            return new AIMoveToActionV2(AIMoveToActionInputData.WorldPosition, AIMoveToActionInputData.GetWorldRotation(), AIMoveToActionInputData.AIMovementSpeed,
                AIPatrolGraphRuntimeCallbacks.SetCoreInteractiveObjectDestinationCallback,
                AIPatrolGraphRuntimeCallbacks.SetAISpeedAttenuationFactorCallback, nextActionsDeffered);
        }
    }

    [Serializable]
    public class AIMoveToActionInputData
    {
        [MultiplePointMovementAware] public Vector3 WorldPosition;

        [HideInInspector] [SerializeField] private bool WorldRotationEnabled;

        [SerializeField] [Foldable(canBeDisabled: true, disablingBoolAttribute: nameof(AIMoveToActionInputData.WorldRotationEnabled))]
        private Vector3 worldRotation;

        public AIMovementSpeedAttenuationFactor AIMovementSpeed;

        public Nullable<Vector3> GetWorldRotation()
        {
            return this.WorldRotationEnabled ? this.worldRotation : default(Nullable<Vector3>);
        }
    }

    public struct AIPatrolGraphRuntimeCallbacks
    {
        public Action<IAgentMovementCalculationStrategy> SetCoreInteractiveObjectDestinationCallback { get; private set; }
        public Action<AIMovementSpeedAttenuationFactor> SetAISpeedAttenuationFactorCallback { get; private set; }

        public AIPatrolGraphRuntimeCallbacks(Action<IAgentMovementCalculationStrategy> coreInteractiveObjectDestinationCallback, Action<AIMovementSpeedAttenuationFactor> aiSpeedAttenuationFactorCallback)
        {
            SetCoreInteractiveObjectDestinationCallback = coreInteractiveObjectDestinationCallback;
            SetAISpeedAttenuationFactorCallback = aiSpeedAttenuationFactorCallback;
        }
    }
}