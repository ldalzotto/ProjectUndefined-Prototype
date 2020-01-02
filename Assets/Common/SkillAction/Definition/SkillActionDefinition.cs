using System;
using OdinSerializer;
using UnityEngine;

namespace SkillAction
{
    [Serializable]
    public abstract class SkillActionDefinition : SerializedScriptableObject
    {
        public SkillActionDefinitionStruct SkillActionDefinitionStruct;
    }

    [Serializable]
    public struct SkillActionDefinitionStruct
    {
        public float CoolDownTime;
    }
}