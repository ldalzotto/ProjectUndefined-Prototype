using System;
using Damage;
using Health;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using UnityEngine;

namespace TrainingLevel
{
    [Serializable]
    [SceneHandleDraw]
    public class SoliderEnemyDefinition : AbstractInteractiveObjectV2Definition
    {
        [DrawNested] public InteractiveObjectBoxLogicColliderDefinition InteractiveObjectBoxLogicColliderDefinition;

        [Inline(CreateAtSameLevelIfAbsent = true)]
        public HealthSystemDefinition HealthSystemDefinition;

        [Inline(CreateAtSameLevelIfAbsent = true)]
        public StunningDamageDealingSystemDefinition StunningDamageDealingSystemDefinition;

        public override CoreInteractiveObject BuildInteractiveObject(GameObject interactiveGameObject)
        {
            return new SoliderEnemy(InteractiveGameObjectFactory.Build(interactiveGameObject), this);
        }
    }
}