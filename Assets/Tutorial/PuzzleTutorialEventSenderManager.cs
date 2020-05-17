using CoreGame;

namespace Tutorial
{
    public class PuzzleTutorialEventSenderManager : GameSingleton<PuzzleTutorialEventSenderManager>
    {
        private TutorialManager tutorialManager = TutorialManager.Get();

        public void Tick(float d)
        {
            //    if (!tutorialManager.GetTutorialCurrentState(TutorialStepID.TUTORIAL_MOVEMENT) && !tutorialManager.IsTutorialStepPlaying()) tutorialManager.PlayTutorialStep(TutorialStepID.TUTORIAL_MOVEMENT);
            //    if (!tutorialManager.GetTutorialCurrentState(TutorialStepID.PUZZLE_CONTEXT_ACTION_AWAKE) && !tutorialManager.IsTutorialStepPlaying()) tutorialManager.PlayTutorialStep(TutorialStepID.PUZZLE_CONTEXT_ACTION_AWAKE);
        }
    }
}