using CoreGame;
using Input;
using InteractiveObjects;
using PlayerActions;
using UnityEngine;

namespace Skill
{
    /// <summary>
    /// SkillSlot is a layer between the <see cref="CoreInteractiveObject"/> and the execution of the <see cref="PlayerAction"/>.
    /// A SkillSlot assciates an Input (<see cref="AssociatedInput"/>) to a PlayerAction (<see cref="AssociatedPlayerActionInherentData"/>).
    /// The SkillSlot object is in charge of handling the UI representation of the skill <see cref="SkillSlotUI"/>
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

        /// <summary>
        /// The associated UI representation of the skill slot.
        /// </summary>
        private SkillSlotUI SkillSlotUI;

        #endregion

        /// <summary>
        /// /!\ <see cref="PlayerActionInherentData"/> can be switched at runtime.
        /// </summary>
        private ObjectVariable<PlayerActionInherentData> CurrentPlayerActionInherentData;

        public SkillSlot(CoreInteractiveObject AssociatedInteractiveObject, PlayerActionPlayerSystem PlayerActionPlayerSystem, InputID AssociatedInput)
        {
            this.AssociatedInteractiveObject = AssociatedInteractiveObject;
            this.PlayerActionPlayerSystem = PlayerActionPlayerSystem;
            this.AssociatedInput = AssociatedInput;
            this.SkillSlotUI = new SkillSlotUI(new SKillSlotUIPositionInput(
                rootPivot: RectTransformSetup.BOTTOM_RIGHT,
                rootSize: new Vector2(100, 100),
                rootLocalPositionInPercentage: new Vector2(0.48f, SkillSlotUI.VerticalLocalPositionInPercentageFromCenter),
                backgroundImageSize: new Vector2(100, 53.9f),
                slotIconSize: new Vector2(70f, 70f)
            ));
            this.CurrentPlayerActionInherentData = new ObjectVariable<PlayerActionInherentData>(this.OnCurrentPlayerActionInherentDataChanged);
        }

        public void Destroy()
        {
            this.SkillSlotUI.Destroy();
        }


        public void SwitchSkillActionDefinition(PlayerActionInherentData PlayerActionInherentData)
        {
            this.CurrentPlayerActionInherentData.SetValue(PlayerActionInherentData);
        }

        private void OnCurrentPlayerActionInherentDataChanged(PlayerActionInherentData OldPlayerActionInherentData, PlayerActionInherentData NewPlayerActionInherentData)
        {
            if (NewPlayerActionInherentData != null)
            {
                this.SkillSlotUI.OnSkillActionChanged(NewPlayerActionInherentData.CorePlayerActionDefinition.GetSkillActionDefinition());
            }
        }

        public void Tick(float d)
        {
            if (this.CurrentPlayerActionInherentData.GetValue() != null)
            {
                this.SkillSlotUI.SetCooldownProgress(this.PlayerActionPlayerSystem.GetCooldownPercentageOfPlayerAction(this.CurrentPlayerActionInherentData.GetValue().PlayerActionUniqueID));
                if (this.GameInputManager.CurrentInput.EvaluateInputCondition(this.AssociatedInput))
                {
                    this.PlayerActionPlayerSystem.ExecuteActionV2(this.CurrentPlayerActionInherentData.GetValue());
                }
            }
        }
    }
}