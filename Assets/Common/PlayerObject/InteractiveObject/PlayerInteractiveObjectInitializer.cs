using AnimatorPlayable;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using PlayerActions;
using Weapon;

namespace PlayerObject
{
    [SceneHandleDraw]
    public class PlayerInteractiveObjectInitializer : InteractiveObjectInitializer
    {
        [DrawNested] public InteractiveObjectBoxLogicColliderDefinition InteractiveObjectLogicCollider;
        [DrawNested] public AIAgentDefinition AIAgentDefinition;
        [Inline(CreateAtSameLevelIfAbsent = true)]
        public WeaponHandlingSystemDefinition WeaponHandlingSystemDefinition;

        public FiringTargetPositionSystemDefinition FiringTargetPositionSystemDefinition;
        public A_AnimationPlayableDefinition LocomotionAnimation;
        [DrawNested] public PlayerActionInherentData FiringPlayerActionInherentData;

        public override CoreInteractiveObject Init()
        {
            var PlayerInteractiveObject = new PlayerInteractiveObject(InteractiveGameObjectFactory.Build(gameObject), this);
            PlayerInteractiveObjectManager.Get().Init(PlayerInteractiveObject);
            return PlayerInteractiveObject;
        }
    }
}