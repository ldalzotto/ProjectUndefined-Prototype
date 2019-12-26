using InteractiveObjects_Interfaces;

namespace InteractiveObjects
{
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