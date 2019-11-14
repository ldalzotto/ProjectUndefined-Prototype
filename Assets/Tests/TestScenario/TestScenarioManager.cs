﻿using CoreGame;
using InteractiveObjects;
using LevelManagement;
using PlayerObject;
using PlayerObject_Interfaces;
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
        /// <summary>
        /// The instance of <see cref="ATestScenarioDefinition.TestEntitiesPrefab"/>.
        /// It is created when the test scenario start <see cref="StartTest"/>.
        /// /!\ It must be disposed when the test scenario is cleared <see cref="ClearTest"/>
        /// </summary>
        private GameObject TestEntitiesPrefabInstance;

        private SequencedActionPlayer SequencedActionPlayer;

        private TestControllerConfiguration TestControllerConfiguration = TestControllerConfiguration.Get();

        public TestScenarioManager()
        {
            PlayerInteractiveObjectDestinationReachedEvent.Get().RegisterOnPlayerInteractiveObjectDestinationReachedEventListener(this.OnPlayerDestinationReached);
        }

        public void Tick(float d)
        {
            if (this.TestControllerConfiguration.TestControllerDefinition.StartTest)
            {
                this.StartTest();
            }

            if (this.TestControllerConfiguration.TestControllerDefinition.ClearTest)
            {
                this.ClearTest();
            }

            if (this.SequencedActionPlayer != null)
            {
                this.SequencedActionPlayer.Tick(d);
            }
        }

        private void StartTest()
        {
            this.TestControllerConfiguration.TestControllerDefinition.StartTest = false;
            var aTestScenarioDefinition = this.TestControllerConfiguration.TestControllerDefinition.aTestScenarioDefinition;
            this.TestEntitiesPrefabInstance = GameObject.Instantiate(aTestScenarioDefinition.TestEntitiesPrefab);
            InteractiveObjectV2Manager.Get().InitializeAllInteractiveObjectsInitializer();
            this.SequencedActionPlayer = new SequencedActionPlayer(aTestScenarioDefinition.BuildScenarioActions());
            this.SequencedActionPlayer.Play();
        }

        private void ClearTest()
        {
            this.TestControllerConfiguration.TestControllerDefinition.ClearTest = false;
            var allInteractiveObjects = InteractiveObjectV2Manager.Get().InteractiveObjects;
            for (var i = allInteractiveObjects.Count - 1; i >= 0; i--)
            {
                //We dont remove the level chunk
                if (allInteractiveObjects[i].GetType() != typeof(LevelChunkInteractiveObject))
                {
                    allInteractiveObjects[i].Destroy();
                }
            }
            
            MonoBehaviour.Destroy(TestEntitiesPrefabInstance);
        }

        /// <summary>
        /// Registered event from <see cref="PlayerInteractiveObjectDestinationReachedEvent"/>
        /// </summary>
        private void OnPlayerDestinationReached()
        {
            IActionAbortedOnDestinationReachedHelper.ProcessOnDestinationReachedEvent(this.SequencedActionPlayer);
        }
    }
}