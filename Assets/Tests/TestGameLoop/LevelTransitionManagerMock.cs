using LevelManagement;

namespace Tests
{
    public class LevelTransitionManagerMock : LevelTransitionManager
    {
        public static LevelTransitionManagerMock MockedInstance;
      
        public static void SetupForTestScene()
        {
            MockedInstance = new LevelTransitionManagerMock();
            SetInstance(MockedInstance);
        }

        public override void OnStartMenuToLevel(LevelZonesID nextZone)
        {
        }

        public override void RestartCurrentLevel()
        {
        }

        public override void OnPuzzleToPuzzleLevel(LevelZonesID nextZone)
        {
        }
    }
}