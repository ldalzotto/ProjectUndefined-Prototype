using System.Collections.Generic;
using CoreGame;
using Input;
using InteractiveObjects;
using InteractiveObjectAction;
using UnityEngine;

namespace Skill
{
    /// <summary>
    /// The <see cref="SkillSystem"/> is a bag of <see cref="SkillSlot"/>. It's goal is to manage (update and destroy) all visible skill slots by handling :
    /// * Differenciation of <see cref="SkillSlot"/> between the <see cref="MainWeaponSkillSlot"/> and <see cref="SubSkillSlots"/> by their position an inputs
    ///         (see <see cref="MainWeaponSkillSlot"/> and <see cref="SubSkillSlots"/>).
    /// * Switch of associated <see cref="InteractiveObjectActionInherentData"/> for every <see cref="SkillSlot"/> (see <see cref="SetPlayerActionToMainWeaponSkill"/> and <see cref="SetPlayerActionToSubSkill"/>)
    /// /!\ The <see cref="SkillSystem"/> has no other logic than updating and destroying <see cref="SkillSlot"/>.
    /// </summary>
    public class SkillSystem
    {
        /// <summary>
        /// The <see cref="MainWeaponSkillSlot"/> is displayed at the bottom Left of the screen.
        /// It's associated input key is meant to be easily accessibile and repeatable while moving.
        /// </summary>
        private SkillSlot MainWeaponSkillSlot;

        /// <summary>
        /// <see cref="SubSkillSlots"/> are slots displayed at the bottom right of the screen.
        /// </summary>
        private List<SkillSlot> SubSkillSlots = new List<SkillSlot>();

        private IEnumerable<SkillSlot> GetAllSkillSlots()
        {
            yield return MainWeaponSkillSlot;
            foreach (var subSkillSlot in SubSkillSlots)
            {
                yield return subSkillSlot;
            }
        }

        public SkillSystem(CoreInteractiveObject AssociatedInteractiveObject, InteractiveObjectActionPlayerSystem interactiveObjectActionPlayerSystem)
        {
            SKillSlotUIPositionInput SKillSlotUIPositionInput = default(SKillSlotUIPositionInput);
            BuildSKillSlotUIPositionInputForMainWeapon(ref SKillSlotUIPositionInput);
            this.MainWeaponSkillSlot = new SkillSlot(AssociatedInteractiveObject, interactiveObjectActionPlayerSystem, ref SKillSlotUIPositionInput,
                InputID.FIRING_PROJECTILE_DOWN_HOLD);

            BuildSkillSlotForSubSkill(ref SKillSlotUIPositionInput, 0);
            this.SubSkillSlots.Add(new SkillSlot(AssociatedInteractiveObject, interactiveObjectActionPlayerSystem, ref SKillSlotUIPositionInput, InputID.SKILL_1_DOWN_HOLD));
            BuildSkillSlotForSubSkill(ref SKillSlotUIPositionInput, 1);
            this.SubSkillSlots.Add(new SkillSlot(AssociatedInteractiveObject, interactiveObjectActionPlayerSystem, ref SKillSlotUIPositionInput, InputID.SKILL_2_DOWN_HOLD));
            BuildSkillSlotForSubSkill(ref SKillSlotUIPositionInput, 2);
            this.SubSkillSlots.Add(new SkillSlot(AssociatedInteractiveObject, interactiveObjectActionPlayerSystem, ref SKillSlotUIPositionInput, InputID.SKILL_3_DOWN_HOLD));
        }

        public void Tick(float d)
        {
            foreach (var skillSlot in this.GetAllSkillSlots())
            {
                skillSlot.Tick(d);
            }
        }

        public void Destroy()
        {
            foreach (var skillSlot in this.GetAllSkillSlots())
            {
                skillSlot.Destroy();
            }
        }

        private void BuildSKillSlotUIPositionInputForMainWeapon(ref SKillSlotUIPositionInput SKillSlotUIPositionInput)
        {
            SKillSlotUIPositionInput = new SKillSlotUIPositionInput(
                rootPivot: RectTransformSetup.BOTTOM_RIGHT,
                rootSize: new Vector2(100, 100),
                rootLocalPositionInPercentage: new Vector2(0.48f, SkillSlotUI.VerticalLocalPositionInPercentageFromCenter),
                backgroundImageSize: new Vector2(100, 53.9f),
                slotIconSize: new Vector2(70f, 70f)
            );
        }

        private void BuildSkillSlotForSubSkill(ref SKillSlotUIPositionInput SKillSlotUIPositionInput, int position)
        {
            SKillSlotUIPositionInput = new SKillSlotUIPositionInput(
                rootPivot: RectTransformSetup.BOTTOM_LEFT,
                rootSize: new Vector2(100, 100),
                rootLocalPositionInPercentage: new Vector2(-0.48f, SkillSlotUI.VerticalLocalPositionInPercentageFromCenter) + new Vector2(-SkillSlotUI.SubSkillSpaceBetween * position, 0f),
                backgroundImageSize: new Vector2(40f, 40f),
                slotIconSize: new Vector2(35f, 35f)
            );
        }

        #region PlayerAction Set

        public void SetPlayerActionToMainWeaponSkill(InteractiveObjectActionInherentData interactiveObjectActionInherentData)
        {
            this.MainWeaponSkillSlot.SwitchSkillActionDefinition(interactiveObjectActionInherentData);
        }

        public void SetPlayerActionToSubSkill(InteractiveObjectActionInherentData interactiveObjectActionInherentData, int position)
        {
            if (position >= 0 && position <= this.SubSkillSlots.Count - 1)
            {
                this.SubSkillSlots[position].SwitchSkillActionDefinition(interactiveObjectActionInherentData);
            }
        }

        #endregion

        #region Data retrieval

        public InputID GetInputIdAssociatedToTheInteractiveObjectAction(string actionUniqueID)
        {
            foreach (var skillSlot in this.GetAllSkillSlots())
            {
                if (skillSlot.CompareAssociatedInteractiveObjectActionInherentData(actionUniqueID))
                {
                    return skillSlot.AssociatedInput;
                }
            }

            return InputID.NONE;
        }

        #endregion
    }
}