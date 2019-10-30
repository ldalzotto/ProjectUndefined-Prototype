using InteractiveObjects_Interfaces;
using LevelManagement;
using RTPuzzle;

namespace InteractiveObjects
{
    public class LevelCompletionInteractiveObject : CoreInteractiveObject
    {
        private LevelCompletionInteractiveObjectInitializerData LevelCompletionInitializerData;

        private LevelCompletionZoneSystem LevelCompletionZoneSystem;

        public LevelCompletionInteractiveObject(LevelCompletionInteractiveObjectInitializerData LevelCompletionInitializerData,
            IInteractiveGameObject interactiveGameObject, bool IsUpdatedInMainManager = true)
        {
            this.LevelCompletionInitializerData = LevelCompletionInitializerData;
            base.BaseInit(interactiveGameObject, IsUpdatedInMainManager);
        }

        public override void Init()
        {
            this.LevelCompletionZoneSystem = new LevelCompletionZoneSystem(this, this.LevelCompletionInitializerData.LevelCompletionZoneSystemDefinition,
                new InteractiveObjectTagStruct {IsPlayer = 1},
                this.OnLevelCompletionTriggerEnterPlayer);
            this.interactiveObjectTag = new InteractiveObjectTag {IsLevelCompletionZone = true};
        }

        public override void Destroy()
        {
            this.LevelCompletionZoneSystem.OnDestroy();
            base.Destroy();
        }

        private void OnLevelCompletionTriggerEnterPlayer(CoreInteractiveObject IntersectedInteractiveObject)
        {
            LevelAvailabilityTimelineEventManager.Get().OnLevelCompleted();
        }
    }
}