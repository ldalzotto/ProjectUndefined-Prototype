using System;
using CoreGame;
using InteractiveObjects_Interfaces;
using UnityEngine;

namespace InteractiveObjects
{
    public class ObjectMovementSpeedSystem
    {
        private CoreInteractiveObject AssociatedInteractiveObject;
        private TransformMoveManagerComponentV3 AITransformMoveManagerComponentV3;

        private ObjectSpeedCalculationSystem ObjectSpeedCalculationSystem;

        public ObjectMovementSpeedSystem(CoreInteractiveObject associatedInteractiveObject, TransformMoveManagerComponentV3 aiTransformMoveManagerComponentV3,
            AIMovementSpeedAttenuationFactor InitialMovementSpeedAttenuationFactor, ObjectSpeedCalculationType ObjectSpeedCalculationType)
        {
            AssociatedInteractiveObject = associatedInteractiveObject;
            AITransformMoveManagerComponentV3 = aiTransformMoveManagerComponentV3;
            this.CurrentMovementSpeedAttenuationFactor = InitialMovementSpeedAttenuationFactor;
            this.ObjectSpeedCalculationSystem = new ObjectSpeedCalculationSystem(ObjectSpeedCalculationType);
            this.LastFrameWorldPosition = this.AssociatedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent.transform.position;
        }

        private AIMovementSpeedAttenuationFactor CurrentMovementSpeedAttenuationFactor;

        private Vector3 LastFrameWorldPosition;
        private Vector3 localSpeedDirection;
        private float SpeedMagnitude;


        /// <summary>
        /// Calulcation of the <see cref="LocalSpeedDirection"/> after every objects have been updated
        /// </summary>
        public void AfterTicks()
        {
            var objectTransform = this.AssociatedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent.transform;
            if (this.ObjectSpeedCalculationSystem.ObjectSpeedCalculationType != ObjectSpeedCalculationType.MANUAL)
            {
                var worldDirection = this.LastFrameWorldPosition - objectTransform.position;
                this.localSpeedDirection =
                    new Vector3(Vector3.Project(worldDirection, objectTransform.right).magnitude,
                        Vector3.Project(worldDirection, objectTransform.up).magnitude,
                        Vector3.Project(worldDirection, objectTransform.forward).magnitude).normalized;

                this.SpeedMagnitude = this.ObjectSpeedCalculationSystem.CalculateSpeed(this.AssociatedInteractiveObject, this.AITransformMoveManagerComponentV3);
            }

            this.LastFrameWorldPosition = objectTransform.position;
        }

        public void ManualCalculation(Vector3 worldSpeedDirection, float speedMagnitude)
        {
            this.localSpeedDirection = this.AssociatedInteractiveObject.InteractiveGameObject.GetWorldToLocal().MultiplyVector(worldSpeedDirection);
            this.SpeedMagnitude = speedMagnitude;
        }

        public void ResetSpeed()
        {
            this.SpeedMagnitude = 0f;
            this.localSpeedDirection = Vector3.zero;
        }

        public void SetSpeedAttenuationFactor(AIMovementSpeedAttenuationFactor SpeedAttenuationFactor)
        {
            this.CurrentMovementSpeedAttenuationFactor = SpeedAttenuationFactor;
        }

        public void SetObjectSpeedCalculationType(ObjectSpeedCalculationType ObjectSpeedCalculationType)
        {
            this.ObjectSpeedCalculationSystem.ObjectSpeedCalculationType = ObjectSpeedCalculationType;
        }

        #region Data Retrieval

        public AIMovementSpeedAttenuationFactor GetSpeedAttenuationFactor()
        {
            return this.CurrentMovementSpeedAttenuationFactor;
        }

        public Vector3 GetWorldDirection()
        {
            return this.AssociatedInteractiveObject.InteractiveGameObject.GetLocalToWorld().MultiplyVector(this.localSpeedDirection);
        }

        public Vector3 GetVelocity()
        {
            return this.GetWorldDirection() * this.SpeedMagnitude * this.AITransformMoveManagerComponentV3.SpeedMultiplicationFactor
                   * AIMovementSpeedAttenuationFactors.AIMovementSpeedAttenuationFactorLookup[this.CurrentMovementSpeedAttenuationFactor];
        }

        public Vector3 GetLocalDirectionSpeedAttenuated()
        {
            var speedAttenuationFactorValue = AIMovementSpeedAttenuationFactors.AIMovementSpeedAttenuationFactorLookup[this.CurrentMovementSpeedAttenuationFactor];
            return this.localSpeedDirection.Mul(speedAttenuationFactorValue);
        }

        public float GetSpeedMagnitude()
        {
            return this.SpeedMagnitude;
        }

        #endregion
    }


    public enum ObjectSpeedCalculationType
    {
        MANUAL,
        AGENT
    }

    class ObjectSpeedCalculationSystem
    {
        public ObjectSpeedCalculationType ObjectSpeedCalculationType;

        public ObjectSpeedCalculationSystem(ObjectSpeedCalculationType ObjectSpeedCalculationType)
        {
            this.ObjectSpeedCalculationType = ObjectSpeedCalculationType;
        }

        public float CalculateSpeed(CoreInteractiveObject CoreInteractiveObject, TransformMoveManagerComponentV3 TransformMoveManagerComponentV3)
        {
            var SpeedMagnitude = 0f;
            if (this.ObjectSpeedCalculationType == ObjectSpeedCalculationType.AGENT)
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