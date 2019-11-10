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
        public SightObjectSystemDefinition SightObjectSystemDefinition;
        public TransformMoveManagerComponentV3 AITransformMoveManagerComponentV3;

        [Inline(CreateAtSameLevelIfAbsent = true)]
        public HealthSystemDefinition HealthSystemDefinition;

        [FormerlySerializedAs("StunningDamageDealingSystemDefinition")] [Inline(CreateAtSameLevelIfAbsent = true)]
        public StunningDamageDealerReceiverSystemDefinition stunningDamageDealerReceiverSystemDefinition;

        [Inline(CreateAtSameLevelIfAbsent = true)]
        public WeaponHandlingSystemDefinition WeaponHandlingSystemDefinition;

        public FiringTargetPositionSystemDefinition FiringTargetPositionSystemDefinition;
        public A_AnimationPlayableDefinition LocomotionAnimation;


        public override CoreInteractiveObject BuildInteractiveObject(GameObject interactiveGameObject)
        {
            return new SoliderEnemy(InteractiveGameObjectFactory.Build(interactiveGameObject), this);
        }
    }
}