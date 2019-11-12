using System;
using CoreGame;
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
        private NavMeshAgent objectAgent;
        private TransformMoveManagerComponentV3 AITransformMoveManagerComponentV3;

        private AIDestinationManager AIDestinationManager;
        [VE_Nested] private AIPositionMoveManager aiPositionMoveManager;
        private A_AIRotationMoveManager A_AIRotationMoveManager;
        private AISpeedEventDispatcher AISpeedEventDispatcher;

        #region State

        private Type LastIAgentMovementCalculationStrategyType;
        public bool IsEnabled;

        #endregion


        public AIMoveToDestinationSystem(CoreInteractiveObject CoreInteractiveObject, TransformMoveManagerComponentV3 AITransformMoveManagerComponentV3,
            OnAIInteractiveObjectDestinationReachedDelegate OnAIInteractiveObjectDestinationReached = null, Action<Vector3> OnAgentUnscaledSpeedMagnitudeCalculatedAction = null)
        {
            this.IsEnabled = true;
            this.objectAgent = CoreInteractiveObject.InteractiveGameObject.Agent;
            this.AITransformMoveManagerComponentV3 = AITransformMoveManagerComponentV3;
            this.aiPositionMoveManager = new AIPositionMoveManager(this.objectAgent, () => this.A_AIRotationMoveManager.CurrentLookingTargetRotation, AITransformMoveManagerComponentV3);
            this.AIDestinationManager = new AIDestinationManager(this.objectAgent, OnAIInteractiveObjectDestinationReached, this.aiPositionMoveManager);
            this.A_AIRotationMoveManager = new AIRotationMoveManager(this.objectAgent, AITransformMoveManagerComponentV3, this.AIDestinationManager);
            this.AISpeedEventDispatcher = new AISpeedEventDispatcher(CoreInteractiveObject, AITransformMoveManagerComponentV3, OnAgentUnscaledSpeedMagnitudeCalculatedAction);
        }

        public override void Tick(float d)
        {
            if (IsEnabled)
            {
                this.EnableAgent();
                this.AIDestinationManager.CheckIfDestinationReached(d);
                this.A_AIRotationMoveManager.UpdateAgentRotation(d);
                this.aiPositionMoveManager.UpdateAgentPosition(d);
            }
            else
            {
                this.StopAgent();
            }
        }

        public override void AfterTicks()
        {
            if (IsEnabled)
            {
                AISpeedEventDispatcher.AfterTicks(this.AIDestinationManager.CurrentDestination.HasValue, this.aiPositionMoveManager.CurrentLocalDirection);
            }
        }

        public NavMeshPathStatus SetDestination(IAgentMovementCalculationStrategy IAgentMovementCalculationStrategy)
        {
            if (LastIAgentMovementCalculationStrategyType == null || LastIAgentMovementCalculationStrategyType != IAgentMovementCalculationStrategy.GetType())
            {
                switch (IAgentMovementCalculationStrategy)
                {
                    case ForwardAgentMovementCalculationStrategy ForwardAgentMovementCalculationStrategy:
                        this.A_AIRotationMoveManager = new AIRotationMoveManager(this.objectAgent, this.AITransformMoveManagerComponentV3, this.AIDestinationManager);
                        break;
                    case LookingAtAgentMovementCalculationStrategy LookingAtAgentMovementCalculationStrategy:
                        this.A_AIRotationMoveManager = new AIRotationFacingMoveManager(this.objectAgent, this.AITransformMoveManagerComponentV3);
                        break;
                }
            }

            switch (IAgentMovementCalculationStrategy)
            {
                case LookingAtAgentMovementCalculationStrategy LookingAtAgentMovementCalculationStrategy:
                    (this.A_AIRotationMoveManager as AIRotationFacingMoveManager).Init(LookingAtAgentMovementCalculationStrategy.TargetLook);
                    break;
            }

            this.LastIAgentMovementCalculationStrategyType = IAgentMovementCalculationStrategy.GetType();
            return this.AIDestinationManager.SetDestination(IAgentMovementCalculationStrategy.GetAIDestination());
        }

        public void SetSpeedAttenuationFactor(AIMovementSpeedDefinition AIMovementSpeedDefinition)
        {
            aiPositionMoveManager.SetSpeedAttenuationFactor(AIMovementSpeedDefinition);
        }

        public void ClearPath()
        {
            this.AIDestinationManager.ClearPath();
            if (objectAgent.hasPath)
            {
                objectAgent.ResetPath();
            }
        }


        public void EnableAgent()
        {
            objectAgent.isStopped = false;
        }

        public void StopAgent()
        {
            this.AIDestinationManager.ClearPath();
            if (objectAgent.hasPath)
            {
                objectAgent.ResetPath();
                objectAgent.isStopped = true;
            }
        }
    }

    internal class AIDestinationManager
    {
        private float DeltaTime;
        private int FrameWereOccuredTheLastDestinationReached = -1;
        private Vector3 lastSettedWorldDestination;
        private NavMeshAgent objectAgent;
        private OnAIInteractiveObjectDestinationReachedDelegate OnAIInteractiveObjectDestinationReached;
        [VE_Nested] private AIDestination? currentDestination;
        public AIDestination? CurrentDestination => currentDestination;

        private AIPositionMoveManager _aiPositionMoveManagerRef;

        public AIDestinationManager(NavMeshAgent objectAgent, OnAIInteractiveObjectDestinationReachedDelegate OnAIInteractiveObjectDestinationReached, AIPositionMoveManager aiPositionMoveManagerRef)
        {
            this.objectAgent = objectAgent;
            this.OnAIInteractiveObjectDestinationReached = OnAIInteractiveObjectDestinationReached;
            this.lastSettedWorldDestination = new Vector3(9999999, 99999999, 9999999);
            this._aiPositionMoveManagerRef = aiPositionMoveManagerRef;
        }

        public void CheckIfDestinationReached(float d)
        {
            this.DeltaTime = d;
            //is destination reached
            if (CurrentDestination.HasValue)
                if (!objectAgent.pathPending && objectAgent.remainingDistance <= objectAgent.stoppingDistance && (!objectAgent.hasPath || objectAgent.velocity.sqrMagnitude == 0f))
                {
                    this.currentDestination = null;
                    FrameWereOccuredTheLastDestinationReached = Time.frameCount;
                    objectAgent.isStopped = true;
                    objectAgent.ResetPath();
                    Debug.Log(MyLog.Format("Destination reached !"));
                    OnAIInteractiveObjectDestinationReached?.Invoke();
                }
        }


        public NavMeshPathStatus SetDestination(AIDestination AIDestination)
        {
            //When a different path is calculated, we manually reset the path and calculate the next destination
            //The input world destination may not be exactly on NavMesh.
            //So we do comparison between world destination
            if (lastSettedWorldDestination != AIDestination.WorldPosition)
            {
                //   Debug.Log(MyLog.Format("Set destination : " + AIDestination.WorldPosition));
                this.currentDestination = AIDestination;
                objectAgent.ResetPath();
                var path = CreateValidNavMeshPathWithFallback(objectAgent, AIDestination.WorldPosition, 50);
                objectAgent.SetPath(path);
                if (path.status != NavMeshPathStatus.PathInvalid)
                {
                    //If direction change is occuring when current destination has been reached
                    //We manually calculate next position to avoid a frame where AI is standing still
                    if (FrameWereOccuredTheLastDestinationReached == Time.frameCount)
                    {
                        this.ManuallyUpdateAgent(this.objectAgent);
                    }
                }
                else
                {
                    this.currentDestination = null;
                }

                lastSettedWorldDestination = AIDestination.WorldPosition;
                return path.status;
            }

            return NavMeshPathStatus.PathComplete;
        }

        public void ClearPath()
        {
            this.lastSettedWorldDestination = new Vector3(9999999, 99999999, 9999999);
            this.currentDestination = null;
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


        private void ManuallyUpdateAgent(NavMeshAgent agent)
        {
            //   Debug.Log(MyLog.Format("ManuallyUpdateAgent"));
            Vector3 velocitySetted = default;
            NavMeshHit pathHit;
            agent.SamplePathPosition(NavMesh.AllAreas, agent.speed * DeltaTime, out pathHit);
            if (DeltaTime > 0) agent.velocity = (pathHit.position - agent.transform.position) / DeltaTime;

            agent.nextPosition = pathHit.position;
        }
    }

    internal class AIPositionMoveManager
    {
        #region Configuration Data

        private TransformMoveManagerComponentV3 AITransformMoveManagerComponentV3;

        #endregion

        #region State

        //Used to change the agent speed
        private AIMovementSpeedDefinition currentSpeedAttenuationFactor;
        public Vector3 CurrentLocalDirection { get; private set; }

        #endregion

        private NavMeshAgent objectAgent;
        private Func<Quaternion> CurrentLookingTargetRotationFromAIRotationMoveManager;

        public AIPositionMoveManager(NavMeshAgent objectAgent, Func<Quaternion> CurrentLookingTargetRotationFromAIRotationMoveManager, TransformMoveManagerComponentV3 AITransformMoveManagerComponentV3)
        {
            this.objectAgent = objectAgent;
            this.CurrentLookingTargetRotationFromAIRotationMoveManager = CurrentLookingTargetRotationFromAIRotationMoveManager;
            this.AITransformMoveManagerComponentV3 = AITransformMoveManagerComponentV3;
            currentSpeedAttenuationFactor = AIMovementSpeedDefinition.RUN;
        }

        public void UpdateAgentPosition(float d)
        {
            objectAgent.speed = this.AITransformMoveManagerComponentV3.SpeedMultiplicationFactor * AIMovementDefinitions.AIMovementSpeedAttenuationFactorLookup[currentSpeedAttenuationFactor];

            var updatePosition = true;
            // We use a minimal velocity amplitude to avoid precision loss occured by the navmesh agent velocity calculation.
            if (objectAgent.hasPath && !objectAgent.isStopped)
            {
                updatePosition =
                    !this.AITransformMoveManagerComponentV3.IsPositionUpdateConstrained ||
                    this.AITransformMoveManagerComponentV3.IsPositionUpdateConstrained
                    && Quaternion.Angle(objectAgent.transform.rotation, this.CurrentLookingTargetRotationFromAIRotationMoveManager.Invoke()) <= this.AITransformMoveManagerComponentV3.TransformPositionUpdateConstraints.MinAngleThatAllowThePositionUpdate;
            }

            if (updatePosition)
            {
                var WorldDirection = (objectAgent.nextPosition - objectAgent.transform.position).normalized;
                this.CurrentLocalDirection =
                    new Vector3(Vector3.Project(WorldDirection, objectAgent.transform.right).magnitude,
                        Vector3.Project(WorldDirection, objectAgent.transform.up).magnitude,
                        Vector3.Project(WorldDirection, objectAgent.transform.forward).magnitude).normalized;
                objectAgent.transform.position = objectAgent.nextPosition;
            }
            else
            {
                this.CurrentLocalDirection = Vector3.zero;
                objectAgent.nextPosition = objectAgent.transform.position;
            }
        }

        #region External Events

        public void SetSpeedAttenuationFactor(AIMovementSpeedDefinition AIMovementSpeedDefinition)
        {
            currentSpeedAttenuationFactor = AIMovementSpeedDefinition;
        }

        #endregion
    }


    internal class AISpeedEventDispatcher
    {
        private TransformMoveManagerComponentV3 AITransformMoveManagerComponentV3;
        private CoreInteractiveObject AssociatedInteractiveObject;

        private Action<Vector3> OnAgentLocalDirectionCalculated;

        public AISpeedEventDispatcher(CoreInteractiveObject associatedInteractiveObject, TransformMoveManagerComponentV3 AITransformMoveManagerComponentV3,
            Action<Vector3> onAgentLocalDirectionCalculated)
        {
            AssociatedInteractiveObject = associatedInteractiveObject;
            this.AITransformMoveManagerComponentV3 = AITransformMoveManagerComponentV3;
            this.OnAgentLocalDirectionCalculated = onAgentLocalDirectionCalculated;
        }

        public void AfterTicks(bool currentlyHasADestination, Vector3 CurrentLocalDirection)
        {
            var currentSpeed = (currentlyHasADestination ? AssociatedInteractiveObject.InteractiveGameObject.Agent.speed : 0) / this.AITransformMoveManagerComponentV3.SpeedMultiplicationFactor;
            this.OnAgentLocalDirectionCalculated?.Invoke(CurrentLocalDirection.normalized * currentSpeed);
        }
    }
}