using System;
using InteractiveObjects_Interfaces;

namespace InteractiveObjects
{
    /// <summary>
    /// The interface of the system that holds and manage the associated <see cref="ObjectMovementSpeedSystem"/> <see cref="AIMovementSpeedAttenuationFactor"/>.
    /// Any set and retrieval of speed attenuation must be done via this interface.
    /// </summary>
    public interface IObjectSpeedAttenuationValueSystem
    {
        AIMovementSpeedAttenuationFactor CurrentMovementSpeedAttenuationFactor();
        void SetSpeedAttenuationFactor(AIMovementSpeedAttenuationFactor speedAttenuationFactor);
    }

    public static class ObjectSpeedAttenuationCalculation
    {
        /// <summary>
        /// The calculation of current speed attenuation factor is the current value retrieved from <see cref="ISpeedAttenuationValueLayer"/> filtered from constraint <see cref="ISpeedAttenuationConstraintLayer"/>
        /// </summary>
        /// <returns></returns>
        public static AIMovementSpeedAttenuationFactor CalculateAIMovementSpeedAttenuationFactor(ISpeedAttenuationConstraintLayer ISpeedAttenuationConstraintLayer,
            ISpeedAttenuationValueLayer ISpeedAttenuationValueLayer)
        {
            return ISpeedAttenuationConstraintLayer.GetCurrentSpeedAttenuationConstraint().ApplyConstraint(ISpeedAttenuationValueLayer.GetCurrentSpeedAttenuationFactor());
        }
    }


}