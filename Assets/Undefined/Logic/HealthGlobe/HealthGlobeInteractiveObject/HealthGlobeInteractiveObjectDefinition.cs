using System;
using Health;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
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

        [Inline(CreateAtSameLevelIfAbsent = true)]
        [DrawNested]
        public InteractiveObjectSphereLogicColliderDefinition InteractiveObjectSphereLogicColliderDefinition;
        
        public static implicit operator HealthGlobeInteractiveObjectDefinitionStruct(HealthGlobeInteractiveObjectDefinition HealthGlobeInteractiveObjectDefinition)
        {
            return new HealthGlobeInteractiveObjectDefinitionStruct()
            {
                RecoveringHealthEmitterSystemDefinitionStruct = HealthGlobeInteractiveObjectDefinition.RecoveringHealthEmitterSystemDefinition,
                InteractiveObjectSphereLogicColliderDefinitionStruct = HealthGlobeInteractiveObjectDefinition.InteractiveObjectSphereLogicColliderDefinition
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
        public InteractiveObjectSphereLogicColliderDefinitionStruct InteractiveObjectSphereLogicColliderDefinitionStruct;
        public void SetRecoveredHealthValue(float RecoveredHealth)
        {
            this.RecoveringHealthEmitterSystemDefinitionStruct.RecoveredHealth = RecoveredHealth;
        }
    }
}