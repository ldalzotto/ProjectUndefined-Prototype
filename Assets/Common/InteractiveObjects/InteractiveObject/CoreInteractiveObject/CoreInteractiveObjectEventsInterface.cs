using InteractiveObjects_Interfaces;

namespace InteractiveObjects
{
    public abstract partial class CoreInteractiveObject
    {
        #region Animation Object Events

        public virtual void OnAnimationObjectSetUnscaledSpeedMagnitude(float UnscaledSpeedMagnitude)
        {
        }

        #endregion

        #region AI Events

        public virtual void SetAIDestination(AIDestination AIDestination)
        {
        }

        public virtual void SetAISpeedAttenuationFactor(AIMovementSpeedDefinition AIMovementSpeedDefinition)
        {
        }

        public virtual void OnAIDestinationReached()
        {
        }

        #endregion

        #region Attractive Object Events

        public virtual void OnOtherAttractiveObjectJustIntersected(CoreInteractiveObject OtherInteractiveObject)
        {
        }

        public virtual void OnOtherAttractiveObjectIntersectedNothing(CoreInteractiveObject OtherInteractiveObject)
        {
        }

        public virtual void OnOtherAttractiveObjectNoMoreIntersected(CoreInteractiveObject OtherInteractiveObject)
        {
        }

        #endregion

        #region Disarm Object Events

        public virtual void OnOtherDisarmObjectTriggerEnter(CoreInteractiveObject OtherInteractiveObject)
        {
        }

        public virtual void OnOtherDisarmobjectTriggerExit(CoreInteractiveObject OtherInteractiveObject)
        {
        }

        #endregion
    }
}