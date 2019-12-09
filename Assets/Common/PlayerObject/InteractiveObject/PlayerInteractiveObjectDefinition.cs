using System;
using AnimatorPlayable;
using Damage;
using Health;
using InteractiveObjects_Interfaces;
using OdinSerializer;
using PlayerActions;
using PlayerLowHealth;
using ProjectileDeflection;
using UnityEngine.Serialization;
using Weapon;

namespace PlayerObject
{
    [Serializable]
    [SceneHandleDraw]
    public class PlayerInteractiveObjectDefinition : SerializedScriptableObject
    {
        [DrawNested] [Inline(CreateAtSameLevelIfAbsent = true)]
        public InteractiveObjectBoxLogicColliderDefinition InteractiveObjectLogicCollider;

        [DrawNested] [Inline(CreateAtSameLevelIfAbsent = true)]
        public AIAgentDefinition AIAgentDefinition;

        [Inline(CreateAtSameLevelIfAbsent = true)]
        public WeaponHandlingSystemDefinition WeaponHandlingSystemDefinition;

        [Inline(CreateAtSameLevelIfAbsent = true)]
        public FiringTargetPositionSystemDefinition FiringTargetPositionSystemDefinition;

        [Inline(CreateAtSameLevelIfAbsent = true)]
        public A_AnimationPlayableDefinition LocomotionAnimation;

        [Inline(CreateAtSameLevelIfAbsent = true)] [DrawNested]
        public PlayerActionInherentData FiringPlayerActionInherentData;

        [Inline(CreateAtSameLevelIfAbsent = true)]
        public HealthSystemDefinition HealthSystemDefinition;

        [Inline(CreateAtSameLevelIfAbsent = true)]
        public StunningDamageDealerReceiverSystemDefinition StunningDamageDealerReceiverSystemDefinition;

        [Inline(CreateAtSameLevelIfAbsent = true)]
        public LowHealthPlayerSystemDefinition LowHealthPlayerSystemDefinition;

        [FormerlySerializedAs("ProjectileDeflectionDefinition")] [DrawNested] [Inline(CreateAtSameLevelIfAbsent = true)]
        public ProjectileDeflectionActorDefinition projectileDeflectionActorDefinition;
    }
}