using CoreGame;
using Timelines;

namespace LevelManagement
{
    public class LevelAvailabilityTimelineNodeManagerV2 : TimelineNodeManagerV2<LevelAvailabilityTimelineNodeID>
    {
        protected override bool isPersisted => true;
        protected override TimelineID TimelineID => TimelineID.LEVEL_AVAILABILITY_TIMELINE;

        public LevelAvailabilityTimelineNodeManagerV2()
        {
            this.Init();
        }
    }

    public class LevelAvailabilityTimelineManager : GameSingleton<LevelAvailabilityTimelineManager>
    {
        private LevelAvailabilityTimelineNodeManagerV2 LevelAvailabilityTimelineNodeManagerV2;

        public void Init()
        {
            //call ctor
        }

        public LevelAvailabilityTimelineManager()
        {
            this.LevelAvailabilityTimelineNodeManagerV2 = new LevelAvailabilityTimelineNodeManagerV2();
            ATimelinesManager.Get().RegisterTimeline(this.LevelAvailabilityTimelineNodeManagerV2);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            ATimelinesManager.Get().UnRegisterTimeline(this.LevelAvailabilityTimelineNodeManagerV2);
        }
    }
}