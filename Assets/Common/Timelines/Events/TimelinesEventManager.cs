using CoreGame;

namespace Timelines
{
    public class TimelinesEventManager : GameSingleton<TimelinesEventManager>
    {
        public void OnScenarioActionExecuted(TimelineAction scenarioAction)
        {
            foreach (var timeline in ATimelinesManager.Get().TimelineManagers)
            {
                timeline.IncrementGraph(scenarioAction);
            }
        }
    }
}