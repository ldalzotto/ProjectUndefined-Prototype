using CoreGame;
using Timelines;

namespace LevelManagement
{
    public class LevelAvailabilityTimelineEventManager : GameSingleton<LevelAvailabilityTimelineEventManager>
    {
        public void OnLevelCompleted()
        {
            TimelinesEventManager.Get().OnScenarioActionExecuted(new LevelCompletedTimelineAction(LevelManager.Get().GetCurrentLevel()));
        }
    }
}