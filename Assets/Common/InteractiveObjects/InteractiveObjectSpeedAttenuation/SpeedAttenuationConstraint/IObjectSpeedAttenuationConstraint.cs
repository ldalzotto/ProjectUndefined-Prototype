using InteractiveObjects_Interfaces;

namespace InteractiveObjects
{
 
    /// <summary>
    /// Every speed attenuation modification must be made according the constraint <see cref="IObjectSpeedAttenuationConstraint"/>.
    /// The final value of speed attenuation is given by <see cref="ApplyConstraint"/>
    /// </summary>
    public interface IObjectSpeedAttenuationConstraint
    {
        AIMovementSpeedAttenuationFactor ApplyConstraint(AIMovementSpeedAttenuationFactor NewMovementSpeedAttenuationFactor);
    }

    public struct NoneSpeedAttenuationConstraint : IObjectSpeedAttenuationConstraint
    {
        /// <summary>
        /// Does nothing, simply return the <paramref name="NewMovementSpeedAttenuationFactor"/>
        /// </summary>
        public AIMovementSpeedAttenuationFactor ApplyConstraint(AIMovementSpeedAttenuationFactor NewMovementSpeedAttenuationFactor)
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
        public AIMovementSpeedAttenuationFactor ApplyConstraint(AIMovementSpeedAttenuationFactor NewMovementSpeedAttenuationFactor)
        {
            var newSpeedAttenuationValue = AIMovementSpeedAttenuationFactors.AIMovementSpeedAttenuationFactorLookup[NewMovementSpeedAttenuationFactor];

            if (newSpeedAttenuationValue <= AIMovementSpeedAttenuationFactors.AIMovementSpeedAttenuationFactorLookup[this.UpperLimit])
            {
                return NewMovementSpeedAttenuationFactor;
            }
            else
            {
                return this.UpperLimit;
            }
        }
    }
}