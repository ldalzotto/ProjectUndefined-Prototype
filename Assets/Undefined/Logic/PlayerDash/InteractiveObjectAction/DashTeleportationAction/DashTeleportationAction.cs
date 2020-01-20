using InteractiveObjectAction;

namespace PlayerDash
{
    public class DashTeleportationAction : AInteractiveObjectAction
    {
        public const string DashTeleportationActionUniqueID = "DashTeleportationAction";

        private DashTeleportationActionDefinitionInput DashTeleportationActionDefinitionInput;

        public DashTeleportationAction(CoreInteractiveObjectActionDefinition coreInteractiveObjectActionDefinition,
            DashTeleportationActionDefinitionInput DashTeleportationActionDefinitionInput) : base(coreInteractiveObjectActionDefinition)
        {
            this.DashTeleportationActionDefinitionInput = DashTeleportationActionDefinitionInput;
        }

        public override string InteractiveObjectActionUniqueID
        {
            get { return DashTeleportationActionUniqueID; }
        }

        public override void FirstExecution()
        {
            this.DashTeleportationActionDefinitionInput.AssociatedInteractiveObject.InteractiveGameObject.PhysicsRigidbody.MovePosition(this.DashTeleportationActionDefinitionInput.TargetWorldPoint);
        }

        public override bool FinishedCondition()
        {
            return true;
        }
    }
}