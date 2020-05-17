using AnimatorPlayable;
using InteractiveObjects;
using InteractiveObjects_Interfaces;

namespace PlayerObject
{
    [SceneHandleDraw]
    public class PlayerInteractiveObjectInitializer : InteractiveObjectInitializer
    {
        [DrawNested] public InteractiveObjectLogicColliderDefinition InteractiveObjectLogicCollider;

        public A_AnimationPlayableDefinition LocomotionAnimation;

        public override void Init()
        {
            var PlayerInteractiveObject = new PlayerInteractiveObject(InteractiveGameObjectFactory.Build(gameObject), InteractiveObjectLogicCollider, LocomotionAnimation);
            PlayerInteractiveObjectManager.Get().Init(PlayerInteractiveObject);
        }
    }
}