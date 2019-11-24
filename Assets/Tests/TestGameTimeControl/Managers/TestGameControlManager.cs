using CoreGame;
using UnityEditor;
using UnityEngine;

namespace Tests
{
    /// <summary>
    /// Holds and update the <see cref="Time.timeScale"/> value for testing scene.
    /// </summary>
    public class TestGameControlManager : GameSingleton<TestGameControlManager>
    {
        private TestGameControlDefinition TestGameControlDefinition;

        public TestGameControlManager()
        {
            /// We can do this because this is an editor only script
            this.TestGameControlDefinition = TestControllerConfiguration.Get().TestControllerDefinition.TestGameControlDefinition;
            this.TestGameControlDefinition.TimeScale = 1f;
        }

        public void Tick()
        {
            Time.timeScale = this.TestGameControlDefinition.TimeScale;
            Time.fixedDeltaTime = 0.02f * this.TestGameControlDefinition.TimeScale;
        }
    }
}