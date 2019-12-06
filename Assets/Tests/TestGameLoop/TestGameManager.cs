using System;
using GameLoop;
using Persistence;
using PlayerObject;
using PlayerObject_Interfaces;
using UnityEngine;

namespace Tests
{
    public class TestGameManager : GameManager
    {
        protected override void AfterGameManagerPersistanceInstanceInitialization()
        {
            base.AfterGameManagerPersistanceInstanceInitialization();

            /// Mock Input to have no effect
            GameTestMockedInputManager.SetupForTestScene();
            /// No level transition
            LevelTransitionManagerMock.SetupForTestScene();
            /// No persistance
            PersistanceManager.SetInstance(new MockPersistanceManager());
        }

        protected override void Start()
        {
            base.OnStart();

            /// Preventing switching to the "Game" window in editor.
            UnityEditor.SceneView.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));

            base.Start();

            PlayerInteractiveObjectCreatedEvent.Get().RegisterPlayerInteractiveObjectCreatedEvent((p) =>
            {
                /// Player is forced to be controlled by agent
                PlayerInteractiveObjectManager.Get().PlayerInteractiveObject.PlayerMoveManager.ForceSwitchToAgent();
            });
        }

        protected override void Update()
        {
            TestGameControlManager.Get().Tick();
            TestScenarioManager.Get().Tick(Time.deltaTime);
            base.Update();
        }
    }

    public class MockPersistanceManager : PersistanceManager
    {
        public override void Init()
        {
        }

        public override void Tick()
        {
        }

        public override void OnPersistRequested(Action persistAction)
        {
        }

        public override T Load<T>(string folderPath, string dataPath, string filename, string fileExtension)
        {
            Debug.Log(MyLog.Format("Load MockPersistanceManager"));
            return default(T);
        }
    }
}