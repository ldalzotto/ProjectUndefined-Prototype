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
    }

}