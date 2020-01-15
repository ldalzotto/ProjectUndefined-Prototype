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
    /// /!\ The word "Unscaled" means that the value is between 0 and 1. The speed value is unscaled relatively to <see cref="TransformMoveManagerComponentV3.SpeedMultiplicationFactor"/>.
    /// /!\ The word "Attenuated" means that the speed is multiplied by the dynamic attenuation factor <see cref="GetSpeedAttenuationFactor"/>.
    /// </summary>
    public class ObjectMovementSpeedSystem
    {
        #region External Dependencies

        private CoreInteractiveObject AssociatedInteractiveObject;
        private TransformMoveManagerComponentV3 AITransformMoveManagerComponentV3;

        #endregion

        private ObjectSpeedCalculationType ObjectSpeedCalculationType;

        [VE_Nested] private IObjectSpeedAttenuationValueSystem ObjectSpeedAttenuationValueSystem;

        public ObjectMovementSpeedSystem(CoreInteractiveObject associatedInteractiveObject, TransformMoveManagerComponentV3 aiTransformMoveManagerComponentV3,
            IObjectSpeedAttenuationValueSystem ObjectSpeedAttenuationValueSystem, ObjectSpeedCalculationType InitialObjectSpeedCalculationType)
        {
            AssociatedInteractiveObject = associatedInteractiveObject;
            AITransformMoveManagerComponentV3 = aiTransformMoveManagerComponentV3;
            this.ObjectSpeedCalculationType = InitialObjectSpeedCalculationType;
            this.ObjectSpeedAttenuationValueSystem = ObjectSpeedAttenuationValueSystem;

            this.ObjectMovementSpeedSystemState = new ObjectMovementSpeedSystemState()
            {
                LastFrameWorldPosition = this.AssociatedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent.transform.position,
            };
        }

        [VE_Nested] private ObjectMovementSpeedSystemState ObjectMovementSpeedSystemState;

        /// <summary>
        /// Calulcation of the <see cref="ObjectMovementSpeedSystemState.LocalSpeedDirection"/> after every objects have been updated.
        /// The importance of execution order lies in the fact that we must be sure that the object position will be it's final displayed position.
        /// </summary>
        public void AfterTicks(float d)
        {
            var objectTransform = this.AssociatedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent.transform;
            if (this.ObjectSpeedCalculationType == ObjectSpeedCalculationType.AGENT)
            {
                this.AssociatedInteractiveObject.InteractiveGameObject.Agent.speed
                    = this.AITransformMoveManagerComponentV3.SpeedMultiplicationFactor * AIMovementSpeedAttenuationFactors.AIMovementSpeedAttenuationFactorLookup[this.GetSpeedAttenuationFactor()];

                var worldDirection = this.ObjectMovementSpeedSystemState.LastFrameWorldPosition - objectTransform.position;
                this.ObjectMovementSpeedSystemState.localSpeedDirection =
                    new Vector3(Vector3.Project(worldDirection, objectTransform.right).magnitude,
                        Vector3.Project(worldDirection, objectTransform.up).magnitude,
                        Vector3.Project(worldDirection, objectTransform.forward).magnitude).normalized;

                this.ObjectMovementSpeedSystemState.EffectiveSpeedMagnitude_Unscaled_Attenuated =
                    ObjectSpeedCalculationSystem.CalculateEffectiveSpeed_Unscaled_Attenuated(this.ObjectSpeedCalculationType, this.AssociatedInteractiveObject, ObjectMovementSpeedSystemState.LastFrameWorldPosition,
                        this.AITransformMoveManagerComponentV3, d);
                this.ObjectMovementSpeedSystemState.DesiredSpeedMagnitude_Unscaled_Attenuated =
                    ObjectSpeedCalculationSystem.CalculateDesiredSpeed_Unscaled_Attenuated(this.ObjectSpeedCalculationType, this.AssociatedInteractiveObject, this.GetSpeedAttenuationFactor());
            }

            this.ObjectMovementSpeedSystemState.LastFrameWorldPosition = objectTransform.position;
        }

        /// <summary>
        /// This method overrides calculation data contained in <see cref="ObjectMovementSpeedSystemState"/> by data passed as input.
        /// /!\ This method must be called only if <see cref="ObjectSpeedCalculationType"/> is set to <see cref="ObjectSpeedCalculationType.MANUAL"/>.
        /// </summary>
        /// <param name="desiredSpeedMagnitudeUnscaled">The desired speed magnitude is unscaled means that it's value is clamped between 0 and 1.
        /// Setted <see cref="ObjectMovementSpeedSystemState.SpeedMagnitude"/> will be rescaled with the <see cref="ObjectMovementSpeedSystemState.CurrentMovementSpeedAttenuationFactor"/>.</param>
        public void ManualCalculation(Vector3 worldSpeedDirection, float desiredSpeedMagnitudeUnscaled, float effectiveSpeedMagnitudeScaled)
        {
            this.ObjectMovementSpeedSystemState.localSpeedDirection = this.AssociatedInteractiveObject.InteractiveGameObject.GetWorldToLocal().MultiplyVector(worldSpeedDirection);

            /// Speed magnitude is multiplied with CurrentMovementSpeedAttenuationFactor because input is normalized.
            this.ObjectMovementSpeedSystemState.DesiredSpeedMagnitude_Unscaled_Attenuated = desiredSpeedMagnitudeUnscaled * AIMovementSpeedAttenuationFactors.AIMovementSpeedAttenuationFactorLookup[this.GetSpeedAttenuationFactor()];
            this.ObjectMovementSpeedSystemState.EffectiveSpeedMagnitude_Unscaled_Attenuated = effectiveSpeedMagnitudeScaled / this.AITransformMoveManagerComponentV3.SpeedMultiplicationFactor;
        }

        public void ResetSpeed()
        {
            this.ObjectMovementSpeedSystemState.localSpeedDirection = Vector3.zero;
            this.ObjectMovementSpeedSystemState.DesiredSpeedMagnitude_Unscaled_Attenuated = 0f;
        }

        public void SetSpeedAttenuationFactor(AIMovementSpeedAttenuationFactor SpeedAttenuationFactor)
        {
            this.ObjectSpeedAttenuationValueSystem.SetSpeedAttenuationFactor(SpeedAttenuationFactor);
        }

        public void SetObjectSpeedCalculationType(ObjectSpeedCalculationType ObjectSpeedCalculationType)
        {
            this.ObjectSpeedCalculationType = ObjectSpeedCalculationType;
        }

        #region Data Retrieval

        private AIMovementSpeedAttenuationFactor GetSpeedAttenuationFactor()
        {
            return this.ObjectSpeedAttenuationValueSystem.CurrentMovementSpeedAttenuationFactor();
        }

        public Vector3 GetWorldSpeedDirection()
        {
            return this.AssociatedInteractiveObject.InteractiveGameObject.GetLocalToWorld().MultiplyVector(this.ObjectMovementSpeedSystemState.localSpeedDirection);
        }

        /// <summary>
        /// The velocity is the projection of <see cref="ObjectMovementSpeedSystemState.localSpeedDirection"/>. It takes into account all speed scale factors.
        /// </summary>
        public Vector3 GetEffectiveVelocity_Scaled_Attenuated()
        {
            return this.GetWorldSpeedDirection() * this.ObjectMovementSpeedSystemState.EffectiveSpeedMagnitude_Unscaled_Attenuated * this.AITransformMoveManagerComponentV3.SpeedMultiplicationFactor
                   * AIMovementSpeedAttenuationFactors.AIMovementSpeedAttenuationFactorLookup[this.GetSpeedAttenuationFactor()];
        }

        public Vector3 GetDesiredVelocity_Scaled_Attenuated()
        {
            return this.GetWorldSpeedDirection() * this.ObjectMovementSpeedSystemState.DesiredSpeedMagnitude_Unscaled_Attenuated * this.AITransformMoveManagerComponentV3.SpeedMultiplicationFactor
                   * AIMovementSpeedAttenuationFactors.AIMovementSpeedAttenuationFactorLookup[this.GetSpeedAttenuationFactor()];
        }

        /// <summary>
        /// Calculates the <see cref="ObjectMovementSpeedSystemState.localSpeedDirection"/> only attenuated by <see cref="AIMovementSpeedAttenuationFactors"/>.
        /// This is mainly used for animation graph that needs speed as input. It is useful when the <see cref="TransformMoveManagerComponentV3.SpeedMultiplicationFactor"/> is not
        /// necessary for the calculation.
        /// </summary>
        public Vector3 GetLocalSpeedDirection_Attenuated()
        {
            var speedAttenuationFactorValue = AIMovementSpeedAttenuationFactors.AIMovementSpeedAttenuationFactorLookup[this.GetSpeedAttenuationFactor()];
            return this.ObjectMovementSpeedSystemState.localSpeedDirection.Mul(speedAttenuationFactorValue);
        }

        public float GetSpeedMagnitude()
        {
            return this.ObjectMovementSpeedSystemState.DesiredSpeedMagnitude_Unscaled_Attenuated;
        }

        #endregion
    }

    struct ObjectMovementSpeedSystemState
    {
        public Vector3 LastFrameWorldPosition;
        public Vector3 localSpeedDirection;
        
        /// <summary>
        /// The Desired speed is the speed that the system wants to reach.
        /// For example we want a RigidBody to move at the speed of 10 but in the physics world, it's speed may not be 10 (because of obstacles).
        /// </summary>
        public float DesiredSpeedMagnitude_Unscaled_Attenuated;
        
        /// <summary>
        /// As opposed to <see cref="DesiredSpeedMagnitude_Unscaled_Attenuated"/>, this speed is the real world speed.
        /// It's value is generally calculated from the distance from last frame.
        /// </summary>
        public float EffectiveSpeedMagnitude_Unscaled_Attenuated;
    }

    static class ObjectSpeedCalculationSystem
    {
        public static float CalculateDesiredSpeed_Unscaled_Attenuated(ObjectSpeedCalculationType ObjectSpeedCalculationType,
            CoreInteractiveObject CoreInteractiveObject,
            AIMovementSpeedAttenuationFactor SpeedAttenuationFactor)
        {
            var SpeedMagnitude = 0f;
            if (ObjectSpeedCalculationType == ObjectSpeedCalculationType.AGENT)
            {
                var agent = CoreInteractiveObject.InteractiveGameObject.Agent;
                if (agent != null && agent.hasPath)
                {
                    SpeedMagnitude = AIMovementSpeedAttenuationFactors.AIMovementSpeedAttenuationFactorLookup[SpeedAttenuationFactor];
                }
            }

            return SpeedMagnitude;
        }

        public static float CalculateEffectiveSpeed_Unscaled_Attenuated(ObjectSpeedCalculationType ObjectSpeedCalculationType,
            CoreInteractiveObject CoreInteractiveObject,
            Vector3 LastFramePosition,
            TransformMoveManagerComponentV3 TransformMoveManagerComponentV3,
            float d)
        {
            var SpeedMagnitude = 0f;

            if (ObjectSpeedCalculationType == ObjectSpeedCalculationType.AGENT)
            {
                var agent = CoreInteractiveObject.InteractiveGameObject.Agent;
                if (agent != null && agent.hasPath)
                {
                    SpeedMagnitude = Vector3.Distance(agent.transform.position, LastFramePosition) / d;
                    SpeedMagnitude /= TransformMoveManagerComponentV3.SpeedMultiplicationFactor;
                }
            }

            return SpeedMagnitude;
        }
    }
}