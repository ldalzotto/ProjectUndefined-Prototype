using System;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using RangeObjects;

namespace LevelManagement
{
    public class LevelChunkTrackerSystem : AInteractiveObjectSystem
    {
        private RangeObjectV2 LevelChunkTrackerRange;

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
                new InteractiveObjectPhysicsEventListenerDelegated(new InteractiveObjectTag(isPlayer: 1), OnLevelChunkTriggerEnterAction, OnLevelChunkTriggerExitAction)
            );
        }
    }
}