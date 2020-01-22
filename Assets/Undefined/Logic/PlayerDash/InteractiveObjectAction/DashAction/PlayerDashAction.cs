using InteractiveObjectAction;
using InteractiveObjects;
using UnityEngine;

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

        private PlayerDashActionDefinition PlayerDashActionDefinition;
        private DashTeleportationActionDefinitionInput DashTeleportationActionDefinitionInput;

        public PlayerDashAction(PlayerDashActionDefinition PlayerDashActionDefinition,
            DashTeleportationActionDefinitionInput DashTeleportationActionDefinitionInput) : base(PlayerDashActionDefinition.coreInteractiveObjectActionDefinition)
        {
            this.PlayerDashActionDefinition = PlayerDashActionDefinition;
            this.DashTeleportationActionDefinitionInput = DashTeleportationActionDefinitionInput;
        }

        public override string InteractiveObjectActionUniqueID
        {
            get { return DashTeleportationActionUniqueID; }
        }

        public override void FirstExecution()
        {
            var startParticles = GameObject.Instantiate<ParticleSystem>(this.PlayerDashActionDefinition.StartPositionParticleSystemPrefab);
            startParticles.transform.position =
                this.DashTeleportationActionDefinitionInput.AssociatedInteractiveObject.InteractiveGameObject.GetAverageModelWorldBounds().center;
            var startPosition = this.DashTeleportationActionDefinitionInput.AssociatedInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition;
            startParticles.transform.rotation = Quaternion.LookRotation((this.DashTeleportationActionDefinitionInput.TargetWorldPoint - startPosition).normalized);
            (this.DashTeleportationActionDefinitionInput.AssociatedInteractiveObject as IEM_PlayerDashAction).DashToWorldPosition(this.DashTeleportationActionDefinitionInput.TargetWorldPoint);
        }

        public override bool FinishedCondition()
        {
            return true;
        }
    }
}