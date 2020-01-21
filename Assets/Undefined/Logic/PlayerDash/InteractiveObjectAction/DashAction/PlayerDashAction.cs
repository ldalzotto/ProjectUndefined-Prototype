using InteractiveObjectAction;
using InteractiveObjects;

namespace PlayerDash
{
    /// <summary>
    /// The <see cref="PlayerDashAction"/> allows the associated <see cref="CoreInteractiveObject"/> to instantantly warp to the
    /// <see cref="DashTeleportationActionDefinitionInput.TargetWorldPoint"> passed as input.
    /// /!\ This action only performs the teleportation. This is because it is executed from a Skill. Thus, additional skill cooldown logic is applyed to this action. 
    /// </summary>
    public class PlayerDashAction : AInteractiveObjectAction
    {
        public const string DashTeleportationActionUniqueID = "DashTeleportationAction";

        private DashTeleportationActionDefinitionInput DashTeleportationActionDefinitionInput;

        public PlayerDashAction(CoreInteractiveObjectActionDefinition coreInteractiveObjectActionDefinition,
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