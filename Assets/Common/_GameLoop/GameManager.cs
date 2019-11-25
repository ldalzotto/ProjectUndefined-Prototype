using CameraManagement;
using CoreGame;
using Health;
using Input;
using InteractiveObjects;
using LevelManagement;
using Obstacle;
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
            PlayerInteractiveObjectManager.Get().InitializeEvents();
            CameraMovementManager.Get().InitializeEvents();
            TargettableInteractiveObjectScreenIntersectionManager.Get().InitializeEvents();
            RangeObjectV2Manager.Get().InitializeEvents();
            GroundEffectsManagerV2.Get().InitializeEvents();
            
            RangeObjectV2Manager.Get().Init();
            GroundEffectsManagerV2.Get().Init(LevelConfigurationGameObject.Get().LevelConfigurationData.LevelRangeEffectInherentData);
            InteractiveObjectV2Manager.Get().Init();

            CameraMovementManager.Get().Init();

            CircleFillBarRendererManager.Get().Init();
            TutorialManager.Get().Init();
            SelectableObjectManagerV2.Get().Init(GameInputManager.Get());

            HealthUIManager.Get().Init();
            PlayerActionEntryPoint.Get().Init();
            TargetCursorManager.Get();
        }

        protected virtual void Update()
        {
            
            var unscaled = TimeManagementManager.Get().GetCurrentDeltaTimeUnscaled();
            
            /// The CameraMovementManager can potentially change TimeManagementManager value, that is why it is updated before everything
            CameraMovementManager.Get().Tick(unscaled);
            
            var d = TimeManagementManager.Get().GetCurrentDeltaTime();
            
            BeforeTick(d);

            TutorialManager.Get().Tick(d);
            PuzzleTutorialEventSenderManager.Get().Tick(d);

            BlockingCutscenePlayerManager.Get().Tick(d);

            if (!CameraMovementManager.Get().IsCameraRotating())
            {
                TargetCursorManager.Get().Tick(d);
            }
            
            PlayerActionEntryPoint.Get().Tick(d);

            PlayerInteractiveObjectManager.Get().Tick(d);
            PlayerInteractiveObjectManager.Get().AfterTicks(d);


            WeaponRecoilTimeManager.Get().Tick(d);

            ObstacleOcclusionCalculationManagerV2.Get().Tick(d);
            RangeIntersectionCalculationManagerV2.Get().Tick(d);

            RangeObjectV2Manager.Get().Tick(d);

            InteractiveObjectV2Manager.Get().Tick(d);

            InteractiveObjectV2Manager.Get().AfterTicks(d);

            GroundEffectsManagerV2.Get().Tick(d);
            DottedLineRendererManager.Get().Tick();
            SelectableObjectManagerV2.Get().Tick(d);
            CircleFillBarRendererManager.Get().Tick(d);
        }

        private void LateUpdate()
        {
            var d = TimeManagementManager.Get().GetCurrentDeltaTime();

            PlayerInteractiveObjectManager.Get().LateTick(d);
            InteractiveObjectV2Manager.Get().LateTick(d);
            PlayerActionEntryPoint.Get().LateTick(d);

            ObstacleOcclusionCalculationManagerV2.Get().LateTick();
            RangeIntersectionCalculationManagerV2.Get().LateTick();
        }

        private void FixedUpdate()
        {
            TimeManagementManager.Get().UpdateFixedDeltaTime();
            
            var d = TimeManagementManager.Get().GetCurrentFixedDeltaTime();

            PlayerInteractiveObjectManager.Get().FixedTick(d);
            InteractiveObjectV2Manager.Get().FixedTick(d);
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