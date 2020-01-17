using System.Collections;
using CoreGame;
using Input;
using LevelManagement;
using Persistence;
using Timelines;
using TimeManagement;
using UnityEngine;

namespace GameLoop
{
    public abstract class AsbtractCoreGameManager : MonoBehaviour
    {
        private LevelType levelType;

        protected void OnAwake(LevelType levelType)
        {
            new GameLogHandler();

            this.levelType = levelType;

            PersistanceManager.Get().Init();
            StartLevelManager.Get().Init();
            if (levelType == LevelType.STARTMENU)
            {
                GameInputManager.Get().Init(CursorLockMode.Confined);
                Cursor.visible = true;
            }
            else
            {
                GameInputManager.Get().Init(CursorLockMode.Locked);
                Cursor.visible = false;
            }

            LevelAvailabilityManager.Get().Init();
            LevelAvailabilityTimelineManager.Get().Init();
            LevelManager.Get().Init(levelType);

            if (this.levelType != LevelType.STARTMENU)
            {
                LevelChunkFXTransitionManager.Get().Init();
                CoreGameSingletonInstances.Coroutiner.StartCoroutine(InitializeTimelinesAtEndOfFrame());
            }
        }

        /// <summary>
        /// /!\ No initialisation logic must be placed here. Initialisation logic must be in <see cref="OnAwake"/>.
        /// </summary>
        protected void OnStart()
        {
        }

        /// <summary>
        /// /!\ This method must be called before EVERYTHING else in the FixedTick update.
        /// </summary>
        protected void BeforeFixedTickGameLogic(out float d, out float unscaled)
        {
            GameInputManager.Get().FixedTick();
            TimeManagementManager.Get().FixedTick();

            /// When the time is frozen, we set the Physics to autosimulation.
            /// The reason is that even if time is stopped (Time.deltaTime == 0f), some physics object may be created.
            /// If Physics.autoSimulation is set to false then no Physics occurs when Time.deltaTime == 0f. So we force it to calculate Physics events event if the time is frozen.
            Physics.autoSimulation = !TimeManagementManager.Get().IsTimeFrozen();
            
            d = TimeManagementManager.Get().GetCurrentFixedDeltaTime();
            unscaled = TimeManagementManager.Get().GetCurrentDeltaTimeUnscaled();
        }

        /// <summary>
        /// /!\ This method is called when the time is frozen <see cref="TimeManagementManager.IsTimeFrozen"/>
        /// It manually simulate the physics world.
        /// </summary>
        protected void BeforeFixedTickTimeFrozenLogic(out float d, out float unscaled)
        {
            d = 0f;
            unscaled = TimeManagementManager.Get().GetCurrentDeltaTimeUnscaled();

            if (Physics.autoSimulation)
            {
                Physics.Simulate(d);
            }
        }

        /// <summary>
        /// /!\ This method must be called before EVERYTHING else in the Tick update.
        /// </summary>
        protected void BeforeTickGameLogic(out float d, out float unscaled)
        {
            GameInputManager.Get().Tick();
            PersistanceManager.Get().Tick();

            TimeManagementManager.Get().Tick();

            d = TimeManagementManager.Get().GetCurrentDeltaTime();
            unscaled = TimeManagementManager.Get().GetCurrentDeltaTimeUnscaled();

            if (levelType != LevelType.STARTMENU) LevelChunkFXTransitionManager.Get().Tick(d);
        }

        /// <summary>
        /// /!\ This method must be called before EVERYTHING else in the LateTick update.
        /// </summary>
        protected void BeforeLateTickGameLogic(out float d, out float unscaled)
        {
            GameInputManager.Get().LateTick();
            TimeManagementManager.Get().LateTick();

            d = TimeManagementManager.Get().GetCurrentDeltaTime();
            unscaled = TimeManagementManager.Get().GetCurrentDeltaTimeUnscaled();
        }

        private IEnumerator InitializeTimelinesAtEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            ATimelinesManager.Get().InitTimelinesAtEndOfFrame();
        }

        /// <summary>
        /// We must destroy the debug updater object because it has conflicts with the new input system.
        /// When the new input system will be realeased, we can remove this method. 
        /// </summary>
        [RuntimeInitializeOnLoadMethod]
        static void DestroyUnityRenderingDebugUpdater()
        {
            if (Debug.isDebugBuild)
            {
                var debugUpdaterObject = GameObject.Find("[Debug Updater]");
                if (debugUpdaterObject != null)
                {
                    GameObject.Destroy(debugUpdaterObject);
                }
            }
        }
    }
}