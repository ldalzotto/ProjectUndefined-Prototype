using System;
using OdinSerializer;
using UnityEngine;

namespace Skill
{
    [Serializable]
    public class SkillSlotUIGlobalConfiguration : SerializedScriptableObject
    {
        public GameObject SkillSlotUIBasePrefab;

        /// <summary>
        /// The <see cref="SkillSlotUI"/> background color when the <see cref="SkillActionDefinition.SkillActionConstraint"/> is set to <see cref="SkillActionConstraint.LOW_HEALTH_ONLY"/>.
        /// </summary>
        public Color LowOnHealthConstrainedBackgroundColor;
    }
}