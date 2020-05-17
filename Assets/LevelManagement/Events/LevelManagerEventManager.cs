using System.Collections.Generic;
using CoreGame;
using UnityEngine;

namespace LevelManagement
{
    public class LevelManagerEventManager : GameSingleton<LevelManagerEventManager>
    {
        public List<AsyncOperation> OnPuzzleToPuzzleLevel(LevelZonesID nextLevel)
        {
            return LevelManager.Get().OnAdventureToPuzzleLevel(nextLevel);
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