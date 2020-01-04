using Input;
using InteractiveObjects;
using PlayerActions;

namespace Skill
{
    /// <summary>
    /// SkillSlot is a layer between the <see cref="CoreInteractiveObject"/> and the execution of the <see cref="PlayerAction"/>.
    /// A SkillSlot assciates an Input (<see cref="AssociatedInput"/>) to a PlayerAction (<see cref="AssociatedPlayerActionInherentData"/>).
    /// The SkillAction has no other logic than triggering the associated PlayerAction when the associated input is pressed.
    /// </summary>
    public class SkillSlot
    {
        #region Input

        /// <summary>
        /// The Input that used to trigger the <see cref="AssociatedPlayerActionInherentData"/>.
        /// </summary>
        private InputID AssociatedInput;

        private GameInputManager GameInputManager = GameInputManager.Get();

        #endregion

        private CoreInteractiveObject AssociatedInteractiveObject;
        private PlayerActionPlayerSystem PlayerActionPlayerSystem;

        #region UI

        private SkillSlotUI SkillSlotUI;

        #endregion

        public SkillSlot(CoreInteractiveObject AssociatedInteractiveObject, PlayerActionPlayerSystem PlayerActionPlayerSystem, InputID AssociatedInput)
        {
            this.AssociatedInteractiveObject = AssociatedInteractiveObject;
            this.PlayerActionPlayerSystem = PlayerActionPlayerSystem;
            this.AssociatedInput = AssociatedInput;
            this.SkillSlotUI = new SkillSlotUI();
        }

        public void Destroy()
        {
            this.SkillSlotUI.Destroy();
        }

        /// <summary>
        /// /!\ <see cref="AssociatedPlayerActionInherentData"/> can be switched at runtime.
        /// </summary>
        private PlayerActionInherentData AssociatedPlayerActionInherentData;

        public void SwitchAssociatedPlayerAction(PlayerActionInherentData playerActionInherentData)
        {
            this.AssociatedPlayerActionInherentData = playerActionInherentData;
        }

        public void Tick(float d)
        {
            if (this.AssociatedPlayerActionInherentData != null)
            {
                this.SkillSlotUI.SetCooldownProgress(this.PlayerActionPlayerSystem.GetCooldownPercentageOfPlayerAction(this.AssociatedPlayerActionInherentData.PlayerActionUniqueID));
                if (this.GameInputManager.CurrentInput.EvaluateInputCondition(this.AssociatedInput))
                {
                    this.PlayerActionPlayerSystem.ExecuteActionV2(this.AssociatedPlayerActionInherentData);
                }
            }
        }
    }
}