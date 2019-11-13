using InteractiveObjects;

namespace LevelManagement
{
    [SceneHandleDraw]
    public class LevelChunkInteractiveObjectInitializer : InteractiveObjectInitializer
    {
        [DrawNested] public LevelChunkInteractiveObjectDefinition LevelChunkInteractiveObjectDefinition;

        protected override CoreInteractiveObject InitializationLogic()
        {
            return new LevelChunkInteractiveObject(InteractiveGameObjectFactory.Build(gameObject), this.LevelChunkInteractiveObjectDefinition);
        }
    }
}