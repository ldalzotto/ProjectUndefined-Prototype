using System.Collections.Generic;
using System.Linq;
using CoreGame;
using InteractiveObjects;
using LevelManagement;
using PlayerObject;
using PlayerObject_Interfaces;
using RangeObjects;
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
        private TransformStruct InitialCameraPivotPointTransform;
        private TestControllerConfiguration TestControllerConfiguration = TestControllerConfiguration.Get();

        public TestScenarioManager()
        {
            PlayerInteractiveObjectDestinationReachedEvent.Get().RegisterOnPlayerInteractiveObjectDestinationReachedEventListener(this.OnPlayerDestinationReached);
            this.InitialCameraPivotPointTransform = new TransformStruct(GameObject.FindGameObjectWithTag(TagConstants.CAMERA_PIVOT_POINT_TAG).transform);
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
            aTestScenarioDefinition.BeforeObjectInitialization();
            RangeObjectV2Manager.InitializeAllRangeObjects();
            InteractiveObjectV2Manager.Get().InitializeAllInteractiveObjectsInitializer();

            /// By default player has infinite health
            PlayerInteractiveObjectManager.Get().PlayerInteractiveObject.DealDamage(9999999, null);

            this.SequencedActionPlayer = new SequencedActionPlayer(aTestScenarioDefinition.BuildScenarioActions());
            this.SequencedActionPlayer.Play();
        }

        private void ClearTest()
        {
            this.TestControllerConfiguration.TestControllerDefinition.ClearTest = false;
            this.SequencedActionPlayer.Kill();

            var allInteractiveObjects = InteractiveObjectV2Manager.Get().InteractiveObjects;

            List<RangeObjectV2> ExcludedRangesToDestroy = new List<RangeObjectV2>();

            for (var i = allInteractiveObjects.Count - 1; i >= 0; i--)
            {
                //We dont remove the level chunk
                if (allInteractiveObjects[i].GetType() != typeof(LevelChunkInteractiveObject))
                {
                    allInteractiveObjects[i].Destroy();
                }
                else
                {
                    ExcludedRangesToDestroy.Add((allInteractiveObjects[i] as LevelChunkInteractiveObject).GetLevelChunkRangeObject());
                }
            }

            for (var i = RangeObjectV2Manager.Get().RangeObjects.Count - 1; i >= 0; i--)
            {
                if (!ExcludedRangesToDestroy.Contains(RangeObjectV2Manager.Get().RangeObjects[i]))
                {
                    RangeObjectV2Manager.Get().RangeObjects[i].OnDestroy();
                }
            }

            MonoBehaviour.Destroy(TestEntitiesPrefabInstance);
            GameTestMockedInputManager.MockedInstance.GetGameTestMockedXInput().GameTestInputMockedValues.Reset();

            ///Reset camera position
            GameObject.FindGameObjectWithTag(TagConstants.CAMERA_PIVOT_POINT_TAG).transform.position = this.InitialCameraPivotPointTransform.WorldPosition;
            GameObject.FindGameObjectWithTag(TagConstants.CAMERA_PIVOT_POINT_TAG).transform.eulerAngles = this.InitialCameraPivotPointTransform.WorldRotationEuler;
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