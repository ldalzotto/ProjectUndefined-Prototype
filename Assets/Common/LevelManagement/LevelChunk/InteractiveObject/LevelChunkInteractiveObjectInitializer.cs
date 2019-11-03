using InteractiveObjects;

namespace LevelManagement
{
    [SceneHandleDraw]
    public class LevelChunkInteractiveObjectInitializer : InteractiveObjectInitializer
    {
        [DrawNested] public LevelChunkInteractiveObjectDefinition LevelChunkInteractiveObjectDefinition;

        public override CoreInteractiveObject Init()
        {
            return new LevelChunkInteractiveObject(InteractiveGameObjectFactory.Build(gameObject), this.LevelChunkInteractiveObjectDefinition);
        }
    }
}