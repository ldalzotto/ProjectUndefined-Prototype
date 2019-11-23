using System.Collections.Generic;
using CoreGame;
using UnityEngine;

namespace LevelManagement
{
    public class LevelManagerEventManager : GameSingleton<LevelManagerEventManager>
    {
        public List<AsyncOperation> OnLevelToAnotherLevel(LevelZonesID nextLevel)
        {
            return LevelManager.Get().OnLevelToAnotherLevel(nextLevel);
        }

        public List<AsyncOperation> RestartCurrentLevel()
        {
            return LevelManager.Get().RestartCurrentLevel();
        }

        public List<AsyncOperation> OnStartMenuToLevel(LevelZonesID nextLevel)
        {
            StartLevelManager.Get().OnStartLevelChange(nextLevel);
            return LevelManager.Get().OnStartMenuToLevel(nextLevel);
        }

        public void OnLevelChunkLoaded(LevelZoneChunkID levelZoneChunkID)
        {
            LevelChunkInteractiveObject.DestroyAllDestroyOnStartObjects();
            LevelManager.Get().OnLevelChunkLoaded(levelZoneChunkID);
        }

        public void OnChunkLevelEnter(LevelChunkInteractiveObject enteredLevelChunkTracker)
        {
            LevelChunkFXTransitionManager.Get().OnChunkLevelEnter(enteredLevelChunkTracker);
            LevelManager.Get().OnChunkLevelEnter(enteredLevelChunkTracker);
        }

        public void OnChunkLevelExit(LevelChunkInteractiveObject exitedLevelChunkTracker)
        {
            LevelChunkFXTransitionManager.Get().OnChunkLevelExit(exitedLevelChunkTracker);
        }
    }
}