using System;
using AnimatorPlayable;
using Damage;
using PlayerAim;
using Health;
using InteractiveObject_Animation;
using InteractiveObjects_Interfaces;
using OdinSerializer;
using InteractiveObjectAction;
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

        [Inline()] public A_AnimationPlayableDefinition BaseLocomotionAnimationDefinition;

        [FormerlySerializedAs("FiringPlayerActionInherentData")] [Inline(CreateAtSameLevelIfAbsent = true)] [DrawNested]
        public InteractiveObjectActionInherentData firingInteractiveObjectActionInherentData;

        [Inline(CreateAtSameLevelIfAbsent = true)]
        public HealthSystemDefinition HealthSystemDefinition;

        [Inline(CreateAtSameLevelIfAbsent = true)]
        public StunningDamageDealerReceiverSystemDefinition StunningDamageDealerReceiverSystemDefinition;

        [Inline(CreateAtSameLevelIfAbsent = true)]
        public LowHealthPlayerSystemDefinition LowHealthPlayerSystemDefinition;

        [FormerlySerializedAs("projectileDeflectionInteractiveObjectActionInherentData")] [FormerlySerializedAs("ProjectileDeflectionSystemInherentData")] [Inline(CreateAtSameLevelIfAbsent = true)]
        public ProjectileDeflectionTrackingInteractiveObjectActionInherentData projectileDeflectionTrackingInteractiveObjectActionInherentData;

        [Inline()] public DeflectingProjectileInteractiveObjectActionInherentData DeflectingProjectileInteractiveObjectActionInherentData;

        [Inline()] [DrawNested] public PlayerDashActionStateBehaviorInputDataSystemDefinition PlayerDashActionStateBehaviorInputDataSystemDefinition;

        [Inline()] public InteractiveObjectActionInherentData PlayerDashTeleportationActionDefinition;

        [Inline(CreateAtSameLevelIfAbsent = true)]
        public PlayerVisualEffectSystemDefinition PlayerVisualEffectSystemDefinition;
    }
}