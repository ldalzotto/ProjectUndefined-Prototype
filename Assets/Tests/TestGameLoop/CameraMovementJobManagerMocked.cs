using CameraManagement;
using PlayerObject_Interfaces;

namespace Tests
{
    /// <summary>
    /// The camera doesn't move
    /// </summary>
    public class CameraMovementJobManagerMocked : CameraMovementJobManager
    {
        private static CameraMovementJobManagerMocked MockedInstance;
        public static void SetupForTestScene()
        {
            MockedInstance = new CameraMovementJobManagerMocked();
            SetInstance(MockedInstance);
        }

        public override void SetupJob(float d)
        {
        }

        public override void Tick()
        {
        }

        protected override void OnPlayerInteractiveObjectCreated(IPlayerInteractiveObject IPlayerInteractiveObject)
        {
        }
    }
}