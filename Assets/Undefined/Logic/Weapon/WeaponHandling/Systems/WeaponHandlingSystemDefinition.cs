using System;
using OdinSerializer;

namespace Weapon
{
    [Serializable]
    [SceneHandleDraw]
    public class WeaponHandlingSystemDefinition : SerializedScriptableObject
    {
        [Inline(CreateAtSameLevelIfAbsent = true)]
        [DrawNested]
        public WeaponHandlingFirePointOriginLocalDefinition WeaponHandlingFirePointOriginLocalDefinition;

        [Inline()] public WeaponDefinition WeaponDefinition;
    }
}