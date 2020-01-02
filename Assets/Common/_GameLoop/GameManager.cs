using CameraManagement;
using CoreGame;
using Health;
using Input;
using InputDynamicTextMenu;
using InteractiveObjects;
using LevelManagement;
using Obstacle;
using ParticleObjects;
using PlayerActions;
using PlayerObject;
using RangeObjects;
using SelectableObject;
using Targetting;
using TimeManagement;
using Tutorial;
using UnityEngine;
using VisualFeedback;
using Weapon;

namespace GameLoop
{
    public class GameManager : AsbtractCoreGameManager
    {
        private void Awake()
        {
            FindObjectOfType<GameManagerPersistanceInstance>().Init();
            AfterGameManagerPersistanceInstanceInitialization();
            //Level chunk initialization
            OnAwake(LevelType.GAME);
        }

        protected virtual void AfterGameManagerPersistanceInstanceInitialization()
        {
        }

        protected virtual void Start()
        {
            base.OnStart();

            PlayerInteractiveObjectManager.Get().InitializeEvents();
            CameraMovementJobManager.Get().InitializeEvents();

            InteractiveObjectCursorScreenIntersectionManager.Get().InitializeEvents();
            RangeObjectV2Manager.Get().InitializeEvents();
            GroundEffectsManagerV2.Get().InitializeEvents();

            RangeObjectV2Manager.Get().Init();
            GroundEffectsManagerV2.Get().Init(LevelConfigurationGameObject.Get().LevelConfigurationData.LevelRangeEffectInherentData);
            InteractiveObjectV2Manager.Get().Init();

            CircleFillBarRendererManager.Get().Init();
            TutorialManager.Get().Init();
            SelectableObjectManagerV2.Get().Init(GameInputManager.Get());

            HealthUIManager.Get().Init();
            PlayerActionEntryPoint.Get().Init();
            TargetCursorManager.Get();

            InputDynamicTextMenuManager.Get();
        }

        private void FixedUpdate()
        {
            base.BeforeFixedTickGameLogic(out float d, out float unscaled);

            if (!TimeManagementManager.Get().IsTimeFrozen())
            {
                PlayerActionEntryPoint.Get().FixedTick(d);
                PlayerInteractiveObjectManager.Get().FixedTick(d);
                InteractiveObjectV2Manager.Get().FixedTick(d);
            }
        }

        protected virtual void Update()
        {
            base.BeforeTickGameLogic(out float d, out float unscaled);

            /// Begin Jobs
            CameraMovementJobManager.Get().SetupJob(unscaled);

            if (!TimeManagementManager.Get().IsTimeFrozen())
            {
                BeziersMovementJobManager.Get().SetupJob(d);
                ObstacleOcclusionCalculationManagerV2.Get().Tick(d);
                RangeIntersectionCalculationManagerV2.Get().Tick(d);
            }
            /// End Jobs

            CameraMovementJobManager.Get().Tick();

            if (!CameraMovementJobManager.Get().IsCameraRotating())
            {
                TargetCursorManager.Get().Tick(unscaled);
            }

            if (!TimeManagementManager.Get().IsTimeFrozen())
            {
                TutorialManager.Get().Tick(d);
                PuzzleTutorialEventSenderManager.Get().Tick(d);

                BlockingCutscenePlayerManager.Get().Tick(d);

                PlayerActionEntryPoint.Get().BeforePlayerTick(d);

                PlayerInteractiveObjectManager.Get().Tick(d);
                PlayerInteractiveObjectManager.Get().AfterTicks(d);

                PlayerActionEntryPoint.Get().AfterPlayerTick(d);

                RangeObjectV2Manager.Get().Tick(d);

                InteractiveObjectV2Manager.Get().Tick(d);
                InteractiveObjectV2Manager.Get().AfterTicks(d);

                ParticleObjectsManager.Get().Tick(d);

                GroundEffectsManagerV2.Get().Tick(d);
                SelectableObjectManagerV2.Get().Tick(d);
                CircleFillBarRendererManager.Get().Tick(d);
            }
            else
            {
                PlayerActionEntryPoint.Get().TickTimeFrozen(d);
                PlayerInteractiveObjectManager.Get().TickTimeFrozen(d);
                InteractiveObjectV2Manager.Get().TickTimeFrozen(d);
            }

            BeziersMovementJobManager.Get().EnsureCalculationIsDone();
        }

        private void LateUpdate()
        {
            base.BeforeLateTickGameLogic(out float d, out float unscaled);

            if (!TimeManagementManager.Get().IsTimeFrozen())
            {
                PlayerInteractiveObjectManager.Get().LateTick(d);
                InteractiveObjectV2Manager.Get().LateTick(d);
                PlayerActionEntryPoint.Get().LateTick(d);

                ObstacleOcclusionCalculationManagerV2.Get().LateTick();
                RangeIntersectionCalculationManagerV2.Get().LateTick();
            }
        }

        private void OnDestroy()
        {
            GameSingletonManagers.Get().OnDestroy();
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                PlayerActionEntryPoint.Get().GizmoTick();
                ObstaclesListenerManager.Get().GizmoTick();
                RangeIntersectionCalculatorManager.Get().GizmoTick();
            }
        }

        private void OnGUI()
        {
            if (Application.isPlaying) PlayerActionEntryPoint.Get().GUITick();
        }
    }
}