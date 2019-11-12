using System;
using AnimatorPlayable;
using CoreGame;
using Damage;
using Health;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using UnityEngine;
using UnityEngine.Serialization;
using Weapon;

namespace TrainingLevel
{
    [Serializable]
    [SceneHandleDraw]
    public class SoliderEnemyDefinition : AbstractInteractiveObjectV2Definition
    {
        [DrawNested] public InteractiveObjectBoxLogicColliderDefinition InteractiveObjectBoxLogicColliderDefinition;
        [DrawNested] public AIAgentDefinition AIAgentDefinition;

        [DrawNested]
        [Inline(CreateAtSameLevelIfAbsent = true)]
        public SightObjectSystemDefinition SightObjectSystemDefinition;

        public TransformMoveManagerComponentV3 AITransformMoveManagerComponentV3;

        [Inline(CreateAtSameLevelIfAbsent = true)]
        public HealthSystemDefinition HealthSystemDefinition;

        [Inline(CreateAtSameLevelIfAbsent = true)] [DrawNested]
        public SoldierAIBehaviorDefinition SoldierAIBehaviorDefinition;

        [FormerlySerializedAs("StunningDamageDealingSystemDefinition")] [Inline(CreateAtSameLevelIfAbsent = true)]
        public StunningDamageDealerReceiverSystemDefinition stunningDamageDealerReceiverSystemDefinition;

        [Inline(CreateAtSameLevelIfAbsent = true)] [DrawNested]
        public WeaponHandlingSystemDefinition WeaponHandlingSystemDefinition;

        [DrawNested] public FiringTargetPositionSystemDefinition FiringTargetPositionSystemDefinition;

        [Inline(CreateAtSameLevelIfAbsent = true)]
        public A_AnimationPlayableDefinition LocomotionAnimation;

        public override CoreInteractiveObject BuildInteractiveObject(GameObject interactiveGameObject)
        {
            return new SoliderEnemy(InteractiveGameObjectFactory.Build(interactiveGameObject), this);
        }
    }
}