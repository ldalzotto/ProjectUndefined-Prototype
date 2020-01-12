using InteractiveObjects_Interfaces;

namespace InteractiveObjects
{
    /// <summary>
    /// The <see cref="CurrentAIMovementSpeedAttenuationFactor"/> is not constrained. Which means that speed can be freely set from any source.
    /// </summary>
    public class UnConstrainedObjectSpeedAttenuationValueSystem : IObjectSpeedAttenuationValueSystem
    {
        private AIMovementSpeedAttenuationFactor CurrentAIMovementSpeedAttenuationFactor;

        public UnConstrainedObjectSpeedAttenuationValueSystem(AIMovementSpeedAttenuationFactor initialSpeedAttenuationFactor)
        {
            CurrentAIMovementSpeedAttenuationFactor = initialSpeedAttenuationFactor;
        }

        public AIMovementSpeedAttenuationFactor CurrentMovementSpeedAttenuationFactor()
        {
            return this.CurrentAIMovementSpeedAttenuationFactor;
        }

        public void SetSpeedAttenuationFactor(AIMovementSpeedAttenuationFactor speedAttenuationFactor)
        {
            this.CurrentAIMovementSpeedAttenuationFactor = speedAttenuationFactor;
        }
    }
}