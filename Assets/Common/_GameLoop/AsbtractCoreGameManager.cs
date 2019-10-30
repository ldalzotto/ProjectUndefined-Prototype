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
            }
            else
            {
                GameInputManager.Get().Init(CursorLockMode.Locked);
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


        protected void OnStart()
        {
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
    }
}