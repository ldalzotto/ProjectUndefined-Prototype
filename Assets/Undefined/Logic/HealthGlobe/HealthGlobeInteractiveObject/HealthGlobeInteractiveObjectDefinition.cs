using System;
using Health;
using InteractiveObjects;
using OdinSerializer;
using UnityEngine;

namespace HealthGlobe
{
    [Serializable]
    [SceneHandleDraw]
    public class HealthGlobeInteractiveObjectDefinition : AbstractInteractiveObjectV2Definition
    {
        [Inline(CreateAtSameLevelIfAbsent = true)]
        [DrawNested]
        public RecoveringHealthEmitterSystemDefinition RecoveringHealthEmitterSystemDefinition;

        public static implicit operator HealthGlobeInteractiveObjectDefinitionStruct(HealthGlobeInteractiveObjectDefinition HealthGlobeInteractiveObjectDefinition)
        {
            return new HealthGlobeInteractiveObjectDefinitionStruct()
            {
                RecoveringHealthEmitterSystemDefinitionStruct = HealthGlobeInteractiveObjectDefinition.RecoveringHealthEmitterSystemDefinition
            };
        }

        public override CoreInteractiveObject BuildInteractiveObject(GameObject interactiveGameObject)
        {
            return null;
        }
    }

    public struct HealthGlobeInteractiveObjectDefinitionStruct
    {
        public RecoveringHealthEmitterSystemDefinitionStruct RecoveringHealthEmitterSystemDefinitionStruct;

        public void SetRecoveredHealthValue(float RecoveredHealth)
        {
            this.RecoveringHealthEmitterSystemDefinitionStruct.RecoveredHealth = RecoveredHealth;
        }
    }
}