using AnimatorPlayable;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using PlayerActions;

namespace PlayerObject
{
    [SceneHandleDraw]
    public class PlayerInteractiveObjectInitializer : InteractiveObjectInitializer
    {
        [DrawNested] public InteractiveObjectLogicColliderDefinition InteractiveObjectLogicCollider;

        public A_AnimationPlayableDefinition LocomotionAnimation;
        public PlayerActionInherentData FiringPlayerActionInherentData;

        public override void Init()
        {
            var PlayerInteractiveObject = new PlayerInteractiveObject(InteractiveGameObjectFactory.Build(gameObject), this);
            PlayerInteractiveObjectManager.Get().Init(PlayerInteractiveObject);
        }
    }
}