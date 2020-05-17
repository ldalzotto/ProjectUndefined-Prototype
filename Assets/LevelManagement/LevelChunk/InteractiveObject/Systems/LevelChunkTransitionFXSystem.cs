using InteractiveObjects;
using InteractiveObjects_Interfaces;

namespace LevelManagement
{
    public class LevelChunkTransitionFXSystem : AInteractiveObjectSystem
    {
        public TransitionableLevelFXType TransitionableLevelFXType { get; private set; }

        public LevelChunkTransitionFXSystem(LevelChunkInteractiveObject associatedPostProcessVolume)
        {
            this.TransitionableLevelFXType = associatedPostProcessVolume.InteractiveGameObject.InteractiveGameObjectParent.GetComponentInChildren<TransitionableLevelFXType>();
            this.TransitionableLevelFXType.Init();
        }
    }
}