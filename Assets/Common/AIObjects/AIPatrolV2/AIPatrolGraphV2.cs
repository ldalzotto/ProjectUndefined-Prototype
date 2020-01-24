using System;
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
        public abstract ASequencedAction[] AIPatrolGraphActions(CoreInteractiveObject InvolvedInteractiveObject, AIPatrolGraphRuntimeCallbacks AIPatrolGraphRuntimeCallbacks);

        public static AIWarpActionV2 CreateAIWarpAction(CoreInteractiveObject InvolvedInteractiveObject, AIMoveToActionInputData AIMoveToActionInputData)
        {
            return CreateAIWarpAction(InvolvedInteractiveObject, AIMoveToActionInputData.WorldPosition, AIMoveToActionInputData.GetWorldRotation());
        }

        public static AIWarpActionV2 CreateAIWarpAction(CoreInteractiveObject InvolvedInteractiveObject, Vector3 WorldPosition, Vector3? WorldRotation)
        {
            return new AIWarpActionV2(InvolvedInteractiveObject, WorldPosition, WorldRotation);
        }

        public static AIMoveToActionV2 CreateAIMoveToActionV2(AIMoveToActionInputData AIMoveToActionInputData, AIPatrolGraphRuntimeCallbacks AIPatrolGraphRuntimeCallbacks)
        {
            return CreateAIMoveToActionV2(AIMoveToActionInputData.WorldPosition, AIMoveToActionInputData.GetWorldRotation(), AIMoveToActionInputData.AIMovementSpeed, AIPatrolGraphRuntimeCallbacks);
        }

        public static AIMoveToActionV2 CreateAIMoveToActionV2(Vector3 WorldPosition, Vector3? WorldRotation,
            AIMovementSpeedAttenuationFactor AIMovementSpeed, AIPatrolGraphRuntimeCallbacks AIPatrolGraphRuntimeCallbacks)
        {
            return new AIMoveToActionV2(WorldPosition, WorldRotation, AIMovementSpeed,
                AIPatrolGraphRuntimeCallbacks.SetCoreInteractiveObjectDestinationCallback,
                AIPatrolGraphRuntimeCallbacks.SetAISpeedAttenuationFactorCallback);
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