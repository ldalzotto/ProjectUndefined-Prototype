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
            var PlayerInteractiveObject = new PlayerAimingInteractiveObject(InteractiveGameObjectFactory.Build(gameObject), this.PlayerInteractiveObjectDefinition);
            return PlayerInteractiveObject;
        }
    }
}