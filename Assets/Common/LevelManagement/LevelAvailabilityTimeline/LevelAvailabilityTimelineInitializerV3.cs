using System;
using System.Collections.Generic;
using Timelines;

namespace LevelManagement
{
    [Serializable]
    public class LevelAvailabilityTimelineInitializerV3 : TimelineInitializerV3<LevelAvailabilityTimelineNodeID>
    {
#if UNITY_EDITOR
        protected override Dictionary<LevelAvailabilityTimelineNodeID, TimelineNodeV2<LevelAvailabilityTimelineNodeID>> BuildTimeline()
        {
            return new Dictionary<LevelAvailabilityTimelineNodeID, TimelineNodeV2<LevelAvailabilityTimelineNodeID>>()
            {
                {LevelAvailabilityTimelineNodeID.SewersLV1_Unlock, SewersLV1_Unlock_Node()}
            };
        }

        protected override List<LevelAvailabilityTimelineNodeID> BuildInitialNodes()
        {
            return new List<LevelAvailabilityTimelineNodeID>
            {
                LevelAvailabilityTimelineNodeID.SewersLV1_Unlock
            };
        }


        private static TimelineNodeV2<LevelAvailabilityTimelineNodeID> SewersLV1_Unlock_Node()
        {
            return new TimelineNodeV2<LevelAvailabilityTimelineNodeID>(
                transitionRequirements: null,
                onStartNodeAction: new List<TimelineNodeWorkflowActionV2<LevelAvailabilityTimelineNodeID>>()
                {
                    new LevelUnlockWorkflowActionV2(LevelZoneChunkID.RTP_PUZZLE_CREATION_TEST),
                    new LevelUnlockWorkflowActionV2(LevelZoneChunkID.INFINITE_LEVEL)
                },
                onExitNodeAction: null
            );
        }
#endif
    }
}