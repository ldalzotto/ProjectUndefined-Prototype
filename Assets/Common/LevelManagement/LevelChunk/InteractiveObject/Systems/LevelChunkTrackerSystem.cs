using System;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using RangeObjects;

namespace LevelManagement
{
    public class LevelChunkTrackerSystem : AInteractiveObjectSystem
    {
        public RangeObjectV2 LevelChunkTrackerRange { get; private set; }

        public LevelChunkTrackerSystem(LevelChunkInteractiveObject AssociatedLevelChunkInteractiveObject, LevelChunkInteractiveObjectDefinition LevelChunkInteractiveObjectDefinition,
            Action<CoreInteractiveObject> OnLevelChunkTriggerEnterAction, Action<CoreInteractiveObject> OnLevelChunkTriggerExitAction)
        {
            this.LevelChunkTrackerRange = new BoxRangeObjectV2(AssociatedLevelChunkInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent, new BoxRangeObjectInitialization()
            {
                BoxRangeTypeDefinition = new BoxRangeTypeDefinition()
                {
                    Center = LevelChunkInteractiveObjectDefinition.LocalCenter,
                    Size = LevelChunkInteractiveObjectDefinition.LocalSize
                },
                RangeTypeID = RangeTypeID.NOT_DISPLAYED,
                IsTakingIntoAccountObstacles = false
            }, AssociatedLevelChunkInteractiveObject, nameof(LevelChunkTrackerSystem.LevelChunkTrackerRange));

            this.LevelChunkTrackerRange.RegisterPhysicsEventListener(
                new InteractiveObjectPhysicsEventListenerDelegated((InteractiveObjectPhysicsTriggerInfo) => InteractiveObjectPhysicsTriggerInfo.GetOtherInteractiveObjectTag().IsPlayer, OnLevelChunkTriggerEnterAction, OnLevelChunkTriggerExitAction)
            );
        }
    }
}