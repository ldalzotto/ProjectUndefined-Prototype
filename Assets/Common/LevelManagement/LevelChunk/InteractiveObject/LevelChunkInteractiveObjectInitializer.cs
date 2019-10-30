using InteractiveObjects;

namespace LevelManagement
{
    [SceneHandleDraw]
    public class LevelChunkInteractiveObjectInitializer : InteractiveObjectInitializer
    {
        [DrawNested] public LevelChunkInteractiveObjectDefinition LevelChunkInteractiveObjectDefinition;

        public override void Init()
        {
            new LevelChunkInteractiveObject(InteractiveGameObjectFactory.Build(gameObject), this.LevelChunkInteractiveObjectDefinition);
        }
    }
}