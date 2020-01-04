﻿using System;
using OdinSerializer;
using UnityEngine;

namespace Skill
{
    [Serializable]
    public class SkillSlotUIGlobalConfiguration : SerializedScriptableObject
    {
        public RectTransform SkillSlotUIBasePrefab;
    }
}