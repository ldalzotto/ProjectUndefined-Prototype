using System;
using AnimatorPlayable;
using Damage;
using Health;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using SightVisualFeedback;
using SoldierAnimation;
using SoliderAIBehavior;
using UnityEngine;
using UnityEngine.Serialization;
using Weapon;

namespace TrainingLevel
{
    [Serializable]
    [SceneHandleDraw]
    public class SoliderEnemyDefinition : AbstractInteractiveObjectV2Definition
    {
        [DrawNested] [Inline(CreateAtSameLevelIfAbsent = true)]
        public InteractiveObjectBoxLogicColliderDefinition InteractiveObjectBoxLogicColliderDefinition;

        [DrawNested] [Inline(CreateAtSameLevelIfAbsent = true)]
        public AIAgentDefinition AIAgentDefinition;

        [DrawNested] [Inline(CreateAtSameLevelIfAbsent = true)]
        public SightObjectSystemDefinition SightObjectSystemDefinition;

        [Inline(CreateAtSameLevelIfAbsent = true)]
        public TransformMoveManagerComponentV3 AITransformMoveManagerComponentV3;

        [Inline(CreateAtSameLevelIfAbsent = true)]
        public HealthSystemDefinition HealthSystemDefinition;

        [Inline(CreateAtSameLevelIfAbsent = true)] [DrawNested]
        public SoldierAIBehaviorDefinition SoldierAIBehaviorDefinition;

        [FormerlySerializedAs("StunningDamageDealingSystemDefinition")] [Inline(CreateAtSameLevelIfAbsent = true)]
        public StunningDamageDealerReceiverSystemDefinition stunningDamageDealerReceiverSystemDefinition;

        [Inline(CreateAtSameLevelIfAbsent = true)] [DrawNested]
        public WeaponHandlingSystemDefinition WeaponHandlingSystemDefinition;

        [DrawNested] [Inline(CreateAtSameLevelIfAbsent = true)]
        public FiringTargetPositionSystemDefinition FiringTargetPositionSystemDefinition;

        [Inline(CreateAtSameLevelIfAbsent = true)]
        public A_AnimationPlayableDefinition LocomotionAnimation;


        [Inline(CreateAtSameLevelIfAbsent = true)]
        public SoldierAnimationSystemDefinition SoldierAnimationSystemDefinition;

        [Inline(CreateAtSameLevelIfAbsent = true)]
        public SightVisualFeedbackSystemDefinition SightVisualFeedbackSystemDefinition;
        
        public override CoreInteractiveObject BuildInteractiveObject(GameObject interactiveGameObject)
        {
            return new SoliderEnemy(InteractiveGameObjectFactory.Build_Allocate(interactiveGameObject), this);
        }
    }
}