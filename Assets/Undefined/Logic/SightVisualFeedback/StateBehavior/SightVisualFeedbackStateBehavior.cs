using System.Collections.Generic;

namespace SightVisualFeedback
{
    public enum SightVisualFeedbackState
    {
        NONE = 0,
        /// <summary>
        /// The AI is aware that the Target is near and walk/runs towards its
        /// </summary>
        WARNING = 1,
        /// <summary>
        /// The AI is seeing it's target.
        /// </summary>
        DANGER = 2
    }

    /// <summary>
    /// The <see cref="SightVisualFeedbackStateBehavior"/> purpose is :
    ///    - Communcate to the associated <see cref="SightVisualFeedbackSystem"/> the current state (<see cref="SightVisualFeedbackState"/>) that will display the correct icon.
    /// </summary>
    public class SightVisualFeedbackStateBehavior
    {
        private ObjectVariable<SightVisualFeedbackState> SightVisualFeedbackState;
        private SightVisualFeedbackSystem SightVisualFeedbackSystemRef;

        public SightVisualFeedbackStateBehavior(SightVisualFeedbackSystem SightVisualFeedbackSystemRef)
        {
            this.SightVisualFeedbackSystemRef = SightVisualFeedbackSystemRef;
            this.SightVisualFeedbackState = new ObjectVariable<SightVisualFeedbackState>(this.OnSightVisualFeedbackStateChange);
            this.SightVisualFeedbackState.SetValue(SightVisualFeedback.SightVisualFeedbackState.NONE);
        }

        private void OnSightVisualFeedbackStateChange(SightVisualFeedbackState old, SightVisualFeedbackState @new)
        {
            switch (@new)
            {
                case SightVisualFeedback.SightVisualFeedbackState.NONE:
                    this.SightVisualFeedbackSystemRef.Hide();
                    break;
                case SightVisualFeedback.SightVisualFeedbackState.DANGER:
                    this.SightVisualFeedbackSystemRef.Show(SightVisualFeedbackColorType.DANGER);
                    break;
                case SightVisualFeedback.SightVisualFeedbackState.WARNING:
                    this.SightVisualFeedbackSystemRef.Show(SightVisualFeedbackColorType.WARNING);
                    break;
            }
        }

        public void SetSightVisualFeedbackState(SightVisualFeedbackState SightVisualFeedbackState)
        {
            this.SightVisualFeedbackState.SetValue(SightVisualFeedbackState);
        }
    }
}