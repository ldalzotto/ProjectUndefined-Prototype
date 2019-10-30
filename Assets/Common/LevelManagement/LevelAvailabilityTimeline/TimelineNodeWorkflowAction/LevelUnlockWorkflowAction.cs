using System;
using Timelines;
using UnityEngine;

namespace LevelManagement
{
    [Serializable]
    public struct LevelUnlockWorkflowActionV2 : TimelineNodeWorkflowActionV2<LevelAvailabilityTimelineNodeID>
    {
        [SerializeField] private LevelZoneChunkID levelZoneChunkToUnlock;

        public LevelUnlockWorkflowActionV2(LevelZoneChunkID levelZoneChunkToUnlock)
        {
            this.levelZoneChunkToUnlock = levelZoneChunkToUnlock;
        }

        public void Execute(TimelineNodeV2<LevelAvailabilityTimelineNodeID> timelineNodeRefence)
        {
            LevelAvailabilityManager.Get().UnlockLevel(this.levelZoneChunkToUnlock);
        }
    }
}