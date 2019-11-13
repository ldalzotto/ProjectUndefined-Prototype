using CoreGame;
using InteractiveObjects;
using SequencedAction;
using Tests.TestScenario;
using UnityEngine;

namespace Tests
{
    /// <summary>
    /// Holds the currently playing <see cref="ATestScenarioDefinition"/> and it's associated
    /// <see cref="ATestScenarioDefinition.TestEntitiesPrefab"/> instance <see cref="TestEntitiesPrefabInstance"/>.
    /// </summary>
    public class TestScenarioManager : GameSingleton<TestScenarioManager>
    {
        private GameObject TestEntitiesPrefabInstance;

        private SequencedActionPlayer SequencedActionPlayer;

        private TestControllerConfiguration TestControllerConfiguration = TestControllerConfiguration.Get();
        
        public void Tick(float d)
        {
            if (this.TestControllerConfiguration.TestControllerDefinition.StartTest)
            {
                this.TestControllerConfiguration.TestControllerDefinition.StartTest = false;
                this.StartTest();
            }

            if (this.SequencedActionPlayer != null)
            {
                this.SequencedActionPlayer.Tick(d);
            }
        }
        
        private void StartTest()
        {
            var aTestScenarioDefinition = this.TestControllerConfiguration.TestControllerDefinition.aTestScenarioDefinition;
            this.TestEntitiesPrefabInstance = GameObject.Instantiate(aTestScenarioDefinition.TestEntitiesPrefab);
            InteractiveObjectV2Manager.Get().InitializeAllInteractiveObjectsInitializer();
            this.SequencedActionPlayer = new SequencedActionPlayer(aTestScenarioDefinition.BuildScenarioActions());
            this.SequencedActionPlayer.Play();
        }
    }
}