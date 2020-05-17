using GameConfigurationID;

namespace Tutorial
{
    public class ActionWheelTutorialStepActionDefinition : TutorialStepActionDefinition
    {
        [CustomEnum()] public DiscussionPositionMarkerID DiscussionPositionMarkerID;

        public override AbstractTutorialTextAction BuildTutorialAction()
        {
            return new ActionWheelTutorialStepAction(this, null);
        }
    }
}