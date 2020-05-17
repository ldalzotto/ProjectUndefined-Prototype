using GameConfigurationID;

namespace Tutorial
{
    public class MovementTutorialTextActionDefinition : TutorialStepActionDefinition
    {
        [CustomEnum()] public DiscussionPositionMarkerID DiscussionPositionMarkerID;
        public float PlayerCrossedDistance;

        public override AbstractTutorialTextAction BuildTutorialAction()
        {
            return new MovementTutorialTextAction(this, null);
        }
    }
}