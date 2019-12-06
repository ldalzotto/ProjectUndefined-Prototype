﻿using System;
using CoreGame;
using InteractiveObjects_Interfaces;
using UnityEngine;

namespace InteractiveObjects
{
    /// <summary>
    /// Indicates how <see cref="ObjectMovementSpeedSystem.ObjectMovementSpeedSystemState"/> is updated.
    /// </summary>
    public enum ObjectSpeedCalculationType
    {
        /// <summary>
        ///  <see cref="ObjectMovementSpeedSystem.ObjectMovementSpeedSystemState"/> Must be manually updated via <see cref="ObjectMovementSpeedSystem.ManualCalculation"/>.
        /// </summary>
        MANUAL,
        /// <summary>
        /// <see cref="ObjectMovementSpeedSystem.ObjectMovementSpeedSystemState"/> is updated by reading data from <see cref="NavMeshAgent"/>.
        /// </summary>
        AGENT
    }

    /// <summary>
    /// Responsible of calculating any informations relative to speed.
    /// </summary>
    public class ObjectMovementSpeedSystem
    {      
        
        #region External Dependencies

        private CoreInteractiveObject AssociatedInteractiveObject;
        private TransformMoveManagerComponentV3 AITransformMoveManagerComponentV3;

        #endregion

        private ObjectSpeedCalculationType ObjectSpeedCalculationType;

        private ObjectSpeedCalculationSystem ObjectSpeedCalculationSystem;

        public ObjectMovementSpeedSystem(CoreInteractiveObject associatedInteractiveObject, TransformMoveManagerComponentV3 aiTransformMoveManagerComponentV3,
            AIMovementSpeedAttenuationFactor InitialMovementSpeedAttenuationFactor, ObjectSpeedCalculationType InitialObjectSpeedCalculationType)
        {
            AssociatedInteractiveObject = associatedInteractiveObject;
            AITransformMoveManagerComponentV3 = aiTransformMoveManagerComponentV3;
            this.ObjectSpeedCalculationSystem = new ObjectSpeedCalculationSystem();
            this.ObjectSpeedCalculationType = InitialObjectSpeedCalculationType;

            this.ObjectMovementSpeedSystemState = new ObjectMovementSpeedSystemState()
            {
                LastFrameWorldPosition = this.AssociatedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent.transform.position,
                CurrentMovementSpeedAttenuationFactor = InitialMovementSpeedAttenuationFactor
            };
        }

        private ObjectMovementSpeedSystemState ObjectMovementSpeedSystemState;

        /// <summary>
        /// Calulcation of the <see cref="ObjectMovementSpeedSystemState.LocalSpeedDirection"/> after every objects have been updated.
        /// The importance of execution order lies in the fact that we must be sure that the object position will be it's final displayed position.
        /// </summary>
        public void AfterTicks()
        {
            var objectTransform = this.AssociatedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent.transform;
            if (this.ObjectSpeedCalculationType != ObjectSpeedCalculationType.MANUAL)
            {
                var worldDirection = this.ObjectMovementSpeedSystemState.LastFrameWorldPosition - objectTransform.position;
                this.ObjectMovementSpeedSystemState.localSpeedDirection =
                    new Vector3(Vector3.Project(worldDirection, objectTransform.right).magnitude,
                        Vector3.Project(worldDirection, objectTransform.up).magnitude,
                        Vector3.Project(worldDirection, objectTransform.forward).magnitude).normalized;

                this.ObjectMovementSpeedSystemState.SpeedMagnitude = this.ObjectSpeedCalculationSystem.CalculateSpeed(this.ObjectSpeedCalculationType, this.AssociatedInteractiveObject, this.AITransformMoveManagerComponentV3);
            }

            this.ObjectMovementSpeedSystemState.LastFrameWorldPosition = objectTransform.position;
        }

        /// <summary>
        /// This method overrides calculation data contained in <see cref="ObjectMovementSpeedSystemState"/> by data passed as input.
        /// /!\ This method must be called only if <see cref="ObjectSpeedCalculationType"/> is set to <see cref="ObjectSpeedCalculationType.MANUAL"/>.
        /// </summary>
        public void ManualCalculation(Vector3 worldSpeedDirection, float speedMagnitude)
        {
            this.ObjectMovementSpeedSystemState.localSpeedDirection = this.AssociatedInteractiveObject.InteractiveGameObject.GetWorldToLocal().MultiplyVector(worldSpeedDirection);
            this.ObjectMovementSpeedSystemState.SpeedMagnitude = speedMagnitude;
        }

        public void ResetSpeed()
        {
            this.ObjectMovementSpeedSystemState.localSpeedDirection = Vector3.zero;
            this.ObjectMovementSpeedSystemState.SpeedMagnitude = 0f;
        }

        public void SetSpeedAttenuationFactor(AIMovementSpeedAttenuationFactor SpeedAttenuationFactor)
        {
            this.ObjectMovementSpeedSystemState.CurrentMovementSpeedAttenuationFactor = SpeedAttenuationFactor;
        }

        public void SetObjectSpeedCalculationType(ObjectSpeedCalculationType ObjectSpeedCalculationType)
        {
            this.ObjectSpeedCalculationType = ObjectSpeedCalculationType;
        }

        #region Data Retrieval

        public AIMovementSpeedAttenuationFactor GetSpeedAttenuationFactor()
        {
            return this.ObjectMovementSpeedSystemState.CurrentMovementSpeedAttenuationFactor;
        }

        public Vector3 GetWorldSpeedDirection()
        {
            return this.AssociatedInteractiveObject.InteractiveGameObject.GetLocalToWorld().MultiplyVector(this.ObjectMovementSpeedSystemState.localSpeedDirection);
        }

        /// <summary>
        /// The velocity is the projection of <see cref="ObjectMovementSpeedSystemState.localSpeedDirection"/>. It takes into account all speed scale factors.
        /// </summary>
        public Vector3 GetVelocity()
        {
            return this.GetWorldSpeedDirection() * this.ObjectMovementSpeedSystemState.SpeedMagnitude * this.AITransformMoveManagerComponentV3.SpeedMultiplicationFactor
                   * AIMovementSpeedAttenuationFactors.AIMovementSpeedAttenuationFactorLookup[this.ObjectMovementSpeedSystemState.CurrentMovementSpeedAttenuationFactor];
        }

        /// <summary>
        /// Calculates the <see cref="ObjectMovementSpeedSystemState.localSpeedDirection"/> only attenuated by <see cref="AIMovementSpeedAttenuationFactors"/>.
        /// This is mainly used for animation graph that needs speed as input. It is useful when the <see cref="TransformMoveManagerComponentV3.SpeedMultiplicationFactor"/> is not
        /// necessary for the calculation.
        /// </summary>
        public Vector3 GetLocalSpeedDirectionAttenuated()
        {
            var speedAttenuationFactorValue = AIMovementSpeedAttenuationFactors.AIMovementSpeedAttenuationFactorLookup[this.ObjectMovementSpeedSystemState.CurrentMovementSpeedAttenuationFactor];
            return this.ObjectMovementSpeedSystemState.localSpeedDirection.Mul(speedAttenuationFactorValue);
        }

        public float GetSpeedMagnitude()
        {
            return this.ObjectMovementSpeedSystemState.SpeedMagnitude;
        }

        #endregion
    }

    struct ObjectMovementSpeedSystemState
    {
        public AIMovementSpeedAttenuationFactor CurrentMovementSpeedAttenuationFactor;
        public Vector3 LastFrameWorldPosition;
        public Vector3 localSpeedDirection;
        public float SpeedMagnitude;
    }

    class ObjectSpeedCalculationSystem
    {
        public float CalculateSpeed(ObjectSpeedCalculationType ObjectSpeedCalculationType,
            CoreInteractiveObject CoreInteractiveObject, TransformMoveManagerComponentV3 TransformMoveManagerComponentV3)
        {
            var SpeedMagnitude = 0f;
            if (ObjectSpeedCalculationType == ObjectSpeedCalculationType.AGENT)
            {
                var agent = CoreInteractiveObject.InteractiveGameObject.Agent;
                if (agent != null && agent.hasPath)
                {
                    SpeedMagnitude = agent.speed / TransformMoveManagerComponentV3.SpeedMultiplicationFactor;
                }
            }

            return SpeedMagnitude;
        }
    }
}