using System.Collections.Generic;
using CoreGame;

namespace Timelines
{
    public class ATimelinesManager : GameSingleton<ATimelinesManager>
    {
        public List<ITimelineNodeManager> TimelineManagers { get; private set; } = new List<ITimelineNodeManager>();

        public void RegisterTimeline(ITimelineNodeManager ITimelineNodeManager)
        {
            this.TimelineManagers.Add(ITimelineNodeManager);
        }

        public void UnRegisterTimeline(ITimelineNodeManager ITimelineNodeManager)
        {
            this.TimelineManagers.Remove(ITimelineNodeManager);
        }

        public void InitTimelinesAtEndOfFrame()
        {
        }
    }
}