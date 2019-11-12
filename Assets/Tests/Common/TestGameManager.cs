using System;
using GameLoop;
using Persistence;
using UnityEngine;

namespace Tests
{
    public class TestGameManager : GameManager
    {
        protected override void AfterGameManagerPersistanceInstanceInitialization()
        {
            base.AfterGameManagerPersistanceInstanceInitialization();

            //Mock Input to have no effect
            GameTestMockedInputManager.SetupForTestScene();
            PersistanceManager.SetInstance(new MockPersistanceManager());
        }
    }

    public class MockPersistanceManager : PersistanceManager
    {
        public override void Init()
        {
        }

        public override void Tick(float d)
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