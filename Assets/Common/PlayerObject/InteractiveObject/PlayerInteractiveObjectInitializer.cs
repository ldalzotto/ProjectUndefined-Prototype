using InteractiveObjects;

namespace PlayerObject
{
    [SceneHandleDraw]
    public class PlayerInteractiveObjectInitializer : InteractiveObjectInitializer
    {
        [Inline()]
        [DrawNested]
        public PlayerInteractiveObjectDefinition PlayerInteractiveObjectDefinition;
        protected override CoreInteractiveObject InitializationLogic()
        {
            var PlayerInteractiveObject = new PlayerInteractiveObject(InteractiveGameObjectFactory.Build(gameObject), this.PlayerInteractiveObjectDefinition);
            PlayerInteractiveObjectManager.Get().Init(PlayerInteractiveObject);
            return PlayerInteractiveObject;
        }
    }
}