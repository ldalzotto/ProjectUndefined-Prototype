using CoreGame;
using Input;
using InteractiveObjects;
using InteractiveObjectAction;
using UnityEngine;

namespace Skill
{
    /// <summary>
    /// SkillSlot is a layer between the <see cref="SkillSystem"/> and the execution of the <see cref="PlayerAction"/>.
    /// A SkillSlot assciates an Input (<see cref="AssociatedInput"/>) to a PlayerAction (<see cref="AssociatedPlayerActionInherentData"/>).
    /// The SkillSlot object is in charge of handling the UI representation of the skill <see cref="SkillSlotUI"/>
    /// /!\ The SkillAction has no other logic than triggering the playing associated PlayerAction when the associated input is pressed via the <see cref="_interactiveObjectActionPlayerSystem"/> provided.
    /// </summary>
    public class SkillSlot
    {
        #region Input

        /// <summary>
        /// The Input that used to trigger the <see cref="AssociatedPlayerActionInherentData"/>.
        /// </summary>
        public InputID AssociatedInput { get; private set; }

        private GameInputManager GameInputManager = GameInputManager.Get();

        #endregion

        private CoreInteractiveObject AssociatedInteractiveObject;

        /// <summary>
        /// The <see cref="_interactiveObjectActionPlayerSystem"/> is used to play the <see cref="CurrentPlayerActionInherentData"/> when the <see cref="AssociatedInput"/> has been pressed.
        /// </summary>
        private InteractiveObjectActionPlayerSystem _interactiveObjectActionPlayerSystem;

        #region UI

        /// <summary>
        /// The associated UI representation of the skill slot.
        /// </summary>
        private SkillSlotUI SkillSlotUI;

        #endregion

        /// <summary>
        /// /!\ <see cref="InteractiveObjectActionInherentData"/> can be switched at runtime.
        /// </summary>
        private ObjectVariable<InteractiveObjectActionInherentData> CurrentPlayerActionInherentData;

        public SkillSlot(CoreInteractiveObject AssociatedInteractiveObject, InteractiveObjectActionPlayerSystem interactiveObjectActionPlayerSystem, ref SKillSlotUIPositionInput SKillSlotUIPositionInput,
            InputID AssociatedInput)
        {
            this.AssociatedInteractiveObject = AssociatedInteractiveObject;
            this._interactiveObjectActionPlayerSystem = interactiveObjectActionPlayerSystem;
            this.AssociatedInput = AssociatedInput;
            this.SkillSlotUI = new SkillSlotUI(ref SKillSlotUIPositionInput);
            this.CurrentPlayerActionInherentData = new ObjectVariable<InteractiveObjectActionInherentData>(this.OnCurrentPlayerActionInherentDataChanged);

            this.SkillSlotUI.SetInputText(InputControlLookup.GetInputControlRawName(InputControlLookup.FindTheFirstInputControlForInputID(this.AssociatedInput)));
        }

        public void Destroy()
        {
            this.SkillSlotUI.Destroy();
        }


        public void SwitchSkillActionDefinition(InteractiveObjectActionInherentData interactiveObjectActionInherentData)
        {
            this.CurrentPlayerActionInherentData.SetValue(interactiveObjectActionInherentData);
        }

        private void OnCurrentPlayerActionInherentDataChanged(InteractiveObjectActionInherentData oldInteractiveObjectActionInherentData, InteractiveObjectActionInherentData newInteractiveObjectActionInherentData)
        {
            if (newInteractiveObjectActionInherentData != null)
            {
                this.SkillSlotUI.OnSkillActionChanged(newInteractiveObjectActionInherentData.coreInteractiveObjectActionDefinition.GetSkillActionDefinition());
            }
        }

        public void Tick(float d)
        {
            if (this.CurrentPlayerActionInherentData.GetValue() != null)
            {
                this.SkillSlotUI.SetCooldownProgress(this._interactiveObjectActionPlayerSystem.GetCooldownPercentageOfPlayerAction(this.CurrentPlayerActionInherentData.GetValue().InteractiveObjectActionUniqueID));
                if (this.GameInputManager.CurrentInput.EvaluateInputCondition(this.AssociatedInput))
                {
                    this._interactiveObjectActionPlayerSystem.ExecuteActionV2(this.CurrentPlayerActionInherentData.GetValue());
                }
            }
        }
        
        /// <summary>
        /// <see cref="CurrentPlayerActionInherentData"/> must be able to be triggered even if time is frozen.
        /// The cooldown state will be initialized but as the passed delta time is 0, cooldown value won't change.
        /// </summary>
        public void TickTimeFrozen(float d)
        {
            this.Tick(d);
        }

        #region Logical conditions

        /// <summary>
        /// Returns true if the input <paramref name="actionUniqueID"/> is equal to the <see cref="CurrentPlayerActionInherentData"/>. 
        /// </summary>
        public bool CompareAssociatedInteractiveObjectActionInherentData(string actionUniqueID)
        {
            return this.CurrentPlayerActionInherentData.GetValue() != null && this.CurrentPlayerActionInherentData.GetValue().AreTheSameID(actionUniqueID);
        }

        #endregion

       
    }
}