using InteractiveObjects;
using InteractiveObjects_Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace AIObjects
{
    #region Callback Events

    public delegate void OnAIInteractiveObjectDestinationReachedDelegate();

    #endregion

    public class AIMoveToDestinationSystem : AInteractiveObjectSystem
    {
        [VE_Nested] private AIDestinationMoveManager AIDestinationMoveManager;
        private AISpeedEventDispatcher AISpeedEventDispatcher;

        public bool IsEnabled;

        public AIMoveToDestinationSystem(CoreInteractiveObject CoreInteractiveObject, AbstractAIInteractiveObjectInitializerData AIInteractiveObjectInitializerData,
            OnAIInteractiveObjectDestinationReachedDelegate OnAIInteractiveObjectDestinationReached)
        {
            this.IsEnabled = true;
            AIDestinationMoveManager = new AIDestinationMoveManager(CoreInteractiveObject.InteractiveGameObject.Agent, AIInteractiveObjectInitializerData, OnAIInteractiveObjectDestinationReached);
            AISpeedEventDispatcher = new AISpeedEventDispatcher(CoreInteractiveObject, AIInteractiveObjectInitializerData);
        }

        public override void Tick(float d)
        {
            if (IsEnabled)
            {
                AIDestinationMoveManager.TickDestinationReached();
                AIDestinationMoveManager.EnableAgent();
                AIDestinationMoveManager.Tick(d);
            }
        }

        public override void AfterTicks()
        {
            if (IsEnabled)
            {
                AISpeedEventDispatcher.AfterTicks(AIDestinationMoveManager.CurrentDestination.HasValue);
            }
        }

        public void SetDestination(AIDestination AIDestination)
        {
            AIDestinationMoveManager.SetDestination(AIDestination);
        }

        public void SetSpeedAttenuationFactor(AIMovementSpeedDefinition AIMovementSpeedDefinition)
        {
            AIDestinationMoveManager.SetSpeedAttenuationFactor(AIMovementSpeedDefinition);
        }

        public void ClearPath()
        {
            AIDestinationMoveManager.ClearPath();
        }
    }

    internal class AIDestinationMoveManager
    {
        #region Configuration Data

        private AbstractAIInteractiveObjectInitializerData AIInteractiveObjectInitializerData;

        #endregion

        #region destination reached manual update

        private int FrameWereOccuredTheLastDestinationReached = -1;

        #endregion

        private Vector3 lastSuccessfulWorldDestination;
        private NavMeshAgent objectAgent;
        private OnAIInteractiveObjectDestinationReachedDelegate OnAIInteractiveObjectDestinationReached;

        public AIDestinationMoveManager(NavMeshAgent objectAgent, AbstractAIInteractiveObjectInitializerData AIInteractiveObjectInitializerData, OnAIInteractiveObjectDestinationReachedDelegate OnAIInteractiveObjectDestinationReached)
        {
            this.objectAgent = objectAgent;
            lastSuccessfulWorldDestination = new Vector3(9999999, 99999999, 9999999);
            this.AIInteractiveObjectInitializerData = AIInteractiveObjectInitializerData;
            this.OnAIInteractiveObjectDestinationReached = OnAIInteractiveObjectDestinationReached;
            currentSpeedAttenuationFactor = AIMovementSpeedDefinition.RUN;
        }

        public void TickDestinationReached()
        {
            //is destination reached
            if (CurrentDestination.HasValue)
                if (!objectAgent.pathPending && objectAgent.remainingDistance <= objectAgent.stoppingDistance && (!objectAgent.hasPath || objectAgent.velocity.sqrMagnitude == 0f))
                {
                    this.currentDestination = null;
                    FrameWereOccuredTheLastDestinationReached = Time.frameCount;
                    objectAgent.isStopped = true;
                    objectAgent.ResetPath();
                    Debug.Log(MyLog.Format("Destination reached !"));
                    OnAIInteractiveObjectDestinationReached.Invoke();
                }
        }

        public void Tick(float d)
        {
            DeltaTime = d;
            UpdateAgentTransform(d);
        }

        private void UpdateAgentTransform(float d)
        {
            var TransformMoveManagerComponentV3 = AIInteractiveObjectInitializerData.TransformMoveManagerComponentV3;
            objectAgent.speed = TransformMoveManagerComponentV3.SpeedMultiplicationFactor * AIMovementDefinitions.AIMovementSpeedAttenuationFactorLookup[currentSpeedAttenuationFactor];

            var updatePosition = true;
            // We use a minimal velocity amplitude to avoid precision loss occured by the navmesh agent velocity calculation.
            if (objectAgent.hasPath && !objectAgent.isStopped)
            {
                //if target is too close, we look to destination
                var distanceToDestination = Vector3.Distance(objectAgent.nextPosition, objectAgent.destination);

                Quaternion targetRotation;

                if (objectAgent.nextPosition != objectAgent.destination && distanceToDestination <= 5f)
                    targetRotation = Quaternion.LookRotation(objectAgent.destination - objectAgent.nextPosition, Vector3.up);
                else
                    targetRotation = Quaternion.LookRotation((objectAgent.path.corners[1] - objectAgent.path.corners[0]).normalized, Vector3.up);

                objectAgent.transform.rotation = Quaternion.Slerp(objectAgent.transform.rotation, targetRotation, TransformMoveManagerComponentV3.RotationSpeed * d);

                updatePosition =
                    !TransformMoveManagerComponentV3.IsPositionUpdateConstrained ||
                    TransformMoveManagerComponentV3.IsPositionUpdateConstrained && Quaternion.Angle(objectAgent.transform.rotation, targetRotation) <= TransformMoveManagerComponentV3.TransformPositionUpdateConstraints.MinAngleThatAllowThePositionUpdate;
            }
            else if (CurrentDestination.HasValue && CurrentDestination.Value.Rotation.HasValue)
            {
                var targetRotation = CurrentDestination.Value.Rotation.Value;
                objectAgent.transform.rotation = Quaternion.Slerp(objectAgent.transform.rotation, targetRotation, TransformMoveManagerComponentV3.RotationSpeed * d);
            }

            if (updatePosition)
                objectAgent.transform.position = objectAgent.nextPosition;
            else
                objectAgent.nextPosition = objectAgent.transform.position;
        }

        private void ManuallyUpdateAgent()
        {
            //   Debug.Log(MyLog.Format("ManuallyUpdateAgent"));
            Vector3 velocitySetted = default;
            NavMeshHit pathHit;
            objectAgent.SamplePathPosition(NavMesh.AllAreas, objectAgent.speed * DeltaTime, out pathHit);
            if (DeltaTime > 0) objectAgent.velocity = (pathHit.position - objectAgent.transform.position) / DeltaTime;

            objectAgent.nextPosition = pathHit.position;
        }

        private static NavMeshPath CreateValidNavMeshPathWithFallback(NavMeshAgent agent, Vector3 WorldDestination, float fallbackNearestPointDistance)
        {
            var path = new NavMeshPath();
            agent.CalculatePath(WorldDestination, path);
            if (path.status == NavMeshPathStatus.PathInvalid)
                if (Physics.Raycast(WorldDestination, WorldDestination + Vector3.down * fallbackNearestPointDistance, out var hit, fallbackNearestPointDistance, 1 << LayerMask.NameToLayer(LayerConstants.PUZZLE_GROUND_LAYER)))
                    agent.CalculatePath(hit.point, path);

            return path;
        }

        #region State

        private float DeltaTime;

        [VE_Nested] private AIDestination? currentDestination;

        public AIDestination? CurrentDestination => this.currentDestination;

        //Used to change the agent speed
        private AIMovementSpeedDefinition currentSpeedAttenuationFactor;

        #endregion

        #region External Events

        public void SetDestination(AIDestination AIDestination)
        {
            //When a different path is calculated, we manually reset the path and calculate the next destination
            //The input world destination may not be exactly on NavMesh.
            //So we do comparison between world destination
            if (lastSuccessfulWorldDestination != AIDestination.WorldPosition)
            {
                //   Debug.Log(MyLog.Format("Set destination : " + AIDestination.WorldPosition));
                this.currentDestination = AIDestination;
                objectAgent.ResetPath();
                var path = CreateValidNavMeshPathWithFallback(objectAgent, AIDestination.WorldPosition, 50);

                objectAgent.SetPath(path);

                //If direction change is occuring when current destination has been reached
                //We manually calculate next position to avoid a frame where AI is standing still
                if (FrameWereOccuredTheLastDestinationReached == Time.frameCount) ManuallyUpdateAgent();

                lastSuccessfulWorldDestination = AIDestination.WorldPosition;
            }
        }

        public void StopAgent()
        {
            if (objectAgent.hasPath)
            {
                objectAgent.ResetPath();
                objectAgent.isStopped = true;
            }
        }

        public void EnableAgent()
        {
            objectAgent.isStopped = false;
        }

        public void ClearPath()
        {
            this.currentDestination = null;
            lastSuccessfulWorldDestination = new Vector3(9999999, -9999999, 999999);
            objectAgent.ResetPath();
        }

        public void SetSpeedAttenuationFactor(AIMovementSpeedDefinition AIMovementSpeedDefinition)
        {
            currentSpeedAttenuationFactor = AIMovementSpeedDefinition;
        }

        #endregion
    }

    internal class AISpeedEventDispatcher
    {
        private AbstractAIInteractiveObjectInitializerData AIInteractiveObjectInitializerData;
        private CoreInteractiveObject AssociatedInteractiveObject;

        public AISpeedEventDispatcher(CoreInteractiveObject associatedInteractiveObject, AbstractAIInteractiveObjectInitializerData aIInteractiveObjectInitializerData)
        {
            AssociatedInteractiveObject = associatedInteractiveObject;
            AIInteractiveObjectInitializerData = aIInteractiveObjectInitializerData;
        }

        public void AfterTicks(bool hasCurrentlyADestination)
        {
            var currentSpeed = (hasCurrentlyADestination ? AssociatedInteractiveObject.InteractiveGameObject.Agent.speed : 0) / AIInteractiveObjectInitializerData.TransformMoveManagerComponentV3.SpeedMultiplicationFactor;
            AssociatedInteractiveObject.OnAnimationObjectSetUnscaledSpeedMagnitude(currentSpeed);
        }
    }
}