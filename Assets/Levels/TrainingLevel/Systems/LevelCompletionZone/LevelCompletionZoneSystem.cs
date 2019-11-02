using System;
using InteractiveObjects_Interfaces;
using RangeObjects;

namespace InteractiveObjects
{
    public class LevelCompletionZoneSystem : AInteractiveObjectSystem
    {
        private RangeObjectV2 LevelCompletionZoneObject;

        public LevelCompletionZoneSystem(CoreInteractiveObject AssociatedInteractiveObject, LevelCompletionZoneSystemDefinition LevelCompletionZoneSystemDefinition,
            InteractiveObjectTag ComparedInteractiveObjectTagStruct,
            Action<CoreInteractiveObject> OnLevelCompletionTriggerEnterPlayer)
        {
            LevelCompletionZoneObject = RangeObjectV2Builder.Build(AssociatedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent,
                LevelCompletionZoneSystemDefinition.TriggerRangeObjectInitialization, AssociatedInteractiveObject, AssociatedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent.name + "_LevelCompletionZoneListener");
            LevelCompletionZoneObject.RegisterPhysicsEventListener(new RangeObjectV2PhysicsEventListener_Delegated(ComparedInteractiveObjectTagStruct, OnLevelCompletionTriggerEnterPlayer));
        }

        public override void OnDestroy()
        {
            LevelCompletionZoneObject.OnDestroy();
        }
    }
}