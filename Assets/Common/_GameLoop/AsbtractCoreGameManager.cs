using System.Collections;
using CoreGame;
using Input;
using LevelManagement;
using Persistence;
using Timelines;
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
            this.DestroyUnityRenderingDebugUpdater();
        }

        protected void BeforeTick(float d)
        {
            PersistanceManager.Get().Tick(d);
            if (levelType != LevelType.STARTMENU) LevelChunkFXTransitionManager.Get().Tick(d);
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
        private void DestroyUnityRenderingDebugUpdater()
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