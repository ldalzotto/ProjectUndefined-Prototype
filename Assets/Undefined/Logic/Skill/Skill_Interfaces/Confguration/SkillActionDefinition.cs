using System;
using OdinSerializer;
using UnityEngine;
using UnityEngine.UI;

namespace Skill
{
    /// <summary>
    /// The <see cref="SkillActionDefinition"/> is associated to a PlayerActionInherentData.
    /// If available, the additional skill informations will be extracted by the <see cref="SkillSlot"/> (see <see cref="SkillSlot.OnCurrentPlayerActionInherentDataChanged"/>)
    /// to feed skill specific informations.
    /// </summary>
    [Serializable]
    public class SkillActionDefinition : SerializedScriptableObject
    {
        public Sprite SkillIcon;
        public SkillActionConstraint SkillActionConstraint;
    }

    /// <summary>
    /// The <see cref="SkillActionConstraint"/> acts as a filter when we gets all available skill slots from criteria.
    ///  - <see cref="SkillActionConstraint.LOW_HEALTH_ONLY"/> : Implies that the <see cref="SkillSlotUI"/> background color is changed to <see cref="SkillSlotUIGlobalConfiguration.LowOnHealthConstrainedBackgroundColor"/>.
    ///     Skill slots can be retrieved with <see cref="SkillSystem.GetAllSkillSlotsThatAreLowOnHealthConstrainted"/>.
    /// </summary>
    [Serializable]
    public enum SkillActionConstraint
    {
        NONE = 0,
        LOW_HEALTH_ONLY = 1
    }

}