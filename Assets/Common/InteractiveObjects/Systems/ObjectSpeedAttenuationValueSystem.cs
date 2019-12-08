using InteractiveObjects_Interfaces;

namespace InteractiveObjects
{
    /// <summary>
    /// Responsible of holding and providing the current <see cref="currentMovementSpeedAttenuationFactor"/>.
    /// The <see cref="currentMovementSpeedAttenuationFactor"/> value must follow a set of constraints controlled by <see cref="currentSpeedAttenuationConstraint"/>.
    /// </summary>
    public class ObjectSpeedAttenuationValueSystem
    {
        private IObjectSpeedAttenuationConstraint currentSpeedAttenuationConstraint;
        private AIMovementSpeedAttenuationFactor currentMovementSpeedAttenuationFactor;
        public AIMovementSpeedAttenuationFactor CurrentMovementSpeedAttenuationFactor => this.currentMovementSpeedAttenuationFactor;

        public ObjectSpeedAttenuationValueSystem(AIMovementSpeedAttenuationFactor InitialMovementSpeedAttenuationFactor)
        {
            this.currentMovementSpeedAttenuationFactor = InitialMovementSpeedAttenuationFactor;
            this.currentSpeedAttenuationConstraint = new NoneSpeedAttenuationConstraint();
        }

        public void SetRule(IObjectSpeedAttenuationConstraint objectSpeedAttenuationConstraint)
        {
            this.currentSpeedAttenuationConstraint = objectSpeedAttenuationConstraint;
        }

        /// <summary>
        /// Setting the <see cref="currentMovementSpeedAttenuationFactor"/> by applying the current constraint <see cref="currentSpeedAttenuationConstraint"/>.
        /// </summary>
        public void SetSpeedAttenuationFactor(AIMovementSpeedAttenuationFactor speedAttenuationFactor)
        {
            this.currentMovementSpeedAttenuationFactor = this.currentSpeedAttenuationConstraint.ApplyConstraint(this.currentMovementSpeedAttenuationFactor, speedAttenuationFactor);
        }
    }

    /// <summary>
    /// Every speed attenuation modification must be made according the constraint <see cref="IObjectSpeedAttenuationConstraint"/>.
    /// The final value of speed attenuation is given by <see cref="ApplyConstraint"/>
    /// </summary>
    public interface IObjectSpeedAttenuationConstraint
    {
        AIMovementSpeedAttenuationFactor ApplyConstraint(AIMovementSpeedAttenuationFactor CurrentMovementSpeedAttenuationFactor, AIMovementSpeedAttenuationFactor NewMovementSpeedAttenuationFactor);
    }

    public struct NoneSpeedAttenuationConstraint : IObjectSpeedAttenuationConstraint
    {
        /// <summary>
        /// Does nothing, simply return the <paramref name="NewMovementSpeedAttenuationFactor"/>
        /// </summary>
        public AIMovementSpeedAttenuationFactor ApplyConstraint(AIMovementSpeedAttenuationFactor CurrentMovementSpeedAttenuationFactor, AIMovementSpeedAttenuationFactor NewMovementSpeedAttenuationFactor)
        {
            return NewMovementSpeedAttenuationFactor;
        }
    }

    public struct NotAboveSpeedAttenuationConstraint : IObjectSpeedAttenuationConstraint
    {
        private AIMovementSpeedAttenuationFactor UpperLimit;

        public NotAboveSpeedAttenuationConstraint(AIMovementSpeedAttenuationFactor upperLimit)
        {
            UpperLimit = upperLimit;
        }

        /// <summary>
        /// Clamps the speed attenuation by the <see cref="UpperLimit"/> value.
        /// </summary>
        public AIMovementSpeedAttenuationFactor ApplyConstraint(AIMovementSpeedAttenuationFactor CurrentMovementSpeedAttenuationFactor, AIMovementSpeedAttenuationFactor NewMovementSpeedAttenuationFactor)
        {
            var oldSpeedAttenuationValue = AIMovementSpeedAttenuationFactors.AIMovementSpeedAttenuationFactorLookup[CurrentMovementSpeedAttenuationFactor];
            var newSpeedAttenuationValue = AIMovementSpeedAttenuationFactors.AIMovementSpeedAttenuationFactorLookup[NewMovementSpeedAttenuationFactor];

            if (newSpeedAttenuationValue <= oldSpeedAttenuationValue || newSpeedAttenuationValue <= AIMovementSpeedAttenuationFactors.AIMovementSpeedAttenuationFactorLookup[this.UpperLimit])
            {
                return NewMovementSpeedAttenuationFactor;
            }
            else
            {
                return CurrentMovementSpeedAttenuationFactor;
            }
        }
    }
}