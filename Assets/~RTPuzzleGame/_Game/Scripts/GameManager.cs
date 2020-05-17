using CameraManagement;
using CoreGame;
using GameLoop;
using Input;
using InteractiveObjects;
using LevelManagement;
using Obstacle;
using PlayerActions;
using PlayerObject;
using RangeObjects;
using SelectableObject;
using Tutorial;
using UnityEngine;
using VisualFeedback;

namespace RTPuzzle
{
    public class GameManager : AsbtractCoreGameManager
    {
#if UNITY_EDITOR
        private EditorOnlyManagers EditorOnlyManagers;
#endif

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

        private void Start()
        {
            OnStart();


            RangeObjectV2Manager.Get().Init();
            GroundEffectsManagerV2.Get().Init(LevelManagementConfigurationGameObject.Get().LevelConfiguration.ConfigurationInherentData[LevelManager.Get().GetCurrentLevel()].LevelRangeEffectInherentData);
            InteractiveObjectV2Manager.Get().Init();

            CameraMovementManager.Get().Init();

            CircleFillBarRendererManager.Get().Init();
            TutorialManager.Get().Init();
            SelectableObjectManagerV2.Get().Init(GameInputManager.Get());

            PlayerActionEntryPoint.Get().Init();
#if UNITY_EDITOR
            EditorOnlyManagers = new EditorOnlyManagers();
            EditorOnlyManagers.Init();
#endif
        }

        private void Update()
        {
            var d = Time.deltaTime;

            BeforeTick(d);


            TutorialManager.Get().Tick(d);
            PuzzleTutorialEventSenderManager.Get().Tick(d);

            BlockingCutscenePlayerManager.Get().Tick(d);

            PlayerActionEntryPoint.Get().Tick(d);

            PlayerInteractiveObjectManager.Get().Tick(d);
            PlayerInteractiveObjectManager.Get().AfterTicks(d);

            CameraMovementManager.Get().Tick(d);

            ObstacleOcclusionCalculationManagerV2.Get().Tick(d);
            RangeIntersectionCalculationManagerV2.Get().Tick(d);

            RangeObjectV2Manager.Get().Tick(d);

            InteractiveObjectV2Manager.Get().Tick(d);

            InteractiveObjectV2Manager.Get().AfterTicks(d);

            GroundEffectsManagerV2.Get().Tick(d);
            DottedLineRendererManager.Get().Tick();
            SelectableObjectManagerV2.Get().Tick(d);
            CircleFillBarRendererManager.Get().Tick(d);

#if UNITY_EDITOR
            EditorOnlyManagers.Tick(d);
#endif
        }

        private void LateUpdate()
        {
            var d = Time.deltaTime;

            PlayerInteractiveObjectManager.Get().LateTick(d);
            InteractiveObjectV2Manager.Get().LateTick(d);
            PlayerActionEntryPoint.Get().LateTick(d);

            ObstacleOcclusionCalculationManagerV2.Get().LateTick();
            RangeIntersectionCalculationManagerV2.Get().LateTick();
        }

        private void FixedUpdate()
        {
            var d = Time.fixedDeltaTime;

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

#if UNITY_EDITOR
    public class EditorOnlyManagers
    {
        public void Init()
        {
            PuzzleDebugModule.Get().Init();
        }

        public void Tick(float d)
        {
            PuzzleDebugModule.Get().Tick();
        }
    }
#endif
}