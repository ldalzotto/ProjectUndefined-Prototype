using System;
using CoreGame;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Profiling;

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

        #region State

        private Type LastIAgentMovementCalculationStrategyType;
        public bool IsEnabled;

        #endregion


        public AIMoveToDestinationSystem(CoreInteractiveObject CoreInteractiveObject, TransformMoveManagerComponentV3 AITransformMoveManagerComponentV3,
            Func<AIMovementSpeedAttenuationFactor> AIMovementSpeedAttenuationFactorProvider,
            OnAIInteractiveObjectDestinationReachedDelegate OnAIInteractiveObjectDestinationReached = null)
        {
            this.IsEnabled = true;
            this.objectAgent = CoreInteractiveObject.InteractiveGameObject.Agent;
            this.AITransformMoveManagerComponentV3 = AITransformMoveManagerComponentV3;
            this.aiPositionMoveManager = new AIPositionMoveManager(this.objectAgent, () => this.A_AIRotationMoveManager.CurrentLookingTargetRotation, AITransformMoveManagerComponentV3,
                AIMovementSpeedAttenuationFactorProvider);
            this.AIDestinationManager = new AIDestinationManager(this.objectAgent, OnAIInteractiveObjectDestinationReached, this.aiPositionMoveManager);
            this.A_AIRotationMoveManager = new AIRotationMoveManager(this.objectAgent, AITransformMoveManagerComponentV3, this.AIDestinationManager);
        }

        public override void Tick(float d)
        {
            Profiler.BeginSample("AIMoveToDestinationSystem");
            if (IsEnabled)
            {
                this.EnableAgent();
                this.AIDestinationManager.CheckIfDestinationReached(d);

                Profiler.BeginSample("A_AIRotationMoveManager");
                this.A_AIRotationMoveManager.UpdateAgentRotation(d);
                Profiler.EndSample();

                this.aiPositionMoveManager.UpdateAgentPosition(d);
            }
            else
            {
                this.StopAgent();
            }

            Profiler.EndSample();
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

    /// <summary>
    /// Stores the current destination <see cref="CurrentDestination"/>.
    /// Calculate path for destination set <see cref="SetDestination"/>.
    /// Emit <see cref="OnAIInteractiveObjectDestinationReached"/> event when the destination is reached.
    /// Manually update agent position following it's path if destination is setted the same frame as the destination is reached <see cref="ManuallyUpdateAgent"/>
    /// </summary>
    internal class AIDestinationManager
    {
        /// <summary>
        /// The <see cref="DeltaTime"/> is used when manually update agent position following it's path <see cref="ManuallyUpdateAgent"/>. The value is stored to sample the <see cref="NavMeshPath"/>.
        /// This value is stored to avoid having a deltaTime parameter when setting the destination <see cref="SetDestination"/> -> the  <see cref="ManuallyUpdateAgent"/> calculation
        /// will use the <see cref="DeltaTime"/> instead of a parameter deltaTime.
        /// </summary>
        private float DeltaTime;

        /// <summary>
        /// Thre frame number were destination has been reached is stored to trigger the <see cref="ManuallyUpdateAgent"/> if a destination is set the same frame as the destination is reached.
        /// </summary>
        private int FrameWereOccuredTheLastDestinationReached = -1;

        private Vector3 lastSettedWorldDestination;
        private NavMeshAgent objectAgent;
        private OnAIInteractiveObjectDestinationReachedDelegate OnAIInteractiveObjectDestinationReached;

        [VE_Nested] private AIDestination? currentDestination;
        public AIDestination? CurrentDestination => currentDestination;

        private AIPositionMoveManager aiPositionMoveManagerRef;

        public AIDestinationManager(NavMeshAgent objectAgent, OnAIInteractiveObjectDestinationReachedDelegate OnAIInteractiveObjectDestinationReached, AIPositionMoveManager aiPositionMoveManagerRef)
        {
            this.objectAgent = objectAgent;
            this.OnAIInteractiveObjectDestinationReached = OnAIInteractiveObjectDestinationReached;
            this.lastSettedWorldDestination = new Vector3(9999999, 99999999, 9999999);
            this.aiPositionMoveManagerRef = aiPositionMoveManagerRef;
        }

        /// <summary>
        /// Trigger <see cref="OnAIInteractiveObjectDestinationReached"/> event when conditions are met.
        /// </summary>
        public void CheckIfDestinationReached(float d)
        {
            this.DeltaTime = d;

            /// If there is currently a destination to reach
            if (CurrentDestination.HasValue)
                if (
                    /// If the NavMeshPath not currently async calculated
                    !objectAgent.pathPending
                    /// If the WorldPosition has been reached
                    && objectAgent.remainingDistance <= objectAgent.stoppingDistance
                    /// If there is a WorldRotation angle to reach
                    && ((!this.CurrentDestination.Value.Rotation.HasValue) || (this.CurrentDestination.Value.Rotation.Value.IsApproximate(objectAgent.transform.rotation)))
                    /// If the agent has stopped
                    && (!objectAgent.hasPath || objectAgent.velocity.sqrMagnitude == 0f))
                {
                    this.currentDestination = null;
                    FrameWereOccuredTheLastDestinationReached = Time.frameCount;
                    objectAgent.isStopped = true;
                    objectAgent.ResetPath();
                    //Debug.Log(MyLog.Format("Destination reached !"));
                    OnAIInteractiveObjectDestinationReached?.Invoke();
                }
        }


        public NavMeshPathStatus SetDestination(AIDestination AIDestination)
        {
            /// When a different path is calculated, we manually reset the path and calculate the next destination
            /// The input world destination may not be exactly on NavMesh.
            /// So we do comparison between world destination
            if (lastSettedWorldDestination != AIDestination.WorldPosition)
            {
                //   Debug.Log(MyLog.Format("Set destination : " + AIDestination.WorldPosition));
                this.currentDestination = AIDestination;
                objectAgent.ResetPath();
                var path = CreateValidNavMeshPathWithFallback(objectAgent, AIDestination.WorldPosition, 50);
                objectAgent.SetPath(path);
                if (path.status != NavMeshPathStatus.PathInvalid)
                {
                    /// If direction change is occuring when current destination has been reached
                    /// We manually calculate next position to avoid a frame where AI is standing still
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

        /// <summary>
        /// When clearing the path, the <see cref="lastSettedWorldDestination"/> is setted to an improbable value to bypass <see cref="SetDestination"/> destination cache and always trigger
        /// <see cref="NavMeshPath"/> calculation.
        /// </summary>
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

        public Vector3 LastFrameWorldPosition { get; private set; }

        #endregion

        private NavMeshAgent objectAgent;
        private Func<Quaternion> CurrentLookingTargetRotationFromAIRotationMoveManager;
        private Func<AIMovementSpeedAttenuationFactor> AIMovementSpeedAttenuationFactorProvider;
        public AIPositionMoveManager(NavMeshAgent objectAgent, Func<Quaternion> CurrentLookingTargetRotationFromAIRotationMoveManager, TransformMoveManagerComponentV3 AITransformMoveManagerComponentV3, 
            Func<AIMovementSpeedAttenuationFactor> AIMovementSpeedAttenuationFactorProvider)
        {
            this.objectAgent = objectAgent;
            this.CurrentLookingTargetRotationFromAIRotationMoveManager = CurrentLookingTargetRotationFromAIRotationMoveManager;
            this.AITransformMoveManagerComponentV3 = AITransformMoveManagerComponentV3;
            this.AIMovementSpeedAttenuationFactorProvider = AIMovementSpeedAttenuationFactorProvider;
        }

        public void UpdateAgentPosition(float d)
        {
            objectAgent.speed = this.AITransformMoveManagerComponentV3.SpeedMultiplicationFactor * AIMovementSpeedAttenuationFactors.AIMovementSpeedAttenuationFactorLookup[this.AIMovementSpeedAttenuationFactorProvider.Invoke()];

            var updatePosition = true;
            // We use a minimal velocity amplitude to avoid precision loss occured by the navmesh agent velocity calculation.
            if (objectAgent.hasPath && !objectAgent.isStopped)
            {
                updatePosition =
                    !this.AITransformMoveManagerComponentV3.IsPositionUpdateConstrained ||
                    this.AITransformMoveManagerComponentV3.IsPositionUpdateConstrained
                    && Quaternion.Angle(objectAgent.transform.rotation, this.CurrentLookingTargetRotationFromAIRotationMoveManager.Invoke()) <= this.AITransformMoveManagerComponentV3.TransformPositionUpdateConstraints.MinAngleThatAllowThePositionUpdate;
            }


            this.LastFrameWorldPosition = objectAgent.transform.position;
            if (updatePosition)
            {
                objectAgent.transform.position = objectAgent.nextPosition;
            }
            else
            {
                objectAgent.nextPosition = objectAgent.transform.position;
            }
        }
    }

}