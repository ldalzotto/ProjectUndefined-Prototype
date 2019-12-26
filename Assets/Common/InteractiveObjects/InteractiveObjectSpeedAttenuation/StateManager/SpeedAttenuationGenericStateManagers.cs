using System;
using InteractiveObjects_Interfaces;

namespace InteractiveObjects
{
    /// <summary>
    /// The speed attenuation calculation <see cref="ObjectSpeedAttenuationCalculation"/> may involve speed constraints which are rules that newly setted speed must comply.
    /// The <see cref="SpeedAttenuationContraintStateManager"/> is an abstract definition of a state manager aiming to provide the current constraint <see cref="AssociatedIObjectSpeedAttenuationConstraint"/>
    /// applied to the speed attenuation factor. 
    /// </summary>
    public abstract class SpeedAttenuationContraintStateManager : StateManager
    {
        protected IObjectSpeedAttenuationConstraint AssociatedIObjectSpeedAttenuationConstraint;

        public IObjectSpeedAttenuationConstraint AssociatedSpeedAttenuationConstraint()
        {
            return this.AssociatedIObjectSpeedAttenuationConstraint;
        }
    }

    public interface ISpeedAttenuationConstraintLayer
    {
        IObjectSpeedAttenuationConstraint GetCurrentSpeedAttenuationConstraint();
    }

    public abstract class SpeedAttenuationConstraintLayer<S, SM> : StateBehavior<S, SM>, ISpeedAttenuationConstraintLayer where S : Enum where SM : SpeedAttenuationContraintStateManager
    {
        public IObjectSpeedAttenuationConstraint GetCurrentSpeedAttenuationConstraint()
        {
            return this.GetCurrentStateManager().AssociatedSpeedAttenuationConstraint();
        }
    }

    /// <summary>
    /// The speed attenuation calculation <see cref="ObjectSpeedAttenuationCalculation"/> needs the value of the currenlty setted speed attenuation factor.
    /// The <see cref="SpeedAttenuationValueStateManager"/> is an abstract definition of a state manager aiming to provide the current setted speed attenuation factor <see cref="AssociatedAIMovementSpeedAttenuationFactor"/>.
    /// </summary>
    public abstract class SpeedAttenuationValueStateManager : StateManager
    {
        protected AIMovementSpeedAttenuationFactor AssociatedAIMovementSpeedAttenuationFactor;

        public AIMovementSpeedAttenuationFactor AssociatedMovementSpeedAttenuationFactor()
        {
            return this.AssociatedAIMovementSpeedAttenuationFactor;
        }
    }

    public interface ISpeedAttenuationValueLayer
    {
        AIMovementSpeedAttenuationFactor GetCurrentSpeedAttenuationFactor();
    }

    public abstract class SpeedAttenuationValueLayer<S, SM> : StateBehavior<S, SM>, ISpeedAttenuationValueLayer where S : Enum where SM : SpeedAttenuationValueStateManager
    {
        public AIMovementSpeedAttenuationFactor GetCurrentSpeedAttenuationFactor()
        {
            return this.GetCurrentStateManager().AssociatedMovementSpeedAttenuationFactor();
        }
    }
}