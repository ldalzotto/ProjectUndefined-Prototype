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
        [DrawNested] [Inline()] public InteractiveObjectBoxLogicColliderDefinition InteractiveObjectLogicCollider;

        [DrawNested] [Inline()] public AIAgentDefinition AIAgentDefinition;

        [Inline()] public WeaponHandlingSystemDefinition WeaponHandlingSystemDefinition;
        [Inline()] public FiringTargetPositionSystemDefinition FiringTargetPositionSystemDefinition;
        [Inline()] public A_AnimationPlayableDefinition LocomotionAnimation;
        [Inline()] [DrawNested] public PlayerActionInherentData FiringPlayerActionInherentData;

        public override CoreInteractiveObject Init()
        {
            var PlayerInteractiveObject = new PlayerInteractiveObject(InteractiveGameObjectFactory.Build(gameObject), this);
            PlayerInteractiveObjectManager.Get().Init(PlayerInteractiveObject);
            return PlayerInteractiveObject;
        }
    }
}