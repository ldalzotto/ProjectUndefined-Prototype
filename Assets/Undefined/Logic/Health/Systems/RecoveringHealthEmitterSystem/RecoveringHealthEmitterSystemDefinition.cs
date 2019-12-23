using System;
using OdinSerializer;
using RangeObjects;

namespace Health
{
    [Serializable]
    [SceneHandleDraw]
    public class RecoveringHealthEmitterSystemDefinition : SerializedScriptableObject
    {
        [DrawNested] [Inline(CreateAtSameLevelIfAbsent = true)]
        public RangeObjectInitialization RecveringHealthTriggerDefinition;

        public float RecoveredHealth;

        public static implicit operator RecoveringHealthEmitterSystemDefinitionStruct(RecoveringHealthEmitterSystemDefinition RecoveringHealthEmitterSystemDefinition)
        {
            return new RecoveringHealthEmitterSystemDefinitionStruct()
            {
                RecveringHealthTriggerDefinition = RecoveringHealthEmitterSystemDefinition.RecveringHealthTriggerDefinition,
                RecoveredHealth = RecoveringHealthEmitterSystemDefinition.RecoveredHealth
            };
        }
    }

    public struct RecoveringHealthEmitterSystemDefinitionStruct
    {
        /// <summary>
        /// /!\ This is stil a SerializedScriptableObject.
        /// </summary>
        public RangeObjectInitialization RecveringHealthTriggerDefinition;

        public float RecoveredHealth;
    }
}